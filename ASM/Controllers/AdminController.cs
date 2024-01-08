using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASM.Data;

namespace ASM.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _context = db;
        }

        // Example action to update user phone number

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult User()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }
        public IActionResult Order()
        {
            var orders = _context.Order.ToList();
            return View(orders);
        }

    }

}
