using GLMS.Api.Dtos;
using GLMS.Data;
using GLMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll()
        {
            var clients = await _context.Clients
                .Select(c => new ClientDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ContactDetails = c.ContactDetails,
                    Region = c.Region
                })
                .ToListAsync();

            return Ok(clients);
        }

        // GET /api/clients/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClientDto>> GetById(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            return Ok(new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                ContactDetails = client.ContactDetails,
                Region = client.Region
            });
        }

        // POST /api/clients
        [HttpPost]
        public async Task<ActionResult<ClientDto>> Create([FromBody] ClientWriteDto dto)
        {
            var client = new Client
            {
                Name = dto.Name,
                ContactDetails = dto.ContactDetails,
                Region = dto.Region
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var result = new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                ContactDetails = client.ContactDetails,
                Region = client.Region
            };

            return CreatedAtAction(nameof(GetById), new { id = client.Id }, result);
        }

        // POST /api/clients/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClientWriteDto dto)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            client.Name = dto.Name;
            client.ContactDetails = dto.ContactDetails;
            client.Region = dto.Region;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/clients/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}