using Avir1.Data;
using Avir1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("images/{imageId}/analysis")]
[ApiController]
public class RadiologyLibraryController : ControllerBase
{
    private readonly AppDbContext _context;

    public RadiologyLibraryController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAnalysis(int imageId)
    {
        return Ok(await _context.Radiology_Libraries
            .Where(x => x.ImageID == imageId)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(int imageId, Radiology_Library model)
    {
        model.ImageID = imageId;
        _context.Radiology_Libraries.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }
}