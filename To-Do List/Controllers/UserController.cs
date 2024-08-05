using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using To_Do_List.Models;

namespace To_Do_List.Controllers
{
    public class UserController : Controller
    {
        private readonly TaskContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(TaskContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: User/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([Bind("FullName,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return View(user);
                }

                // Hash the password before storing it
                user.Password = HashPassword(user.Password);

                _context.Users.Add(user);
                try
                {
                    _context.SaveChanges();
                    _logger.LogInformation("User registered successfully: {Email}", user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error registering user: {Email}", user.Email);
                    ModelState.AddModelError("", "An error occurred while registering. Please try again.");
                    return View(user);
                }

                return RedirectToAction("Login", "User"); // Redirect to Login page after successful registration
            }

            // Log the ModelState errors
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogError(error.ErrorMessage);
                }
            }

            return View(user);
        }

        // GET: User/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Manually validate Email and Password
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("Email", "Email is required.");
            }
            else if (!new EmailAddressAttribute().IsValid(email))
            {
                ModelState.AddModelError("Email", "Invalid email address.");
            }

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }
            else if (password.Length < 6)
            {
                ModelState.AddModelError("Password", "Password must be at least 6 characters long.");
            }

            if (email!=null&&password!=null)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user != null && VerifyPassword(password, user.Password))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("UserId", user.UserId.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(claimsIdentity));

                    HttpContext.Session.SetInt32("UserId", user.UserId); // Set the UserId in session

                    return RedirectToAction("Index", "Home"); // Redirect to Home page after successful Login
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }

            // Log the ModelState errors
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogError(error.ErrorMessage);
                }
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            var hashOfEnteredPassword = HashPassword(enteredPassword);
            return hashOfEnteredPassword == storedPasswordHash;
        }
    }
}
