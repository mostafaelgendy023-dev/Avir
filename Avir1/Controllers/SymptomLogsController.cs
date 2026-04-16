using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Avir1.Data;
using Avir1.Models;

namespace Avir1.Controllers
{
    [Route("users/{userId}/symptoms")]
    [ApiController]
    public class SymptomLogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SymptomLogsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int userId)
        {
            var data = await _context.SymptomLogs
                .Where(x => x.UserID == userId)
                .ToListAsync();

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int userId, SymptomLog model)
        {
            model.UserID = userId;
            _context.SymptomLogs.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [HttpPut("{symptomId}")]
        public async Task<IActionResult> Update(int symptomId, SymptomLog model)
        {
            var existing = await _context.SymptomLogs.FindAsync(symptomId);
            if (existing == null) return NotFound();

            existing.SymptomType = model.SymptomType;
            existing.Description = model.Description;
            existing.DateLogged = model.DateLogged;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{symptomId}")]
        public async Task<IActionResult> Delete(int symptomId)
        {
            var existing = await _context.SymptomLogs.FindAsync(symptomId);
            if (existing == null) return NotFound();

            _context.SymptomLogs.Remove(existing);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}