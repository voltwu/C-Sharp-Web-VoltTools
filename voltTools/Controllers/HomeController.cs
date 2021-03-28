using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VoltTools.Models;
using VoltTools.Models.Views;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public async Task<IActionResult> Page([FromServices] PageView _page, string urltext)
        {
            if (string.IsNullOrEmpty(urltext))
                return Redirect("index.html");

            _page.urltext = urltext;

            await _page.LoadData();

            return View(_page);
        }
        public IActionResult Error() {
            return View();
        }
    }
}
