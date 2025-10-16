using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingApp.Pages.Admin.Rooms
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        // ��������� � ������� /Edit/{id}, � query ?id=
        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }

        public Guid RoomId { get; set; }

        public IActionResult OnGet()
        {
            if (Id is not Guid id || id == Guid.Empty)
                return NotFound(); // ��� Redirect("/Admin/Rooms")

            RoomId = id;
            return Page();
        }
    }
}
