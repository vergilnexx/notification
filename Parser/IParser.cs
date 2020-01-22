using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parser
{
    public interface IParser
    {
        public Task<IReadOnlyCollection<VacationData>> Parse(IReadOnlyCollection<IMessage> messages);
    }
}
