using Contracts;
using System;

namespace Storage.Data
{
    /// <summary>
    /// Данные для хранения.
    /// </summary>
    public class FileStorageData
    {
        public DateTime LastChangesDateTime { get; set; }

        public NotificationData[] Data { get; set; }
    }
}
