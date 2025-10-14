using HotelBookingApp.Models;

namespace HotelBookingApp.Repositories.Interfaces
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(Guid id);
        Task<IEnumerable<Room>> GetByHotelAsync(Guid hotelId);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(Guid id);
        Task SaveAsync();
    }
}
