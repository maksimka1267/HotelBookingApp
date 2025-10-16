using HotelBookingApp.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Booking
{
    public Guid Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [JsonIgnore]          // не ожидаем навигацию в JSON
    [ValidateNever]       // не валидируем навигацию при биндинге
    public Users? User { get; set; }

    [Required]
    public Guid RoomId { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public Room? Room { get; set; }

    [Required]
    public DateTime CheckIn { get; set; }

    [Required]
    public DateTime CheckOut { get; set; }

    public bool IsConfirmed { get; set; } = false;
}
