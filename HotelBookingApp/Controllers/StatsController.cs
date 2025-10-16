using HotelBookingApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/stats/bookings
        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookingStats()
        {
            var data = await _context.Bookings
                .GroupBy(b => b.CheckIn.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .OrderBy(g => g.Month)
                .ToListAsync();

            return Ok(data);
        }
    }
}
