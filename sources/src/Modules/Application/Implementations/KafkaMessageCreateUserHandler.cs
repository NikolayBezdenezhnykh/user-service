using Application.Dtos;
using Domain;
using Infrastructure;
using Infrastructure.KafkaConsumerHandlers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class KafkaMessageCreateUserHandler : IKafkaMessageHandler
    {
        private readonly UserDbContext _dbContext;

        public KafkaMessageCreateUserHandler(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleMessageAsync(string message)
        {
            var userDto = JsonConvert.DeserializeObject<CreateUserVm>(message);

            var existedUser = await _dbContext.Users.SingleOrDefaultAsync(u => u.UserId == userDto.UserId);
            if (existedUser != null)
            {
                return;
            }

            var user = new User()
            {
                Email = userDto.Email,
                UserId = userDto.UserId
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
