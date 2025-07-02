namespace telerik.Services
{
    public interface IBookService
    {
        Task <string> UploadCoverImageAsync(IFormFile file);
        void RemoveFiles(string[] fileNames);
    }
}
