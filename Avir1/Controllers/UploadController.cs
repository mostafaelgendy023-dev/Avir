using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Avir1.Data;
using Avir1.Models;

namespace Avir1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UploadController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/upload/user/5
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserUploads(int userId)
        {
            var uploads = await _context.Upload
                .Where(u => u.UserID == userId)
                .ToListAsync();

            return Ok(uploads);
        }

        // POST: api/upload
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var folderPath = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var upload = new Upload
            {
                FileName = file.FileName,
                FilePath = filePath,
                UserID = userId
            };

            _context.Upload.Add(upload);
            await _context.SaveChangesAsync();

            return Ok(upload);
        }

        // DELETE: api/upload/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUpload(int id)
        {
            var upload = await _context.Upload.FindAsync(id);

            if (upload == null)
                return NotFound();

            if (System.IO.File.Exists(upload.FilePath))
            {
                System.IO.File.Delete(upload.FilePath);
            }

            _context.Upload.Remove(upload);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}