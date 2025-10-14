using HotelBookingApp.Data;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingApp.Models
{
    public class Hotel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Description { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
