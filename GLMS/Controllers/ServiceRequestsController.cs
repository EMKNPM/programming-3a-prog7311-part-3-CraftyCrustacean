using GLMS.ApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IGlmsApiClient _api;


        public ServiceRequestsController(IGlmsApiClient api)
        {
            _api = api;

        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _api.GetServiceRequestsAsync();
            return View(requests);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _api.GetServiceRequestAsync(id.Value);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: ServiceRequests/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdown(null);
            return View(new CreateServiceRequestDto());
        }

        // POST: ServiceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdown(dto.ContractId);
                return View(dto);
            }

            var created = await _api.CreateServiceRequestAsync(dto);
            if (created == null)
            {
                ModelState.AddModelError(string.Empty, "Could not create service request");
                await PopulateDropdown(dto.ContractId);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var request = await _api.GetServiceRequestAsync(id.Value);
            if (request == null) return NotFound();
            return View(request);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.DeleteServiceRequestAsync(id);
            return RedirectToAction(nameof(Index));
        }

        //Post: ServiceRequests/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _api.ExecuteActionAsync(id, new ServiceRequestActionDto { Action = "Approve" });
            SetActionMessage(result);
            return RedirectToAction(nameof(Details), new { id });
        }

        //Post: ServiceRequests/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var result = await _api.ExecuteActionAsync(id, new ServiceRequestActionDto
            {
                Action = "Reject",
                Reason = reason
            });
            SetActionMessage(result);
            return RedirectToAction(nameof(Details), new { id });

        }

        //Post: ServiceRequests/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel (int id)
        {
            var result = await _api.ExecuteActionAsync(id, new ServiceRequestActionDto { Action = "Cancel" });
            SetActionMessage(result);
            return RedirectToAction(nameof(Details), new { id });
        }

        //get: ServiceRequest/History
        public async Task<IActionResult> History()
        {
            var history = await _api.GetCommandHistoryAsync();
            return View(history);
        }

        private async Task PopulateDropdown(int? selectedId)
        {
            var contracts = await _api.GetContractsAsync(new ContractFilterDto());
            var eligible = contracts
                .Where(c => c.Status != ContractStatus.Expired && c.Status != ContractStatus.OnHold)
                .Select(c => new
                {
                    c.Id,
                    Display = $"{c.ClientName} - {c.ServiceLevel} ({c.Status})"
                })
                .ToList();
            ViewBag.ContractId = new SelectList(eligible, "Id", "Display", selectedId);
        }

        private void SetActionMessage(ServiceRequestActionResultDto? result)
        {
            if (result == null)
            {
                TempData["errorMessage"] = "Action failed";
                return;
            }
            if (result.Success)
                TempData["successMessage"] = result.Message;
            else
                TempData["errorMessage"] = result.Message;
        }
    }
}
