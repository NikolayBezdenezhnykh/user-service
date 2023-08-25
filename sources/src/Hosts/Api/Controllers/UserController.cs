using Api.Attributes;
using Api.ViewModels;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/v{version:apiVersion}/user")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _dbContext;

        public UserController(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid userId)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return NotFound();

            var userVm = new UserVm()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Email = user.Email,
                UserId = user.UserId.ToString()
            };

            return Ok(user);
        }

        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid userId, [FromBody] UpdateUserVm userVm)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return NotFound();

            user.LastName = userVm.LastName;
            user.FirstName = userVm.FirstName;
            user.Phone = userVm.Phone;

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return NotFound();

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}