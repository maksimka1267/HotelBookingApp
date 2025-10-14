using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingApp.Models
{
    public class Room
    {
        public Guid Id { get; set; }
        [Required]
        public string Number { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int Capacity { get; set; }

        [ForeignKey("Hotel")]
        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;
    }
}
