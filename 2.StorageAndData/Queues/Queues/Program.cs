using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queues
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageConnectionString = ConfigurationManager.AppSettings.Get("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var queueClient = storageAccount.CreateCloudQueueClient();

            // Add create a queue - NOTE - Must be lowercase! See https://blogs.msdn.microsoft.com/jmstall/2014/06/12/azure-storage-naming-rules/
            var myQueue = queueClient.GetQueueReference("firstqueue");
            myQueue.CreateIfNotExists();

            // Add a few messages
            var message1 = new CloudQueueMessage("This is the first messsage!");
            var message2 = new CloudQueueMessage("This is the second messsage!");
            var message3 = new CloudQueueMessage("This is the third messsage!");
            var message4 = new CloudQueueMessage("This is the fourth messsage!");
            myQueue.AddMessage(message1);
            myQueue.AddMessage(message2);
            myQueue.AddMessage(message3);
            myQueue.AddMessage(message4);

            // Peek twice
            Console.WriteLine("Going to peek 3 times...");
            Console.WriteLine(myQueue.PeekMessage().AsString);
            Console.WriteLine(myQueue.PeekMessage().AsString);
            Console.WriteLine(myQueue.PeekMessage().AsString);

            // Dequeue
            Console.WriteLine("Going to get messages...");
            Console.WriteLine(myQueue.GetMessage().AsString);
            Console.WriteLine(myQueue.GetMessage().AsString);
            Console.WriteLine(myQueue.GetMessage().AsString);
            Console.WriteLine(myQueue.GetMessage().AsString);
        }
    }
}
