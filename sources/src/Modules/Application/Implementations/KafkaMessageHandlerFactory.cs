﻿using Infrastructure.KafkaConsumerHandlers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class KafkaMessageHandlerFactory : IKafkaMessageHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public KafkaMessageHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IKafkaMessageHandler GetKafkaMessageHandler(string topic)
        {
            return topic switch
            {
                "user_registered" => _serviceProvider.GetRequiredService<KafkaMessageCreateUserHandler>(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
