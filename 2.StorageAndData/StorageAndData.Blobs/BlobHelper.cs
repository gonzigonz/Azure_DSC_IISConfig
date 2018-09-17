using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;

namespace StorageAndData.Blobs
{
    public static class BlobHelper
    {
        public static async Task<CloudBlobContainer> GetContainerAsync(this CloudStorageAccount storageAccount, string containerName)
        {
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            return container;
        }
        public static async Task<string> UploadFileAsync(this CloudBlobContainer container, string sourceFile, string blobName)
        {
            var log = new StringBuilder();
            log.AppendLine($"Uploading '{sourceFile}' ...");

            var blockBlob = container.GetBlockBlobReference(blobName);
            using (var fileStream = File.OpenRead(sourceFile))
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            log.AppendLine($"File uploaded to {container.StorageUri.PrimaryUri}/{blobName}");
            return log.ToString();
        }
        public static async Task<string> CreateSharedAccessPolicy(this CloudBlobContainer container, string policyName)
        {
            var log = new StringBuilder();
            log.AppendLine($"Creating new Stored Access Policy...");

            // Create a new stored access policy and define its constraints.
            var sharedPolicy = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Read
            };

            // Get the container's existing permissions
            var permissions = new BlobContainerPermissions();

            // Add the new policcy to the container's permissions.
            permissions.SharedAccessPolicies.Clear();
            permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
            await container.SetPermissionsAsync(permissions);
            log.AppendLine($"Policy '{policyName}' created for {container.StorageUri.PrimaryUri}!");

            // Generate a Shared access signature (SAS) the user can use
            var sas = container.GetSharedAccessSignature(sharedPolicy);
            log.AppendLine($"Access with SAS token:\n{sas}");
            return log.ToString();
        }
        public static async Task<string> UploadToDirectoriesAsync(this CloudBlobContainer container)
        {
            var log = new StringBuilder();
            log.AppendLine("Uploading a file with nesting directories...");

            var directory1 = container.GetDirectoryReference("1stDirectory");
            var subDirectory2 = directory1.GetDirectoryReference("2ndDirectory");
            var nestedBlockBlob = subDirectory2.GetBlockBlobReference("nested.html");
            await nestedBlockBlob.UploadTextAsync("I'm a blob inside directories!");
            log.AppendLine($"File created at {nestedBlockBlob.Uri}");

            return log.ToString();
        }
        public static async Task<string> AttributesAndMetaDataAsyn(this CloudBlobContainer container)
        {
            var log = new StringBuilder();
            log.AppendLine("Fetching container attributes...");

            // Access Attributes
            await container.FetchAttributesAsync();
            log.AppendLine($"Name: {container.StorageUri.PrimaryUri}");
            log.AppendLine($"Last Modified: {container.Properties.LastModified}");

            // Set MetaData (Drop and Re-Create so we don't get alrady exists error)
            container.Metadata.Clear();
            container.Metadata.Add("author", "gonzalo");
            container.Metadata["authoredOn"] = "Sep 6th, 2018";
            await container.SetMetadataAsync();

            // Retreive MetaData - NOTE! Not accessible via the portal!
            log.AppendLine("Settings MetaData...");
            await container.FetchAttributesAsync();
            foreach (var item in container.Metadata)
            {
                log.AppendLine($"{item.Key} = {item.Value}");
            }

            return log.ToString();
        }
        public static async Task<string> CopyBlobAsync(this CloudBlobContainer container, string blobName)
        {
            var log = new StringBuilder();
            log.AppendLine("Copying a blob...");

            var srcBlockBlob = container.GetBlockBlobReference(blobName);
            var destBlockBlob = container.GetBlockBlobReference($"{Path.GetFileNameWithoutExtension(blobName)}-copy{Path.GetExtension(blobName)}");
            await destBlockBlob.StartCopyAsync(srcBlockBlob);
            // Commented below is an alternative
            //destBlockBlob.StartCopy(new Uri(srcBlockBlob.Uri.AbsoluteUri));
            log.AppendLine($"Blob '{srcBlockBlob.Uri}' copied to '{destBlockBlob.Uri}'");

            return log.ToString();
        }
        public static async Task<string> CreateCORSPolicy(CloudBlobContainer container)
        {
            var log = new StringBuilder();
            log.AppendLine("Setting Storage CORS rule...");

            var serviceProperties = new ServiceProperties();
            serviceProperties.Cors.CorsRules.Add(new CorsRule
            {
                AllowedMethods = CorsHttpMethods.Get,
                AllowedOrigins = new string[] { "http://localhost:8088" },
                MaxAgeInSeconds = 3600
            });
            await container.ServiceClient.SetServicePropertiesAsync(serviceProperties);
            log.AppendLine("New CORS rule added!");

            return log.ToString();
        }
    }
}
