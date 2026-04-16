using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Avir1.Data;
using Avir1.Models;

namespace Avir1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReminderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reminder/user/5
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserReminders(int userId)
        {
            var reminders = await _context.Reminder
                .Where(r => r.UserID == userId)
                .ToListAsync();

            return Ok(reminders);
        }

        // POST: api/reminder
        [HttpPost]
        public async Task<IActionResult> CreateReminder(Reminder reminder)
        {
            _context.Reminder.Add(reminder);
            await _context.SaveChangesAsync();

            return Ok(reminder);
        }

        // PUT: api/reminder/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(int id, Reminder reminder)
        {
            if (id != reminder.Id)
                return BadRequest();

            _context.Entry(reminder).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(reminder);
        }

        // DELETE: api/reminder/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(int id)
        {
            var reminder = await _context.Reminder.FindAsync(id);

            if (reminder == null)
                return NotFound();

            _context.Reminder.Remove(reminder);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}