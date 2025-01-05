using IOTWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IOTWebAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOTDbContext _context;

        public HomeController(IOTDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == username);
            if (user != null)
            {
                if (user.Password == password)
                {
                    return RedirectToAction("Index","Masa", new { userId = user.UserId });
                }
                else
                {
                    TempData["msg"] = "Hatalı şifre";
                }
            }
            else
            {
                TempData["msg"] = "Kullanici bulunamadi";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}