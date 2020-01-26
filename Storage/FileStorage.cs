using Contracts;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using Storage.Data;
using FileOptions = Storage.Options.FileOptions;
using System.Linq;

namespace Storage
{
    public class FileStorage : IStorage
    {
        private readonly FileOptions _options;
        private readonly string _dataFilePath;
        private readonly string _backupDirectoryPath;
        private FileStorageData _data;

        public FileStorage(FileOptions options)
        {
            _options = options;

            var path = string.IsNullOrEmpty(_options.Path) ? Directory.GetCurrentDirectory() : _options.Path;
            _dataFilePath = $"{path}\\data.dat";
            _backupDirectoryPath = $"{path}\\backup";
        }

        public async Task<NotificationData[]> Load()
        {
            if(!File.Exists(_dataFilePath))
            {
                return new NotificationData[0];
            }

            var jsonData = await File.ReadAllTextAsync(_dataFilePath);
            _data = JsonConvert.DeserializeObject<FileStorageData>(jsonData);

            return _data.Data;
        }

        public async Task Save(NotificationData[] data)
        {
            var storageData = new FileStorageData();

            storageData.LastChangesDateTime = DateTime.Now;
            storageData.Data = data;

            await UpdateStorageData(storageData);
            var jsonData = JsonConvert.SerializeObject(_data);
         
            await File.WriteAllTextAsync(_dataFilePath, jsonData);
        }

        private async Task UpdateStorageData(FileStorageData storageData)
        {
            foreach (var item in storageData.Data)
            {
                if (_data?.Data?.Any(x => item.Subject == x.Subject && item.Start == x.Start) != true)
                {
                    _data.Data.Append(item);
                }
            }
        }

        public async Task Backup()
        {
            if(!Directory.Exists(_backupDirectoryPath))
            {
                Directory.CreateDirectory(_backupDirectoryPath);
            }
            File.Copy(_dataFilePath, $"{_backupDirectoryPath}\\data_{DateTime.Now.ToString("dd.MM.yyyy")}.dat");
        }
    }
}
