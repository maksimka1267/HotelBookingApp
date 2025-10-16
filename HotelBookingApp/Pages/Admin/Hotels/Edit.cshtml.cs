using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HotelBookingApp.Pages.Admin.Hotels
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        // ������ �� �������� ��� �� query (?id=)
        [BindProperty(SupportsGet = true)]
        public Guid? Id { get; set; }

        public Guid HotelId { get; set; }

        public IActionResult OnGet()
        {
            if (Id is not Guid id || id == Guid.Empty)
                return NotFound(); // ��� Redirect("/Admin/Hotels")

            HotelId = id;
            return Page();
        }
    }
}
