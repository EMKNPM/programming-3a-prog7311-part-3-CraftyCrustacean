using GLMS.Api.Dtos;
using GLMS.Data;
using GLMS.Factories;
using GLMS.Models;
using GLMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileStorage;

        public ContractsController(ApplicationDbContext context, IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        // GET /api/contracts (filtered)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetAll([FromQuery] ContractFilterDto filter)
        {
            IQueryable<Contract> query = _context.Contracts.Include(c => c.Client);

            if (filter.StartDateFrom.HasValue)
                query = query.Where(c => c.StartDate >= filter.StartDateFrom.Value);
            if (filter.StartDateTo.HasValue)
                query = query.Where(c => c.StartDate <= filter.StartDateTo.Value);
            if (filter.Status.HasValue)
                query = query.Where(c => c.Status == filter.Status.Value);

            var results = await query
                .OrderByDescending(c => c.StartDate)
                .Select(c => new ContractDto
                {
                    Id = c.Id,
                    ClientId = c.ClientId,
                    ClientName = c.Client!.Name,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Status = c.Status,
                    ServiceLevel = c.ServiceLevel,
                    SignedAgreementFileName = c.SignedAgreementFileName,
                    SignedAgreementFilePath = c.SignedAgreementFilePath
                })
                .ToListAsync();

            return Ok(results);
        }

        // GET /api/contracts/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ContractDto>> GetById(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null) return NotFound();

            return Ok(MapToDto(contract));
        }

        // POST /api/contracts
        [HttpPost]
        public async Task<ActionResult<ContractDto>> Create([FromForm] CreateContractDto dto, IFormFile? signedAgreement)
        {
            //make sure client exists
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == dto.ClientId);
            if (!clientExists)
            {
                return BadRequest($"Client id: {dto.ClientId}, does not exist.");
            }

            //use the factory, same as mvc
            try
            {
                IContractFactory factory = ContractFactoryResolver.GetFactory(dto.ContractType);
                IContract abstraction = factory.CreateContract(dto.StartDate, dto.EndDate, dto.ServiceLevel);
                Contract entity = abstraction.ToEntity(dto.ClientId);

                // Save file if uploaded
                if (signedAgreement != null && signedAgreement.Length > 0)
                {
                    try
                    {
                        var (fileName, filePath) = await _fileStorage.SaveAsync(signedAgreement, "contracts");
                        entity.SignedAgreementFileName = fileName;
                        entity.SignedAgreementFilePath = filePath;
                    }
                    catch (FileValidationException ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }

                _context.Contracts.Add(entity);
                await _context.SaveChangesAsync();

                await _context.Entry(entity).Reference(c => c.Client).LoadAsync();
                return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapToDto(entity));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH /api/contracts/5/status
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateContractStatusDto dto)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            contract.Status = dto.Status;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/contracts/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            //remove attached file if it exists
            _fileStorage.Delete(contract.SignedAgreementFilePath);

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private static ContractDto MapToDto(Contract c) => new ContractDto
        {
            Id = c.Id,
            ClientId = c.ClientId,
            ClientName = c.Client?.Name ?? string.Empty,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            Status = c.Status,
            ServiceLevel = c.ServiceLevel,
            SignedAgreementFileName = c.SignedAgreementFileName,
            SignedAgreementFilePath = c.SignedAgreementFilePath
        };
    }
}