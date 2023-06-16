using Api.Attributes;
using Api.ViewModels;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/v{version:apiVersion}/user")]
    [ApiController]
    [ApiVersion("1.0")]
    [AllowCurrentUserAuthorization(RouteField = "login")]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _dbContext;

        public UserController(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateUserVm userVm, ApiVersion version)
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
                login = user.Login,
                version = version.ToString()
            }, null);

        }

        [HttpGet("{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string login)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Login == login);
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

        [HttpPut("{login}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string login, [FromBody] UpdateUserVm userVm)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Login == login);
            if (user == null) NotFound();

            user.LastName = userVm.LastName;
            user.FirstName = userVm.FirstName;
            user.Email = userVm.Email;
            user.Phone = userVm.Phone;

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{login}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string login)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Login == login);
            if (user == null) NotFound();

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}