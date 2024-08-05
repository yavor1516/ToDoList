using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using To_Do_List.Models;

namespace To_Do_List.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly TaskContext _context;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(TaskContext context, ILogger<ProfileController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Profile/Index
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Profile/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index([Bind("UserId,FullName,Email,Password")] User user, string NewPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (existingUser == null)
            {
                return NotFound();
            }

            if (user.Email!=null)
            {
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;

                if (!string.IsNullOrEmpty(NewPassword))
                {
                    existingUser.Password = HashPassword(NewPassword);
                }

                _context.SaveChanges();
                ViewBag.Message = "Profile updated successfully.";
            }
            else
            {
                _logger.LogWarning("Model state is invalid: {ModelStateErrors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
            return View(existingUser);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
