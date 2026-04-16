using Avir1.Data;
using Avir1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("users/{userId}/selfexams")]
[ApiController]
public class SelfExamsController : ControllerBase
{
    private readonly AppDbContext _context;

    public SelfExamsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int userId)
    {
        return Ok(await _context.SelfExams
            .Where(x => x.UserID == userId)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(int userId, SelfExam model)
    {
        model.UserID = userId;
        _context.SelfExams.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }

    [HttpPut("{examId}")]
    public async Task<IActionResult> Update(int examId, SelfExam model)
    {
        var existing = await _context.SelfExams.FindAsync(examId);
        if (existing == null) return NotFound();

        existing.Result = model.Result;
        existing.DatePerformed = model.DatePerformed;
        existing.Notes = model.Notes;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{examId}")]
    public async Task<IActionResult> Delete(int examId)
    {
        var existing = await _context.SelfExams.FindAsync(examId);
        if (existing == null) return NotFound();

        _context.SelfExams.Remove(existing);
        await _context.SaveChangesAsync();
        return Ok();
    }
}