using Avir1.Data;
using Avir1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avir1.Controllers
{
    [Route("users/{userId}/treatmentplans")]
    [ApiController]
    public class TreatmentPlansController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TreatmentPlansController(AppDbContext context)
        {
            _context = context;
        }

        // GET /users/{userId}/treatmentplans
        [HttpGet]
        public async Task<IActionResult> GetAll(int userId)
        {
            return Ok(await _context.TreatmentPlans
                .Where(x => x.UserID == userId)
                .ToListAsync());
        }

        // GET /users/{userId}/treatmentplans/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int userId, int id)
        {
            var plan = await _context.TreatmentPlans
                .FirstOrDefaultAsync(x => x.Id == id && x.UserID == userId);

            if (plan == null) return NotFound();
            return Ok(plan);
        }

        // POST /users/{userId}/treatmentplans
        [HttpPost]
        public async Task<IActionResult> Create(int userId, TreatmentPlan model)
        {
            model.UserID = userId;
            model.StartDate = DateTime.Now;

            _context.TreatmentPlans.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        // PUT /users/{userId}/treatmentplans/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TreatmentPlan model)
        {
            var existing = await _context.TreatmentPlans.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.NutritionalAdvice = model.NutritionalAdvice;
            existing.PsychologicalSupport = model.PsychologicalSupport;
            existing.EndDate = model.EndDate;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE /users/{userId}/treatmentplans/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.TreatmentPlans.FindAsync(id);
            if (existing == null) return NotFound();

            _context.TreatmentPlans.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}