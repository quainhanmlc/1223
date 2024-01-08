using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace ASM.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var dataCart = JsonConvert.DeserializeObject<List<Cart>>(cart);
                  
                    Order order = new Order
                    {
                        UserId = user.Id,
                        OrderDate = DateTime.Now,
                };
                    _dbContext.Order.Add(order);
                    _dbContext.SaveChanges();
                    List<OrderItem> orderItems = new List<OrderItem>();
                    foreach (var cartItem in dataCart)
                    {
                        OrderItem orderItem = new OrderItem
                        {
                            ProductId = cartItem.Product.Id,
                            Quantity = cartItem.Quantity,
                            Price = cartItem.Product.Price,
                            OrderId = order.Id
                        };
                        _dbContext.OrderItems.Add(orderItem);            
                    }
                    _dbContext.SaveChanges();
                    HttpContext.Session.Remove("cart");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                return RedirectToAction("ListCart", "Products");
            }
        }
    }
}
