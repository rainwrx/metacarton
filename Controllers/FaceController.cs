using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MetaCarton.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FaceController : ControllerBase
    {
        private static readonly IFaceClient Client = FaceService.Authenticate();
        private static readonly string ASSETS = Path.Combine(Directory.GetCurrentDirectory(), "Assets");

        private readonly ILogger<FaceController> _logger;

        public FaceController(ILogger<FaceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var detected = await FaceService.Detect(Client, @"https://localhost:5001/assets/family.jpg", FaceService.RECOGNITION_MODEL1);
           
            var files = new List<string>();
            var fileEntries = Directory.GetFiles(ASSETS);

            foreach(var file in fileEntries)
            {
                files.Add($"/assets/{Path.GetFileName(file)}");
            }

            return Ok(new { Images = files, Detected = detected });
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> DeleteAsync()
        {
            try
            {
                await FaceService.Delete(Client);
            } catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(new { Error=ex.Message });
            }

            return Ok(new { Info = "Face List Deleted" });
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> UploadAsync(List<IFormFile> files)
        {
            var size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    var filePath = Path.Combine(ASSETS, formFile.FileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, ASSETS });
        }
    }
}
