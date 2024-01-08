using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASM.Data;
using ASM.Models;
using Microsoft.AspNetCore.Authorization;
using ASM.Data;
using ASM.Models;

namespace ASM.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BooksController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            this._db = db;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _db.Book.Include(b => b.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _db.Book == null)
            {
                return NotFound();
            }

            var book = await _db.Book
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_db.Set<Category>(), "Id", "Name");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);
                Book book = new Book
                {
                    Title = model.Title,
                    Description = model.Description,
                    Author = model.Author,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    Picture = uniqueFileName,
                };

                _db.Add(book);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_db.Set<Category>(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var book = _db.Book.Find(id);

            if (book == null)
            {
                return NotFound();
            }

            var bookEdit = new Book
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author,
                Price = book.Price,
                CategoryId = book.CategoryId
            };
            ViewData["CategoryId"] = new SelectList(_db.Set<Category>(), "Id", "Name", book.CategoryId);
            return View(bookEdit);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var book = await _db.Book.FindAsync(id);

                    if (book == null)
                    {
                        return NotFound();
                    }

                    if (!string.IsNullOrEmpty(book.Picture))
                    {
                        var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, "images", book.Picture);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string uniqueFileName = UploadedFile(model);

                    book.Title = model.Title;
                    book.Description = model.Description;
                    book.Author = model.Author;
                    book.Price = model.Price;
                    book.Picture = uniqueFileName;
                    book.CategoryId = model.CategoryId;

                    await _db.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in Edit action: {ex.Message}");
                    return View(model);
                }
            }
            ViewData["CategoryId"] = new SelectList(_db.Set<Category>(), "Id", "Name", model.CategoryId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.Book == null)
            {
                return NotFound();
            }

            var book = await _db.Book
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var books = await _db.Book.FindAsync(id);
                if (books == null)
                {
                    return NotFound();
                }

                // Delete the profile picture file if it exists
                if (!string.IsNullOrEmpty(books.Picture))
                {
                    var imagePath = Path.Combine(webHostEnvironment.WebRootPath, "images", books.Picture);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _db.Book.Remove(books);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteConfirmed action: {ex.Message}");
                TempData["ErrorMessage"] = "Error deleting product.";
                return RedirectToAction(nameof(Index));
            }
        }

        private string UploadedFile(Book model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
