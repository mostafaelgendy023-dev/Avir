using Avir1.Data;
using Avir1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("users/{userId}/results")]
[ApiController]
public class ResultSummariesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ResultSummariesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int userId)
    {
        return Ok(await _context.ResultSummaries
            .Where(x => x.UserID == userId)
            .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(int userId, ResultSummary model)
    {
        model.UserID = userId;
        _context.ResultSummaries.Add(model);
        await _context.SaveChangesAsync();
        return Ok(model);
    }
}