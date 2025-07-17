using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using telerik.Data;
using telerik.Models;

namespace telerik.Services
{
    public class BookService : IBookService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _context;

        public BookService(IWebHostEnvironment env, ApplicationDbContext context)
        {
            _env = env;
            _context = context;
        }

        public List<Book> GetAllBooks()
        {
            var books = _context.Book.ToList();
            Console.WriteLine($"Retrieved books: {Newtonsoft.Json.JsonConvert.SerializeObject(books)}");
            return books;
        }

        public List<BookCategory> GetAllCategories() => _context.BookCategories.ToList();

        public async Task<string> UploadCoverImageAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/uploads/{fileName}";
            }
            return null; // Return null if no file is uploaded
        }

        public void RemoveFiles(string[] fileNames) { }

        public void CreateBook(Book book)
        {
            if (book != null)
            {
                Console.WriteLine($"Adding book to context: {Newtonsoft.Json.JsonConvert.SerializeObject(book)}");
                _context.Book.Add(book);
                try
                {
                    _context.SaveChanges();
                    Console.WriteLine($"SaveChanges completed for book Id: {book.Id}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"SaveChanges error: {ex.Message}");
                }
            }
        }

        public void UpdateBook(Book book)
        {
            var existing = _context.Book.FirstOrDefault(b => b.Id == book.Id);
            if (existing != null)
            {
                existing.Title = book.Title;
                existing.Author = book.Author;
                existing.Genre = book.Genre;
                existing.Price = book.Price;
                existing.Stock = book.Stock;
                existing.CoverImage = book.CoverImage;
                _context.SaveChanges();
            }
        }

        public void DeleteBook(int id)
        {
            var book = _context.Book.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                _context.Book.Remove(book);
                _context.SaveChanges();
            }
        }

        public void CreateCategory(BookCategory category)
        {
            _context.BookCategories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(BookCategory category)
        {
            var existing = _context.BookCategories.FirstOrDefault(c => c.Id == category.Id);
            if (existing != null)
            {
                existing.Name = category.Name;
                existing.ParentId = category.ParentId;
                _context.SaveChanges();
            }
        }

        public void DeleteCategory(BookCategory category)
        {
            var existing = _context.BookCategories.FirstOrDefault(c => c.Id == category.Id);
            if (existing != null)
            {
                _context.BookCategories.Remove(existing);
                _context.SaveChanges();
            }
        }
        public void SubmitOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders.Include(o => o.Book).ToList();
        }

    }
}
