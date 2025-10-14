using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingApp.Models
{
    public class Booking
    {
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public Users User { get; set; } = null!;

        [Required]
        public Guid RoomId { get; set; }
        [ForeignKey("RoomId")]
        public Room Room { get; set; } = null!;

        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }

        public bool IsConfirmed { get; set; } = false;
    }
}
