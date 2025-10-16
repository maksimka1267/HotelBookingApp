using HotelBookingApp.Data;
using HotelBookingApp.Models;
using HotelBookingApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;
        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync() =>
            await _context.Bookings
                          .Include(b => b.Room)
                          .ThenInclude(r => r.Hotel)
                          .Include(b => b.User)
                          .ToListAsync();

        public async Task<IEnumerable<Booking>> GetByUserAsync(string userId) =>
            await _context.Bookings
                          .Include(b => b.Room)
                          .ThenInclude(r => r.Hotel)
                          .Where(b => b.UserId == userId)
                          .ToListAsync();

        public async Task<Booking?> GetByIdAsync(Guid id) =>
            await _context.Bookings.Include(b => b.Room)
                                   .ThenInclude(r => r.Hotel)
                                   .Include(b => b.User)
                                   .FirstOrDefaultAsync(b => b.Id == id);

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await SaveAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var booking = await GetByIdAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await SaveAsync();
            }
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
