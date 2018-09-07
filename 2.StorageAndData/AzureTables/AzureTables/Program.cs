using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AzureTables
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageConnectionString = ConfigurationManager.AppSettings.Get("StorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient();

            // Create a table
            var table = tableClient.GetTableReference("FirstTestTable");
            table.CreateIfNotExists();

            // Add a new item
            //var newCar = new CarEntity(123, 2011, "BMW", "X1", "Black");
            //var insertNewCarOperation = TableOperation.Insert(newCar);
            //table.Execute(insertNewCarOperation);

            // Retrieve
            Console.WriteLine("Retrieving 1 car...");
            var retrieveCarOperation = TableOperation.Retrieve<CarEntity>("car", "123");
            var tableResult = table.Execute(retrieveCarOperation);
            if (tableResult.Result == null)
            {
                Console.WriteLine("Car object not found!");
            }
            else
            {
                Console.WriteLine("Car found!..");
                Console.WriteLine(JsonConvert.SerializeObject(tableResult, Formatting.Indented));
            }

            // batch/transaction insert - NOTE! Must be all of the same partition key, unique keys
            //var batchNewCarOperation = new TableBatchOperation();
            //batchNewCarOperation.Insert(new CarEntity(124, 2018, "Jeep", "Renegate", "Red"));
            //batchNewCarOperation.Insert(new CarEntity(125, 2000, "Chev", "Cavalaier", "Green"));
            //batchNewCarOperation.Insert(new CarEntity(126, 2007, "Mitsubish", "Triton", "White"));
            //table.ExecuteBatch(batchNewCarOperation);

            // Select all query
            Console.WriteLine("Query for all cars...");
            var carQuery = new TableQuery<CarEntity>();
            var cars = table.ExecuteQuery(carQuery);
            Console.WriteLine(JsonConvert.SerializeObject(cars, Formatting.Indented));

            // Select with filter query
            Console.WriteLine("Top 2 cars...");
            var filteredCarQuery = new TableQuery<CarEntity>().Take(2);
            var filteredCars = table.ExecuteQuery(filteredCarQuery);
            Console.WriteLine(JsonConvert.SerializeObject(filteredCars, Formatting.Indented));
        }
    }

    public class CarEntity : TableEntity
    {
        public CarEntity(int id, int year, string make, string model, string color)
        {
            UniqueId = id;
            Year = year;
            Make = make;
            Model = model;
            Color = color;
            PartitionKey = "car";
            RowKey = UniqueId.ToString();
        }

        public CarEntity()
        {

        }

        public int UniqueId { get; set; }
        public int Year { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
    }
}
