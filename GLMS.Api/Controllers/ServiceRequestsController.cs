using GLMS.Api.Dtos;
using GLMS.Commands;
using GLMS.Data;
using GLMS.Models;
using GLMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/service-requests")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestInvoker _invoker;
        private readonly ICurrencyExchangeService _currencyService;
        private readonly IContractEligibilityService _eligibilityService;

        public ServiceRequestsController(
            ApplicationDbContext context,
            RequestInvoker invoker,
            ICurrencyExchangeService currencyService,
            IContractEligibilityService eligibilityService)
        {
            _context = context;
            _invoker = invoker;
            _currencyService = currencyService;
            _eligibilityService = eligibilityService;
        }

        // GET /api/service-requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRequestDto>>> GetAll()
        {
            var requests = await _context.ServiceRequests
                .Include(r => r.Contract)
                    .ThenInclude(c => c!.Client)
                .OrderByDescending(r => r.CreatedDate)
                .Select(r => MapToDto(r))
                .ToListAsync();

            return Ok(requests);
        }

        // GET /api/service-requests/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ServiceRequestDto>> GetById(int id)
        {
            var request = await _context.ServiceRequests
                .Include(r => r.Contract)
                    .ThenInclude(c => c!.Client)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();
            return Ok(MapToDto(request));
        }

        // POST /api/service-requests
        [HttpPost]
        public async Task<ActionResult<ServiceRequestDto>> Create([FromBody] CreateServiceRequestDto dto)
        {
            //check eligibility like in the mvc
            var eligibility = await _eligibilityService.CheckEligibilityAsync(dto.ContractId);
            if (!eligibility.isEligible)
            {
                return BadRequest(eligibility.Reason);
            }

            //fetch and cache live exchange rate
            decimal rate = await _currencyService.GetUsdToZarRateAsync();

            var request = new ServiceRequest
            {
                ContractId = dto.ContractId,
                Discription = dto.Discription,
                CostUSD = dto.CostUSD,
                WeightTonnes = dto.WeightTonnes,
                ExchangeRateUsed = rate,
                CostZAR = dto.CostUSD * rate,
                Status = ServiceRequestStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();

            await _context.Entry(request).Reference(r => r.Contract).LoadAsync();
            if (request.Contract != null)
            {
                await _context.Entry(request.Contract).Reference(c => c.Client).LoadAsync();
            }

            return CreatedAtAction(nameof(GetById), new { id = request.Id }, MapToDto(request));
        }

        // PATCH /api/service-requests/5/status
        //use command pattern for status
        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<ServiceRequestActionResultDto>> UpdateStatus(
            int id, [FromBody] ServiceRequestActionDto dto)
        {            var request = await _context.ServiceRequests
                .Include(r => r.Contract)
                    .ThenInclude(c => c!.ServiceRequests)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null) return NotFound();
            if (request.Contract == null) return BadRequest("request has no associated contract.");

            ICommand command;
            switch (dto.Action.ToLowerInvariant())
            {
                case "approve":
                    command = new ApproveCommand(request, request.Contract);
                    break;
                case "reject":
                    command = new RejectCommand(request, dto.Reason ?? "No reason given");
                    break;
                case "cancel":
                    command = new CancelCommand(request);
                    break;
                default:
                    return BadRequest($"Unknown command '{dto.Action}'. Try: Approve, Reject, or Cancel.");
            }

            _invoker.ExecuteCommand(command);
            await _context.SaveChangesAsync();

            string? invoice = null;
            string message = $"{dto.Action} succeeded.";
            bool success = true;

            if (command is ApproveCommand approve && approve.LastResult != null)
            {
                success = approve.LastResult.Success;
                message = approve.LastResult.Message;
                invoice = approve.LastResult.InvoiceNumber;
            }

            return Ok(new ServiceRequestActionResultDto
            {
                Success = success,
                FinalStatus = request.Status,
                InvoiceNumber = invoice,
                Message = message
            });
        }

        // DELETE /api/service-requests/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET /api/service-requests/history
        [HttpGet("history")]
        public ActionResult<IEnumerable<CommandHistoryEntryDto>> GetHistory()
        {
            var entries = _invoker.History
                .Select(c => new CommandHistoryEntryDto { Description = c.Description })
                .ToList();
            return Ok(entries);
        }

        private static ServiceRequestDto MapToDto(ServiceRequest r) => new ServiceRequestDto
        {
            Id = r.Id,
            ContractId = r.ContractId,
            ContractServiceLevel = r.Contract?.ServiceLevel ?? string.Empty,
            ClientName = r.Contract?.Client?.Name ?? string.Empty,
            Discription = r.Discription,
            CostUSD = r.CostUSD,
            CostZAR = r.CostZAR,
            ExchangeRateUsed = r.ExchangeRateUsed,
            Status = r.Status,
            CreatedDate = r.CreatedDate,
            WeightTonnes = r.WeightTonnes
        };
    }
}