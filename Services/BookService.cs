
namespace telerik.Services
{
    public class BookService : IBookService
    {
        private readonly IWebHostEnvironment _env;
        public BookService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<string> UploadCoverImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/" + fileName;
        }
        public void RemoveFiles(string[] fileNames)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            foreach (var name in fileNames)
            {
                var filePath = Path.Combine(uploadsFolder, Path.GetFileName(name));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

    }
}
