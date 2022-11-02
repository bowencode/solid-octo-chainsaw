using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Demo.Notes.Web.Host.Pages
{
    [Authorize]
    public class NotesListModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
