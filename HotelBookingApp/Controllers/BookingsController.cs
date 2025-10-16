using HotelBookingApp.Data;
using HotelBookingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // Если анти-CSRF включён глобально для MVC, раскомментируй следующую строку:
    // [IgnoreAntiforgeryToken]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // -------- DTOs --------
        public record BookingCreateDto(Guid RoomId, DateTime CheckIn, DateTime CheckOut);
        public record BookingListItemDto(
            Guid Id,
            Guid RoomId,
            string RoomNumber,
            Guid HotelId,
            string HotelName,
            DateTime CheckIn,
            DateTime CheckOut,
            bool IsConfirmed
        );

        // GET: api/bookings/my
        [HttpGet("my")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<ActionResult<IEnumerable<BookingListItemDto>>> GetMyBookings(CancellationToken ct)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var items = await _context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == user.Id)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .OrderByDescending(b => b.CheckIn)
                .Select(b => new BookingListItemDto(
                    b.Id,
                    b.RoomId,
                    b.Room.Number,
                    b.Room.HotelId,
                    b.Room.Hotel.Name,
                    b.CheckIn,
                    b.CheckOut,
                    b.IsConfirmed
                ))
                .ToListAsync(ct);

            return Ok(items);
        }

        // GET: api/bookings
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<BookingListItemDto>>> GetAll(CancellationToken ct)
        {
            var items = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Room).ThenInclude(r => r.Hotel)
                .Include(b => b.User)
                .OrderByDescending(b => b.CheckIn)
                .Select(b => new BookingListItemDto(
                    b.Id,
                    b.RoomId,
                    b.Room.Number,
                    b.Room.HotelId,
                    b.Room.Hotel.Name,
                    b.CheckIn,
                    b.CheckOut,
                    b.IsConfirmed
                ))
                .ToListAsync(ct);

            return Ok(items);
        }

        // POST: api/bookings
        [HttpPost]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Create([FromBody] BookingCreateDto dto, CancellationToken ct)
        {
            // базовая валидация
            if (dto.CheckIn.Date >= dto.CheckOut.Date)
                return ValidationProblem("'CheckIn' must be earlier than 'CheckOut'.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var room = await _context.Rooms
                .AsNoTracking()
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == dto.RoomId, ct);

            if (room is null) return NotFound("Room not found.");

            // пересечения по датам
            var overlaps = await _context.Bookings.AnyAsync(b =>
                b.RoomId == dto.RoomId &&
                b.CheckIn < dto.CheckOut &&
                b.CheckOut > dto.CheckIn, ct);

            if (overlaps)
                return BadRequest("Room is already booked for selected dates.");

            var entity = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoomId = room.Id,
                CheckIn = dto.CheckIn.Date,
                CheckOut = dto.CheckOut.Date,
                IsConfirmed = false
            };

            _context.Bookings.Add(entity);
            await _context.SaveChangesAsync(ct);

            var result = new BookingListItemDto(
                entity.Id, entity.RoomId, room.Number, room.HotelId, room.Hotel.Name,
                entity.CheckIn, entity.CheckOut, entity.IsConfirmed
            );

            // Created (можно вернуть ссылку на "my")
            return CreatedAtAction(nameof(GetMyBookings), new { }, result);
        }

        // DELETE: api/bookings/{id}
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        {
            var entity = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id, ct);
            if (entity == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (User.IsInRole("Client") && entity.UserId != user.Id)
                return Forbid();

            _context.Bookings.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return NoContent();
        }
    }
}
