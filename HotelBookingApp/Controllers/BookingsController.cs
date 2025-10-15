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
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/bookings/my
        [HttpGet("my")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> GetMyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Where(b => b.UserId == user.Id)
                .ToListAsync();

            return Ok(bookings);
        }

        // GET: api/bookings
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .ToListAsync();
            return Ok(bookings);
        }

        // POST: api/bookings
        [HttpPost]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Create([FromBody] Booking booking)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            booking.UserId = user.Id;

            // Проверка доступности номера
            bool isBooked = await _context.Bookings.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                b.CheckIn < booking.CheckOut &&
                b.CheckOut > booking.CheckIn);

            if (isBooked)
                return BadRequest("Room is already booked for selected dates.");

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return Ok(booking);
        }

        // DELETE: api/bookings/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Клиент может удалять только свои
            if (User.IsInRole("Client") && booking.UserId != user.Id)
                return Forbid();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
