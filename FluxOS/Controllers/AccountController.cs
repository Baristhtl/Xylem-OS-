using FluxOS.Models;
using Microsoft.AspNetCore.Mvc;

namespace FluxOS.Controllers
{
    public class AccountController : Controller
    {
        private readonly FluxOsContext _context;

        public AccountController(FluxOsContext context)
        {
            _context = context;
        }

        // --- KAYIT OL SAYFASI ---
        public IActionResult Register()
        {
            return View();
        }

        // --- KAYIT İŞLEMİ (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string username, string email, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Hata = "Bu e-posta adresi zaten kullanılıyor!";
                return View();
            }

            var newUser = new User
            {
                Username = username,
                Email = email,
                Password = password,
                Role = "User",
                AvatarColor = "#6c63ff",
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // --- GİRİŞ YAP SAYFASI ---
        public IActionResult Login()
        {
            return View();
        }

        // --- GİRİŞ İŞLEMİ (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // Session'a bilgileri yaz
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Hata = "E-posta veya şifre hatalı!";
                return View();
            }
        }

        // --- ÇIKIŞ YAP ---
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}