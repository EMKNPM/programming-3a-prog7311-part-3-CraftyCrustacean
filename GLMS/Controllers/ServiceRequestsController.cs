using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GLMS.Data;
using GLMS.Models;
using GLMS.Commands;
using GLMS.Processors;
using GLMS.Services;

namespace GLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RequestInvoker _invoker;
        private readonly ICurrencyExchangeService _currencyService;
        private readonly IContractEligibilityService _contractEligibilityService;

        public ServiceRequestsController(ApplicationDbContext context, RequestInvoker invoker, ICurrencyExchangeService currencyService, IContractEligibilityService contractEligibilityService)
        {
            _context = context;
            _invoker = invoker;
            _currencyService = currencyService;
            _contractEligibilityService = contractEligibilityService;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServiceRequests.ToListAsync());
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(m => m.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public IActionResult Create()
        {
            var eligibleContracts = _context.Contracts.Include(c => c.Client).Where(c => c.Status != ContractStatus.Expired && c.Status != ContractStatus.OnHold)
                .Select(c => new
                {
                    c.Id,
                    Display = $"{c.Client!.Name} - {c.ServiceLevel} ({c.Status})"
                }).ToList();

            ViewData["ContractId"] = new SelectList(eligibleContracts, "Id", "Display");
            return View();
        }

        // POST: ServiceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,Discription,CostUSD,WeightTonnes")] ServiceRequest serviceRequest)
        {
            var eligibility = await _contractEligibilityService.CheckEligibilityAsync(serviceRequest.ContractId);
            if (!eligibility.isEligible)
            {
                ModelState.AddModelError(nameof(serviceRequest.ContractId), eligibility.Reason!);
            }

            if (ModelState.IsValid)
            {
                decimal exchangeRate = await _currencyService.GetUsdToZarRateAsync();
                serviceRequest.ExchangeRateUsed = exchangeRate;
                serviceRequest.CostZAR = serviceRequest.CostUSD * exchangeRate;

                serviceRequest.CreatedDate = DateTime.UtcNow;
                serviceRequest.Status = ServiceRequestStatus.Pending;

                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var eligibleContracts = _context.Contracts.Include(c => c.Client).Where(c => c.Status != ContractStatus.Expired && c.Status != ContractStatus.OnHold)
                .Select(c => new
                {
                    c.Id,
                    Display = $"{c.Client!.Name} - {c.ServiceLevel} ({c.Status})"
                }).ToList();
            ViewData["ContractId"] = new SelectList(eligibleContracts, "Id", "Display", serviceRequest.ContractId);
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return NotFound();
            }
            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContractId,Contract,Discription,CostUSD,CostZAR,ExchangeRateUsed,Status,CreatedDate")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.Id == id);
        }

        //Post: ServiceRequests/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.ServiceRequests.Include(r => r.Contract).ThenInclude(c => c!.ServiceRequests).FirstOrDefaultAsync(r  => r.Id == id);

            if (request == null || request.Contract == null) return NotFound();
            
            var command = new ApproveCommand(request, request.Contract);
            _invoker.ExecuteCommand(command);

            if (command.LastResult != null && !command.LastResult.Success)
            {
                TempData["errorMessage"] = command.LastResult.Message;
            }
            else if (command.LastResult != null)
            {
                TempData["successMessage"] = command.LastResult.Message;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new {id});
        }

        //Post: ServiceRequests/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();
            
            if(string.IsNullOrWhiteSpace(reason))
            {
                reason = "No reason provided";
            }

            _invoker.ExecuteCommand(new RejectCommand(request, reason));
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });

        }

        //Post: ServiceRequests/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel (int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            _invoker.ExecuteCommand(new CancelCommand(request));
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        //get: ServiceRequest/History
        public IActionResult History()
        {
            return View(_invoker.History);
        }
    }
}
