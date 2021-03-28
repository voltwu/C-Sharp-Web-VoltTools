using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VoltTools.Models.Views;

namespace VoltTools.Controllers
{
    public class JSONController : BaseController
    {
        public IActionResult Index([FromServices] BaseView _view)
        {
            return View(_view);
        }
        public IActionResult Escape([FromServices] BaseView _view) {
            return View(_view);
        }
    }
}
