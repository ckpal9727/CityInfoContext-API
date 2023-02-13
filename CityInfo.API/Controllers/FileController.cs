using CityInfo.API.DbContexts;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Net.Mime;
using System.Web;

namespace CityInfo.API.Controllers
{

    [Route("api/files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider fileExtensionContentTypeProvider;
        private readonly IFileServices fileServices;
        private readonly CityInfoContext cityInfoContext;

        public FileController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider,IFileServices fileServices,CityInfoContext cityInfoContext)
        {
            this.fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ??
                throw new System.ArgumentException(
                    nameof(fileExtensionContentTypeProvider));
            this.fileServices = fileServices ?? throw new System.ArgumentException(nameof(fileServices));
            this.cityInfoContext = cityInfoContext;
        }
        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            // look up the actual file, depending on the fileId
            //demo code 
            var pathToFile = "getting-started-with-rest-slides.pdf";

            //check wheather the file exists
            if (!System.IO.File.Exists(pathToFile))
            {
                return NotFound();
            }

            if (!fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(pathToFile);

            return File(bytes, contentType, Path.GetFileName(pathToFile));
        }

        [HttpPost("PostSingleFile")]
        public async Task<ActionResult> PostFile([FromForm] FileUpload fileUpload)
        {
            
            if (fileUpload == null)
            {
                return BadRequest();
            }
            try
            {
                await fileServices.PostFileAsync(fileUpload.FileDetails);
              
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

      

        [HttpGet("DownlaodFile/{fileId}")]
        public async Task<ActionResult> DownloadFile(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }

            try
            {


                    await fileServices.DownloadFileById(id);

                var file = cityInfoContext.FileDetails.Where(f => f.ID == id).FirstOrDefault();
                if (file == null)
                {
                    return NotFound();
                }
                if (!fileExtensionContentTypeProvider.TryGetContentType(file.ToString(), out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                return File(file.FileData,contentType, Path.GetFileName(file.ToString()));
            }
              
                
            
            catch (Exception)
            {
                throw;
            }
        }

     

        
    }
}
