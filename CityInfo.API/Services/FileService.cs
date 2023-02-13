using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;

namespace CityInfo.API.Services
{
    public class FileService : IFileServices
    {
        private readonly CityInfoContext dbContexts;
        private readonly FileExtensionContentTypeProvider fileExtensionContentTypeProvider;

        public FileService(CityInfoContext dbContexts,FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            this.dbContexts = dbContexts;
            this.fileExtensionContentTypeProvider = fileExtensionContentTypeProvider;
        }

       

        public async Task PostFileAsync(IFormFile fileData)
        {
            try
            {
                var fileDetails = new FileDetails()
                {
                    ID=0,
                    FileName = fileData.FileName,
                   
                    

                };
                using (var stream = new MemoryStream())
                {
                    fileData.CopyTo(stream);
                    fileDetails.FileData = stream.ToArray();
                }
                var result = dbContexts.FileDetails.Add(fileDetails);
                await dbContexts.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                throw;  
            }
        }

        
        public async Task DownloadFileById(int Id)
        {
            try
            {
                var file = dbContexts.FileDetails.Where(x => x.ID == Id).FirstOrDefault();
                var content = new System.IO.MemoryStream(file.FileData);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "File Download", file.FileName);
                await CopyStream(content, path);
                
                 
                
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task CopyStream(Stream stream, string downloadPath)
        {
            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write))
            {
                var bytes = System.IO.File.ReadAllBytes(downloadPath);
                await stream.CopyToAsync(fileStream);
            }
        }

       /* public async Task<byte> GetFileById(int Id)
        {
            var file = dbContexts.FileDetails.Where(x => x.ID == Id).FirstOrDefault();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "File Download", file.FileName);
            if (!fileExtensionContentTypeProvider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(path);

            return ;
        }*/
    }
}
