using Avir1.Data;
using Avir1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Avir1.Controllers
{
    [Route("users/{userId}/communityposts")]
    [ApiController]
    public class CommunityPostsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommunityPostsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /users/{userId}/communityposts
        [HttpGet]
        public async Task<IActionResult> GetAll(int userId)
        {
            return Ok(await _context.CommunityPosts
                .Where(x => x.UserID == userId)
                .OrderByDescending(x => x.DatePosted)
                .ToListAsync());
        }

        // GET /communityposts — public feed for all users
        [HttpGet("/communityposts")]
        public async Task<IActionResult> GetPublicFeed()
        {
            return Ok(await _context.CommunityPosts
                .OrderByDescending(x => x.DatePosted)
                .ToListAsync());
        }

        // POST /users/{userId}/communityposts
        [HttpPost]
        public async Task<IActionResult> Create(int userId, CommunityPost model)
        {
            model.UserID = userId;
            model.DatePosted = DateTime.Now;
            model.Likes = 0;

            _context.CommunityPosts.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        // PUT /users/{userId}/communityposts/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CommunityPost model)
        {
            var existing = await _context.CommunityPosts.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = model.Title;
            existing.Content = model.Content;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // POST /users/{userId}/communityposts/{id}/like
        [HttpPost("{id}/like")]
        public async Task<IActionResult> Like(int id)
        {
            var existing = await _context.CommunityPosts.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Likes += 1;
            await _context.SaveChangesAsync();
            return Ok(new { existing.Id, existing.Likes });
        }

        // DELETE /users/{userId}/communityposts/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _context.CommunityPosts.FindAsync(id);
            if (existing == null) return NotFound();

            _context.CommunityPosts.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}