using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingApp.Pages.Admin.Rooms
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
