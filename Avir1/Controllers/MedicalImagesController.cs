using Avir1.Data;
using Avir1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("users/{userId}/images")]
[ApiController]
public class MedicalImagesController : ControllerBase
{
    private readonly AppDbContext _context;

    public MedicalImagesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int userId)
    {
        return Ok(await _context.MedicalImages
            .Where(x => x.UserID == userId)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(int userId, MedicalImage model)
    {
        model.UserID = userId;
        _context.MedicalImages.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }

    [HttpDelete("{imageId}")]
    public async Task<IActionResult> Delete(int imageId)
    {
        var existing = await _context.MedicalImages.FindAsync(imageId);
        if (existing == null) return NotFound();

        _context.MedicalImages.Remove(existing);
        await _context.SaveChangesAsync();
        return Ok();
    }
}