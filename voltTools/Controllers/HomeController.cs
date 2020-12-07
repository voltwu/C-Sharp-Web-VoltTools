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

        public HomeController(ILogger<HomeController> logger, IDatabase database)
        {
            _logger = logger;
        }

        public IActionResult Index([FromServices] BaseView _base)
        {
            return View(_base);
        }
        public async Task<IActionResult> Page([FromServices] PageView _page, int id)
        {
            _page.pageId = id;

            await _page.LoadData();

            return View(_page);
        }
        public IActionResult Error()
        {
            return View();
        }
    }
}
