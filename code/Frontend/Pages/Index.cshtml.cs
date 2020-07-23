using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages
{
    public class IndexModel : PageModel
    {
        private readonly Client _client;

        public IndexModel(Client client)
        {
            _client = client;
        }

        public async Task OnGet()
        {
            var response = await _client.GetData();
            ViewData["Message"] = $"{response.Cache.Name}";
            ViewData["Timestamp"] = $"{response.Timestamp}";
        }
    }
}
