using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using telerik.Models;

namespace telerik.Services
{
    public interface IBookService
    {
        List<Book> GetAllBooks();
        List<BookCategory> GetAllCategories();
        Task<string> UploadCoverImageAsync(IFormFile file);
        void RemoveFiles(string[] fileNames);
        void CreateBook(Book book);
        void UpdateBook(Book book);
        void CreateCategory(BookCategory category);
        void UpdateCategory(BookCategory category);
        void DeleteBook(int bookId);
        void DeleteCategory(BookCategory category);
        void SubmitOrder(Order order);
        List<Order> GetAllOrders();


    }
}
