using Avir1.Data;
using Avir1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("users/{userId}/labtests")]
[ApiController]
public class LabTestsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LabTestsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int userId)
    {
        return Ok(await _context.LabTests
            .Where(x => x.UserID == userId)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(int userId, LabTest model)
    {
        model.UserID = userId;
        _context.LabTests.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }

    [HttpPut("{testId}")]
    public async Task<IActionResult> Update(int testId, LabTest model)
    {
        var existing = await _context.LabTests.FindAsync(testId);
        if (existing == null) return NotFound();

        existing.Type = model.Type;
        existing.Result = model.Result;
        existing.Date = model.Date;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{testId}")]
    public async Task<IActionResult> Delete(int testId)
    {
        var existing = await _context.LabTests.FindAsync(testId);
        if (existing == null) return NotFound();

        _context.LabTests.Remove(existing);
        await _context.SaveChangesAsync();
        return Ok();
    }
}