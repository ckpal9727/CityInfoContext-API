using CityInfo.API.Models;
using Microsoft.VisualBasic.FileIO;

namespace CityInfo.API.Services
{
    public interface IFileServices
    {
      
        public Task PostFileAsync(IFormFile fileData);

       // public Task PostMultiFileAsync(List<FileUpload> fileData);

        public Task DownloadFileById(int fileName);

        //public Task GetFileById(int fileId);

    }
}
