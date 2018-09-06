using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Blobs
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageConnectionString = ConfigurationManager.AppSettings.Get("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("objective2");
            container.CreateIfNotExists();

            // Upload a file
            //var blockBlob = container.GetBlockBlobReference("gonz_profile.png");
            //using (var fileStream = File.OpenRead(@"C:\Users\Gonzalo\Downloads\unr_gonz_180711_0500_wgqf0 (1).png"))
            //{
            //    blockBlob.UploadFromStream(fileStream);
            //}

            // Upload to Directories
            var directory1 = container.GetDirectoryReference("1stDirectory");
            var subDirectory2 = directory1.GetDirectoryReference("2ndDirectory");
            var nestedBlockBlob = subDirectory2.GetBlockBlobReference("nested.html");
            nestedBlockBlob.UploadText("I'm a blob inside directories!");

            // Access Attributes
            container.FetchAttributes();
            Console.WriteLine($"Name: {container.StorageUri.PrimaryUri}");
            Console.WriteLine($"Last Modified: {container.Properties.LastModified}");

            // Set MetaData (Drop and Re-Create so we don't get alrady exists error)
            container.Metadata.Clear();
            container.Metadata.Add("author", "gonzalo");
            container.Metadata["authoredOn"] = "Sep 6th, 2018";
            container.SetMetadata();

            // Retreive MetaData - NOTE! Not accessible via the portal!
            Console.WriteLine("/nMetaData:");
            container.FetchAttributes();
            foreach (var item in container.Metadata)
            {
                Console.WriteLine($"{item.Key} = {item.Value}");
            }

            // Copy Blob
            Console.WriteLine("Copying blob...");
            var srcBlockBlob = container.GetBlockBlobReference("gonz_profile.png");
            var destBlockBlob = container.GetBlockBlobReference("gonz_profile-copy2.png");
            //destBlockBlob.StartCopy(new Uri(srcBlockBlob.Uri.AbsoluteUri));
            destBlockBlob.StartCopyAsync(srcBlockBlob);
            Console.WriteLine("Blob copied!");
        }
    }
}
