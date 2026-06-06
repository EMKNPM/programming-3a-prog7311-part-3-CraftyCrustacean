using GLMS.ApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace GLMS.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IGlmsApiClient _api;


        public ContractsController(IGlmsApiClient api)
        {
            _api = api;

        }

        // GET: Contracts
        public async Task<IActionResult> Index(ContractFilterDto filter)
        {
            var results = await _api.GetContractsAsync(filter);
            ViewBag.Results = results;
            return View(filter);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _api.GetContractAsync(id.Value);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new CreateContractDto());
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContractDto dto, IFormFile? signedAgreement)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(dto);
            }

            var created = await _api.CreateContractAsync(dto, signedAgreement);
            if (created == null)
            {
                ModelState.AddModelError(string.Empty, "Could not create contract.");
                await PopulateDropdowns();
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Contracts/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, ContractStatus status)
        {
            await _api.UpdateContractStatusAsync(id, status);
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _api.GetContractAsync(id.Value);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.DeleteContractAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns()
        {
            var clients = await _api.GetClientsAsync();
            ViewBag.ClientId = new SelectList(clients, "Id", "Name");
            ViewBag.ContractType = new SelectList(new[] { "Freight", "SLA", "Warehouse" });
        }
    }
}
