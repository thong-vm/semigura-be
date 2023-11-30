using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using semigura.DAL;
//using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;
using System.Text.Json;

namespace semigura.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private IMyFileRepository _repository;

        public ImagesController(IMyFileRepository repo)
        {
            _repository = repo;
        }

        [HttpGet("{limit?}")]

        // GET: api/MyFiles
        public async Task<IActionResult> GetImage(int limit = 5)
        {
            var lst = await _repository.GetAll().OrderByDescending(p => p.CreatedDate)
                .Take(limit).Select(f => new MyFileDOT()
                {
                    FileName = f.FileName,
                    Data = f.Data,
                    Id = f.Id,
                    CreatedDate = f.CreatedDate,
                }).ToListAsync();
            return Ok(lst);
        }

        [HttpGet("{id?}")]

        // GET: api/MyFiles/5
        public async Task<IActionResult> GetImage(string id)
        {
            MyFile myFile = await _repository.FindAsync(id);
            if (myFile == null)
            {
                return NotFound();
            }

            return Ok(new MyFileDOT(myFile));
        }

        [HttpPost]
        public async Task<IActionResult> PostImage()
        {
            try
            {
                var myFile = new MyFile();

                await BigFileToBase64(myFile);

                _repository.Add(myFile);
                await _repository.SaveChangesAsync();

                //return StatusCode(HttpStatusCode.Created);
                //message.Data = "";
                return CreatedAtRoute(
                    "DefaultApi",
                    new { id = myFile.Id },
                    new MyFileDOT()
                    {
                        Id = myFile.Id,
                    }
                    );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private async Task BigFileToBase64(MyFile myFile)
        {
            var content = HttpContext.Request.Form.Files;
            if (content.Count == 0) { return; }

            string? id = HttpContext.Request.Form.ContainsKey("id") ? HttpContext.Request.Form["id"].ToString() : null;
            IFormFile file = content[0];
            var fileName = file.Name;
            var fileSize = file.Length;

            //await using Stream stream = file.OpenReadStream();
            var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var buffer = ms.ToArray();

            var savedPath = System.IO.Path.Combine(_repository.GetUploadPath(), fileName);
            if (System.IO.File.Exists(savedPath))
            {
                System.IO.File.Delete(savedPath);
            }

            System.IO.File.WriteAllBytes(savedPath, buffer);
            //var base64 = Convert.ToBase64String(buffer);
            {
                myFile.FileName = fileName;
                myFile.LocalPath = savedPath;
                myFile.Data = JsonSerializer.Serialize(new MyFileData
                {
                    mediaType = file.ContentType,
                    base64 = Convert.ToBase64String(buffer)
                });
            }
        }

        private class MyFileDOT
        {
            public string? Id { get; set; }
            public string? FileName { get; set; }
            public string? LocalPath { get; set; }
            public string? Data { get; set; }
            public DateTime? CreatedDate { get; set; }
            public MyFileDOT(MyFile myFile)
            {
                Id = myFile.Id;
                FileName = myFile.FileName;
                Data = myFile.Data;
                CreatedDate = myFile.CreatedDate;
            }

            public MyFileDOT()
            {
            }
        }
        private class MyFileData
        {
            public string base64 { get; set; }
            public string mediaType { get; set; }
        }

        // DELETE: api/MyFiles/5
        [HttpDelete("{id?}")]
        public async Task<IActionResult> DeleteImage(string id)
        {
            MyFile file = await _repository.FindAsync(id);
            if (file == null)
            {
                return NotFound();
            }

            _repository.Remove(file);
            await _repository.SaveChangesAsync();

            return Ok(new MyFileDOT(file));
        }


        private bool ImagesExists(string id)
        {
            return _repository.Exists(id);
        }
    }
}