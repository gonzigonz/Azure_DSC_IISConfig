using System;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using StorageAndData.Blobs;

namespace StorageAndData.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageConnectionString = Config("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            RunBlobDemo(storageAccount);
        }

        static void RunBlobDemo(CloudStorageAccount storageAccount)
        {
            string log;
            Console.WriteLine("BLOB STORAGE DEMO\n");

            // Get referrence to container object
            var container = storageAccount.GetContainerAsync(Config("ContainerName")).Result;
            Console.WriteLine($"Workgin with container '{container.Uri}'\n");
            
            // Upload File
            log = container.UploadFileAsync(Config("UploadSourceFile"), Config("UploadFileName")).Result;
            Console.WriteLine(log);

            // Copy Blob
            log = container.CopyBlobAsync(Config("UploadFileName")).Result;
            Console.WriteLine(log);

            // Upload to Directories
            log = container.UploadToDirectoriesAsync().Result;
            Console.WriteLine(log);

            // Attributes and MetaData
            log = container.AttributesAndMetaDataAsyn().Result;
            Console.WriteLine(log);

            // Stored Access Policies
            log = container.CreateSharedAccessPolicy(Config("SASPolicyName")).Result;
            Console.WriteLine(log);
        }

        static string Config(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }
    }
}
