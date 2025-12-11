using FluxOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Session için

namespace FluxOS.Controllers
{
    public class NewsController : Controller
    {
        private readonly FluxOsContext _context;

        public NewsController(FluxOsContext context)
        {
            _context = context;
        }

        // --- 1. HABERLERİ LİSTELE (HERKES GÖREBİLİR) ---
        public IActionResult Index()
        {
            // Haberleri tarihe göre yeniden eskiye sırala
            var haberler = _context.News.OrderByDescending(n => n.CreatedDate).ToList();

            // Admin mi kontrolü (Silme butonunu göstermek için)
            string userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.IsAdmin = (userRole == "Admin");

            return View(haberler);
        }

        // --- 2. HABER EKLEME SAYFASI (SADECE ADMIN) ---
        public IActionResult Create()
        {
            // Güvenlik: Admin değilse ana sayfaya at
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index");

            return View();
        }

        // --- 3. HABERİ KAYDET (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(News news)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Index");

            news.CreatedDate = DateTime.Now; // Tarihi otomatik at
            _context.News.Add(news);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // --- 4. HABER SİL (SADECE ADMIN) ---
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return Content("Yetkisiz İşlem");

            var haber = _context.News.Find(id);
            if (haber != null)
            {
                _context.News.Remove(haber);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}