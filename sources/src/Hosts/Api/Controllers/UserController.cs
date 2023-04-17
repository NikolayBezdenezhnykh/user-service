using Api.ViewModels;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _dbContext;

        public UserController(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(CreateUserVm userVm)
        {
            var user = new User()
            {
                LastName = userVm.LastName,
                FirstName = userVm.FirstName,
                Login = userVm.Login,
                Email = userVm.Email,
                Phone = userVm.Phone
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("Get", new
            {
                userId = user.Id,
            }, null);

        }

        [HttpGet("{userId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(long userId)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) NotFound();

            var userVm = new UserVm()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Login = user.Login,
                Email = user.Email,
                Id = user.Id
            };

            return Ok(user);
        }

        [HttpPut("{userId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long userId, [FromBody] UpdateUserVm userVm)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) NotFound();

            user.LastName = userVm.LastName;
            user.FirstName = userVm.FirstName;
            user.Email = userVm.Email;
            user.Phone = userVm.Phone;

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{userId:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(long userId)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null) NotFound();

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}