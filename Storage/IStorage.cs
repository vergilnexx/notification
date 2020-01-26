using Contracts;
using System.Threading.Tasks;

namespace Storage
{
    public interface IStorage
    {
        Task Save(NotificationData[] data);

        Task Backup();

        Task<NotificationData[]> Load();
    }
}
