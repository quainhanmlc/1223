using ASM.Data;
using ASM.Models;
using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using X.PagedList;

namespace ASM_1670.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            this._db = db;
            _logger = logger;
            _userManager = userManager;
        }

        public IActionResult Index(int category, string searchTitle, int? page)
        {
            var categories = GetCategories();
            var pageNumber = page ?? 1;
            var pageSize = 8;

            var books = _db.Book.Include(b => b.Category).AsQueryable();

            if (category > 0)
            {
                books = books.Where(b => b.CategoryId == category);
            }

            if (!string.IsNullOrEmpty(searchTitle))
            {
                var searchTitleLower = searchTitle.ToLower();
                books = books.Where(b => b.Title.ToLower().Contains(searchTitleLower));
            }

            var pagedList = books.ToPagedList(pageNumber, pageSize);

            ViewBag.Categories = categories;
            ViewBag.Book = pagedList;
            ViewBag.SearchTitle = searchTitle;
            return View(pagedList);
        }


        private List<Category> GetCategories()
        {
            var categories = _db.Category.ToList();
            return categories;
        }


        public Book GetDetailProduct(int id)
        {
            var books = _db.Book.Find(id);
            return books;
        }
        //[Authorize(Roles = "Admin, Customer")]
        public async Task<IActionResult> ViewOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Order> userOrders = new List<Order>();

            if (user != null)
            {
                var userId = user.Id;
                userOrders = _db.Order
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Book)
                    .Where(o => o.UserId == userId)
                    .ToList();
            }

            return View(userOrders);
        }
        //[Authorize(Roles = "Admin, Customer")]
        public IActionResult AddCart(int id)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart == null)
            {
                var book = GetDetailProduct(id);
                List<Cart> listCart = new List<Cart>()
                {
                    new Cart
                    {
                        Product = book,
                        Quantity = 1
                    }
                };
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));
            }
            else
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                bool check = true;
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Product.Id == id)
                    {
                        dataCart[i].Quantity++;
                        check = false;
                    }
                }
                if (check)
                {
                    dataCart.Add(new Cart
                    {
                        Product = GetDetailProduct(id),
                        Quantity = 1
                    });
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
            }
            return RedirectToAction("Index");

        }
        public IActionResult UpdateCart(int id, int quantity)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                if (quantity > 0)
                {
                    for (int i = 0; i < dataCart.Count; i++)
                    {
                        if (dataCart[i].Product.Id == id)
                        {
                            dataCart[i].Quantity = quantity;
                        }
                    }
                    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                }
                return Ok(quantity);
            }
            return BadRequest();
        }

        public IActionResult DeleteCart(int id)
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Product.Id == id)
                    {
                        dataCart.RemoveAt(i);
                    }
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));

                return RedirectToAction(nameof(ListCart));
            }
            return RedirectToAction(nameof(Index));
        }
        //[Authorize(Roles = "Admin, Customer")]
        public IActionResult ListCart()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<Cart> dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);

                if (dataCart.Count > 0)
                {
                    ViewBag.carts = dataCart;
                    return View();
                }
                else
                {
                    return RedirectToAction(nameof(NotFoundCart));
                }
            }

            return RedirectToAction(nameof(NotFoundCart));
        }

        public IActionResult Detail(int id)
        {
            var book = GetDetailProduct(id);

            if (book == null)
            {
                return RedirectToAction(nameof(NotFoundBook));
            }

            return View(book);
        }
        public IActionResult NotFoundBook()
        {
            return View();
        }
        public IActionResult NotFoundCart()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult SuccessfullyOrder()
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