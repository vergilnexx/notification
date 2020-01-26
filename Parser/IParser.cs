using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parser
{
    public interface IParser
    {
        public Task<IReadOnlyCollection<NotificationData>> Parse(IReadOnlyCollection<IMessage> messages);
    }
}
