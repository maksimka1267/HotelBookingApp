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
        public RoomsController(ApplicationDbContext context) => _context = context;

        // ---------- DTOs ----------
        public record RoomListItemDto(Guid Id, string Number, decimal PricePerNight, int Capacity, Guid HotelId, string? HotelName);
        public record RoomDetailsDto(Guid Id, string Number, decimal PricePerNight, int Capacity, Guid HotelId, string? HotelName);

        public record RoomCreateDto(Guid HotelId, string Number, decimal PricePerNight, int Capacity);
        public record RoomUpdateDto(Guid Id, Guid HotelId, string Number, decimal PricePerNight, int Capacity);

        // GET: api/rooms
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _context.Rooms
                .AsNoTracking()
                // Include не обязателен для проекции; EF сам сделает JOIN
                .Select(r => new RoomListItemDto(
                    r.Id,
                    r.Number,
                    r.PricePerNight,
                    r.Capacity,
                    r.HotelId,
                    r.Hotel != null ? r.Hotel.Name : null
                ))
                .ToListAsync();

            return Ok(rooms);
        }

        // GET: api/rooms/{id}
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var room = await _context.Rooms
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RoomDetailsDto(
                    r.Id,
                    r.Number,
                    r.PricePerNight,
                    r.Capacity,
                    r.HotelId,
                    r.Hotel != null ? r.Hotel.Name : null
                ))
                .FirstOrDefaultAsync();

            if (room == null) return NotFound();
            return Ok(room);
        }

        // GET: api/rooms/search?hotelId=...&checkIn=...&checkOut=...
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search(Guid? hotelId, DateTime? checkIn, DateTime? checkOut)
        {
            var query = _context.Rooms
                .AsNoTracking()
                .Include(r => r.Hotel)
                .AsQueryable();

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

            var availableRooms = await query
                .Select(r => new RoomListItemDto(
                    r.Id,
                    r.Number,
                    r.PricePerNight,
                    r.Capacity,
                    r.HotelId,
                    r.Hotel != null ? r.Hotel.Name : null
                ))
                .ToListAsync();

            return Ok(availableRooms);
        }

        // POST: api/rooms
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] RoomCreateDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var entity = new Room
            {
                Id = Guid.NewGuid(),
                HotelId = dto.HotelId,
                Number = dto.Number,
                PricePerNight = dto.PricePerNight,
                Capacity = dto.Capacity
            };

            _context.Rooms.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new
            {
                id = entity.Id,
                entity.Number,
                entity.PricePerNight,
                entity.Capacity,
                entity.HotelId
            });
        }

        // PUT: api/rooms/{id}
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] RoomUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("Route id and body id mismatch.");

            var entity = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (entity == null) return NotFound();

            // Обновляем только разрешённые поля
            entity.Number = dto.Number;
            entity.PricePerNight = dto.PricePerNight;
            entity.Capacity = dto.Capacity;
            entity.HotelId = dto.HotelId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/rooms/{id}
        [HttpDelete("{id:guid}")]
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
