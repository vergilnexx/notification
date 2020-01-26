using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reader
{
    public interface IReader
    {
        Task<IReadOnlyCollection<NotificationData>> Get();
    }
}
