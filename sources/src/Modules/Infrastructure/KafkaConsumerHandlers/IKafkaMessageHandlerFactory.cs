using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.KafkaConsumerHandlers
{
    public interface IKafkaMessageHandlerFactory
    {
        IKafkaMessageHandler GetKafkaMessageHandler(string topic);
    }
}
