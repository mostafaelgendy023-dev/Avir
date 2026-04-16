using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Avir1.Models;

namespace Avir1.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;

        public UsersController(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }

        // GET /users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST /users
        [HttpPost]
        public async Task<IActionResult> CreateUser(Users user)
        {
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));
            return Ok(user);
        }

        // PUT /users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, Users updatedUser)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.UserName = updatedUser.Email;
            user.Gender = updatedUser.Gender;
            user.DOB = updatedUser.DOB;
            user.FamilyHistory = updatedUser.FamilyHistory;
            user.BMI = updatedUser.BMI;
            user.ActivityLevel = updatedUser.ActivityLevel;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(user);
        }

        // DELETE /users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok();
        }
    }
}