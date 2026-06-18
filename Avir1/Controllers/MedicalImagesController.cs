using Avir1.Data;
using Avir1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Avir1.Controllers
{
    [Route("users/{userId}/images")]
    [ApiController]
    public class MedicalImagesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public MedicalImagesController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // 1. GET: users/{userId}/images
        [HttpGet]
        public async Task<IActionResult> GetAll(int userId)
        {
            var images = await _context.MedicalImages
                .Where(x => x.UserID == userId)
                .ToListAsync();

            return Ok(images);
        }

        // 2. POST: users/{userId}/images (الدالة المعدلة والآمنة للاستضافة أونلاين)
        [HttpPost]
        public async Task<IActionResult> Create(int userId, [FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file uploaded.");
            }

            try
            {
                // تأمين مسار الـ wwwroot للأونلاين استضافة
                string webRootPath = _environment.WebRootPath;

                // لو الاستضافة مخلية الـ WebRootPath بـ null، بنجيب مسار فولدر المشروع الحالي كبديل
                if (string.IsNullOrEmpty(webRootPath))
                {
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                string uploadsFolder = Path.Combine(webRootPath, "uploads");

                // محاولة إنشاء الفولدر وتفادي رفض الصلاحيات
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // توليد اسم فريد وحفظ الملف
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(image.FileName);
                string fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                string relativePath = $"/uploads/{uniqueFileName}";

                // الحفظ في الـ Database
                var medicalImage = new MedicalImage
                {
                    UserID = userId,
                    FileName = uniqueFileName,
                    FilePath = relativePath,
                    DateUploaded = DateTime.Now
                };

                _context.MedicalImages.Add(medicalImage);
                await _context.SaveChangesAsync();

                return Ok(medicalImage);
            }
            catch (Exception ex)
            {
                // رجع تفاصيل الخطأ كاملة في الـ 500 عشان تلمح السبب في الـ Network Tab فوراً لو ضرب تاني
                return StatusCode(500, $"Internal server error: {ex.Message} -> Inner: {ex.InnerException?.Message} -> Stack: {ex.StackTrace}");
            }
        }

        // 3. DELETE: users/{userId}/images/{imageId}
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(int imageId)
        {
            var existing = await _context.MedicalImages.FindAsync(imageId);
            if (existing == null) return NotFound();

            try
            {
                if (!string.IsNullOrEmpty(existing.FilePath))
                {
                    string fileName = existing.FilePath.Replace("/uploads/", "");

                    string webRootPath = _environment.WebRootPath;
                    if (string.IsNullOrEmpty(webRootPath))
                    {
                        webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }

                    string fullPath = Path.Combine(webRootPath, "uploads", fileName);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Physical file delete failed: {ex.Message}");
            }

            _context.MedicalImages.Remove(existing);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}