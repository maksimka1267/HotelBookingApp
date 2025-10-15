using HotelBookingApp.Data;
using HotelBookingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/rooms
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _context.Rooms.Include(r => r.Hotel).ToListAsync();
            return Ok(rooms);
        }

        // GET: api/rooms/search?hotelId=...&checkIn=...&checkOut=...
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(Guid? hotelId, DateTime? checkIn, DateTime? checkOut)
        {
            var query = _context.Rooms.Include(r => r.Hotel).AsQueryable();

            if (hotelId.HasValue)
                query = query.Where(r => r.HotelId == hotelId);

            if (checkIn.HasValue && checkOut.HasValue)
            {
                var bookedRoomIds = await _context.Bookings
                    .Where(b => b.CheckIn < checkOut && b.CheckOut > checkIn)
                    .Select(b => b.RoomId)
                    .ToListAsync();

                query = query.Where(r => !bookedRoomIds.Contains(r.Id));
            }

            var availableRooms = await query.ToListAsync();
            return Ok(availableRooms);
        }

        // POST: api/rooms
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Room room)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = room.Id }, room);
        }

        // PUT: api/rooms/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Room room)
        {
            if (id != room.Id) return BadRequest();
            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/rooms/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
