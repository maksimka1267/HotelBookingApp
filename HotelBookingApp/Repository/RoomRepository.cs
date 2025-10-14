using HotelBookingApp.Data;
using HotelBookingApp.Models;
using HotelBookingApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingApp.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;
        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllAsync() =>
            await _context.Rooms.Include(r => r.Hotel).ToListAsync();

        public async Task<Room?> GetByIdAsync(Guid id) =>
            await _context.Rooms.Include(r => r.Hotel)
                                .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<Room>> GetByHotelAsync(Guid hotelId) =>
            await _context.Rooms.Where(r => r.HotelId == hotelId).ToListAsync();

        public async Task AddAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await SaveAsync();
        }

        public async Task UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);
            await SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var room = await GetByIdAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                await SaveAsync();
            }
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}
