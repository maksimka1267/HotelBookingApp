using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HotelBookingApp.Models
{
    public class Room
    {
        public Guid Id { get; set; }

        [Required]
        public string Number { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal PricePerNight { get; set; }

        [Range(1, 100)]
        public int Capacity { get; set; }

        [Required]
        public Guid HotelId { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public Hotel? Hotel { get; set; }
    }
}
