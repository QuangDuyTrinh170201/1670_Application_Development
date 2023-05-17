using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _1670_Source_Duy.Data;
using _1670_Source_Duy.Models;
using Microsoft.Extensions.Hosting;

namespace _1670_Source_Duy.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;


        public BooksController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Book.Include(b => b.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Category)
                .Include(b => b.Category)
                .Include(b => b.Comments) // Bao gồm bảng Comment
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            var comments = await _context.Comment
            .Where(c => c.BookId == id)
            .Select(c => new Comment
            {
                Content = c.Content,
                Date = c.Date
            })
            .ToListAsync();

            // Gán danh sách comment vào book
            book.Comments = comments;

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Category, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,BookTitle,Description,Price,Image,CategoryId,Author")] Book book, IFormFile Image)
        {
            if (ModelState.IsValid == false)
            {
                if (Image != null)
                {
                    // Lưu ảnh vào thư mục
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                    string extension = Path.GetExtension(Image.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    book.Image = fileName;

                    string path = Path.Combine(wwwRootPath + "/img/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", book.CategoryId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = new SelectList(_context.Category, "CategoryId", "CategoryName", book.CategoryId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,BookTitle,Description,Price,Image,CategoryId,Author")] Book book, IFormFile Image)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try 
                {
                    if(Image != null && Image.Length > 0)
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(Image.FileName);
                        string extension = Path.GetExtension(book.Image);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/img/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await Image.CopyToAsync(fileStream);
                        }

                        string oldImagePath = book.Image;
                        if (!string.IsNullOrEmpty(oldImagePath))
                        {
                            // Xóa ảnh cũ
                            string oldImagePathFullPath = Path.Combine(wwwRootPath + "/img/", oldImagePath);
                            if (System.IO.File.Exists(oldImagePathFullPath))
                            {
                                System.IO.File.Delete(oldImagePathFullPath);
                            }
                        }

                        book.Image = fileName;
                    }
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryId", book.CategoryId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);
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
            if (_context.Book == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool BookExists(int id)
        {
          return (_context.Book?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
    }
}
