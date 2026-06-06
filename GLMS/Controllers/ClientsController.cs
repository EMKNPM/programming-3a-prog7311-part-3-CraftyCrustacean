using GLMS.ApiClient;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IGlmsApiClient _api;

        public ClientsController(IGlmsApiClient api)
        {
            _api = api;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = await _api.GetClientsAsync();
            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _api.GetClientAsync(id.Value);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create() => View();

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientWriteDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var created = await _api.CreateClientAsync(dto);
            if (created == null) {
                ModelState.AddModelError(string.Empty, "Could not create client");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _api.GetClientAsync(id.Value);
            if (client == null)
            {
                return NotFound();
            }

            ViewBag.ClientId = id.Value;
            return View(new ClientWriteDto
            {
                Name = client.Name,
                ContactDetails = client.ContactDetails,
                Region = client.Region
            });
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientWriteDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ClientId = id;
                return View(dto);
            }
            var success = await _api.UpdateClientAsync(id, dto);
            if (!success)
            {
                ViewBag.ClientId = id;
                ModelState.AddModelError(string.Empty, "Could not update client");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));

        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _api.GetClientAsync(id.Value);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _api.DeleteClientAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
