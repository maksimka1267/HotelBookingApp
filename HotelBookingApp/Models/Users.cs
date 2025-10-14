using Microsoft.AspNetCore.Identity;

namespace HotelBookingApp.Models
{
    public class Users:IdentityUser
    {
        public string Name { get; set; }
    }
}
