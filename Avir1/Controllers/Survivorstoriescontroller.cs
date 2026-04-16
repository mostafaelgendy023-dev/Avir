using Avir1.Data;
using Avir1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avir1.Controllers
{
    [Route("users/{userId}/stories")]
    [ApiController]
    public class SurvivorStoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SurvivorStoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET /users/{userId}/stories
        [HttpGet]
        public async Task<IActionResult> GetAll(int userId)
        {
            return Ok(await _context.SurvivorStories
                .Where(x => x.UserID == userId)
                .ToListAsync());
        }

        // GET /stories/approved — all approved stories (public feed)
        [HttpGet("/stories/approved")]
        public async Task<IActionResult> GetApproved()
        {
            return Ok(await _context.SurvivorStories
                .Where(x => x.IsApproved)
                .OrderByDescending(x => x.DatePosted)
                .ToListAsync());
        }

        // POST /users/{userId}/stories
        [HttpPost]
        public async Task<IActionResult> Create(int userId, SurvivorStory model)
        {
            model.UserID = userId;
            model.DatePosted = DateTime.Now;
            model.IsApproved = false; // requires admin approval

            _context.SurvivorStories.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        // PUT /users/{userId}/stories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, SurvivorStory model)
        {
            var existing = await _context.SurvivorStories.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = model.Title;
            existing.Content = model.Content;
            existing.VideoUrl = model.VideoUrl;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE /users/{userId}/stories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.SurvivorStories.FindAsync(id);
            if (existing == null) return NotFound();

            _context.SurvivorStories.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}