using HotelBookingApp.Data;
using HotelBookingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public record HotelUpdateDto(Guid Id, string Name, string? Address, string? Description);
        public HotelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/hotels
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var hotels = await _context.Hotels
                .AsNoTracking()
                .OrderBy(h => h.Name)
                .Select(h => new { id = h.Id, name = h.Name })
                .ToListAsync();

            return Ok(hotels);
        }


        // GET: api/hotels/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }

        // POST: api/hotels
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Hotel hotel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
        }

        // PUT: api/hotels/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] HotelUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("Route id and body id mismatch.");

            var entity = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == id);
            if (entity == null) return NotFound();

            // ТОЛЬКО разрешённые поля
            entity.Name = dto.Name;
            entity.Address = dto.Address;
            entity.Description = dto.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // на случай, если включишь конкуррентность
                return Conflict("Concurrency conflict.");
            }

            return NoContent();
        }

        // DELETE: api/hotels/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null) return NotFound();

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
