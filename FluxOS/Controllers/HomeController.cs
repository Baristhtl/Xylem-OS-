using FluxOS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FluxOS.Controllers
{
    public class HomeController : Controller
    {
        private readonly FluxOsContext _context;

        public HomeController(FluxOsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Forum()
        {
            var konular = _context.Topics
                .Include(t => t.User)
                .Include(t => t.Category)
                .Include(t => t.Replies) // <-- BU SATIRI EKLE (Yanıtları da pakete dahil et)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();

            var kategoriler = _context.Categories
                .Include(c=> c.Topics) // Kategorilerin konularını da dahil et)
                .ToList();

            // Bu kategori listesini "ViewBag" (sanal çanta) ile sayfaya taşıyoruz
            ViewBag.KategoriListesi = kategoriler;

            return View(konular);
        }
        // --- YENİ KONU AÇ (SAYFAYI GÖSTER) ---
        public IActionResult NewTopic()
        {
            // Giriş kontrolü
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Kategoriler = _context.Categories.ToList();
            return View();
        }
        // --- YENİ KONU KAYDET (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewTopic(string title, string content, int categoryId)
        {
            // 1. Session'dan Giriş Yapanın ID'sini Al
            int? userId = HttpContext.Session.GetInt32("UserId");

            // Eğer giriş yapmamışsa Login'e at (Güvenlik)
            if (userId == null) return RedirectToAction("Login", "Account");

            var yeniKonu = new Topic
            {
                Title = title,
                Content = content,
                CategoryId = categoryId,

                // --- İŞTE KRİTİK NOKTA BURASI ---
                // Burası eskiden "UserId = 2" idi. Bunu düzeltmen lazım:
                UserId = userId.Value,
                // --------------------------------

                CreatedDate = DateTime.Now,
                ViewCount = 0,
                IsSolved = false
            };

            _context.Topics.Add(yeniKonu);
            _context.SaveChanges();

            return RedirectToAction("Forum");
        }
        // --- DETAY SAYFASI (YETKİ İÇİN) ---
        public IActionResult TopicDetails(int id)
        {
            // Veritabanından seçilen konuyu (id'ye göre) bul
            // Ayrıca konuyu yazan kişiyi, kategorisini ve ALTINDAKİ YORUMLARI da getir.
            var konu = _context.Topics
                .Include(t => t.User)           // Konu sahibini getir
                .Include(t => t.Category)       // Kategoriyi getir
                .Include(t => t.Replies)        // Konuya yapılan yorumları getir
                    .ThenInclude(r => r.User)   // Yorumu yapan kişileri de getir
                .FirstOrDefault(t => t.TopicId == id);

            if (konu == null)
            {
                return NotFound(); // Konu yoksa hata ver
            }

            // Görüntülenme sayısını 1 artır ve kaydet
            konu.ViewCount++;
            _context.SaveChanges();

            ViewBag.AktifId = HttpContext.Session.GetInt32("UserId");
            ViewBag.AktifRol = HttpContext.Session.GetString("UserRole");

            return View(konu);
        }

        // --- YORUM EKLEME (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReply(string content, int topicId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var yeniYorum = new Reply
            {
                Content = content,
                TopicId = topicId,
                UserId = userId.Value, // Gerçek kullanıcı ID'si
                CreatedDate = DateTime.Now
            };
            _context.Replies.Add(yeniYorum);
            _context.SaveChanges();
            return RedirectToAction("TopicDetails", new { id = topicId });
        }

        // --- SİLME ---
        public IActionResult DeleteTopic(int id)
        {
            // 1. Silinecek konuyu bul
            var konu = _context.Topics.Find(id);
            if (konu == null) return NotFound();

            // 2. Kim silmeye çalışıyor?
            int? userId = HttpContext.Session.GetInt32("UserId");
            string userRole = HttpContext.Session.GetString("UserRole");

            // 3. Yetki Kontrolü: Giriş yapmış mı? VE (Sahibi mi VEYA Admin mi?)
            if (userId != null && (konu.UserId == userId || (userRole != null && userRole.ToLower() == "admin")))
            {
                _context.Topics.Remove(konu);
                _context.SaveChanges();
                return RedirectToAction("Forum");
            }
            else
            {
                return Content("HATA: Bu konuyu silmeye yetkiniz yok!");
            }
        }
        // --- KONU DÜZENLEME SAYFASINI GÖSTER (GET) ---
        public IActionResult EditTopic(int id)
        {
            var konu = _context.Topics.Find(id);
            if (konu == null) return NotFound();

            // YETKİ KONTROLÜ: Sadece Sahibi veya Admin girebilir
            int? userId = HttpContext.Session.GetInt32("UserId");
            string userRole = HttpContext.Session.GetString("UserRole");

            // Admin değilse VE Kendi konusu değilse -> Hata ver
            if (userRole != "Admin" && (userId == null || konu.UserId != userId))
            {
                return Content("HATA: Bu konuyu düzenlemeye yetkiniz yok!");
            }

            // Kategorileri de gönderelim ki değiştirebilsin
            ViewBag.Kategoriler = _context.Categories.ToList();

            return View(konu);
        }

        // --- DEĞİŞİKLİKLERİ KAYDET (POST) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditTopic(int id, string title, string content, int categoryId)
        {
            var konu = _context.Topics.Find(id);
            if (konu == null) return NotFound();

            // YETKİ KONTROLÜ (Tekrar kontrol ediyoruz, güvenlik şart)
            int? userId = HttpContext.Session.GetInt32("UserId");
            string userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != "Admin" && (userId == null || konu.UserId != userId))
            {
                return RedirectToAction("Forum"); // Yetkisizse foruma at
            }

            // Verileri Güncelle
            konu.Title = title;
            konu.Content = content;
            konu.CategoryId = categoryId;

            // Veritabanına "Değişiklik var, kaydet" diyoruz
            _context.Topics.Update(konu);
            _context.SaveChanges();

            // Detay sayfasına geri dön
            return RedirectToAction("TopicDetails", new { id = konu.TopicId });
        }

        // --- İNDİRME SAYFASI ---
        public IActionResult Download()
        {
            return View();
        }
        // --- GÜVENLİ VE PERFORMANSLI İNDİRME ---
        public IActionResult DownloadIso()
        {
            string dosyaYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "downloads", "XylemOS-v2025.12.iso.txt");

            if (!System.IO.File.Exists(dosyaYolu)) return Content("Dosya Yok!");

            // FileShare.Read: "Ben okurken başkası da (OneDrive) okuyabilir" izni verir. Çakışmayı önler.
            var fileStream = new FileStream(dosyaYolu, FileMode.Open, FileAccess.Read, FileShare.Read);

            return File(fileStream, "application/octet-stream", "XylemOS-v2025.12.iso");
        }
        // --- WIKI / KURULUM REHBERİ ---
        public IActionResult Wiki()
        {
            return View();
        }
    }
}