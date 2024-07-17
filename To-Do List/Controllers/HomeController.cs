using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using To_Do_List.Models;
using To_Do_List.Service;

namespace To_Do_List.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmailService _emailService;

        public HomeController(ILogger<HomeController> logger, EmailService emailService)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Tasks");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
