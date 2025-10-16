using HotelBookingApp.Data;
using HotelBookingApp.Models;
using HotelBookingApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly ApplicationDbContext _context;
        public HotelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> GetAllAsync() =>
            await _context.Hotels.Include(h => h.Rooms).ToListAsync();

        public async Task<Hotel?> GetByIdAsync(Guid id) =>
            await _context.Hotels.Include(h => h.Rooms)
                                 .FirstOrDefaultAsync(h => h.Id == id);

        public async Task AddAsync(Hotel hotel)
        {
            await _context.Hotels.AddAsync(hotel);
            await SaveAsync();
        }

        public async Task UpdateAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var hotel = await GetByIdAsync(id);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                await SaveAsync();
            }
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
