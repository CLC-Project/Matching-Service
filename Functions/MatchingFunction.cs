using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using MongoDB.Bson;
using System.Linq;
using MatchingService.Entities;
using MatchingService.Dtos;

namespace MatchingService.Functions
{
    public static class MatchingFunction
    {
        private const string _databaseName = "CLC-Project";
        private const string _collectionName = "matchings";
        private const string _dbConnection = "MongoDBAtlasConnection";

        #region Util

        private static IMongoCollection<Matching> GetCollection()
        {
            var settings = MongoClientSettings.FromConnectionString(Environment.GetEnvironmentVariable(_dbConnection));
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            var database = client.GetDatabase(_databaseName);
            var collection = database.GetCollection<Matching>(_collectionName);
            return collection;
        }

        #endregion

        #region Get

        [FunctionName("Matchings_GetById")]
        public static async Task<IActionResult> GetById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "matchings/{id}")] HttpRequest req,
                                                         string id,
                                                         ILogger log)
        {
            log.LogInformation($"{DateTime.Now}: get matching by id");

            var collection = GetCollection();
            var entry = await collection.FindAsync(x => x.Id == ObjectId.Parse(id));
            var result = await entry.FirstOrDefaultAsync();

            return new OkObjectResult(result is null ? result : new MatchingDto(result));

        }

        [FunctionName("Matchings_GetByUserId")]
        public static async Task<IActionResult> GetByUserId([HttpTrigger(AuthorizationLevel.Function, "get", Route = "matchings/user/{userId}")] HttpRequest req,
                                                             string userId,
                                                             ILogger log)
        {
            log.LogInformation($"{DateTime.Now}: get matchings by user id");

            var collection = GetCollection();
            var entries = await collection.FindAsync(x => x.Destination.User == userId);
            var result = await entries.ToListAsync();

            return new OkObjectResult(result.Select(x => new MatchingDto(x)));
        }

        [FunctionName("Matchings_GetByDestinationId")]
        public static async Task<IActionResult> GetByDestinationId([HttpTrigger(AuthorizationLevel.Function, 
                                                                                "get", 
                                                                                Route = "matchings/destination/{destinationId}")] HttpRequest req,
                                                                    string destinationId,
                                                                    ILogger log)
        {
            log.LogInformation($"{DateTime.Now}: get matchings by destination id");

            var collection = GetCollection();
            var entries = await collection.FindAsync(x => x.Destination.Id == ObjectId.Parse(destinationId));
            var result = await entries.FirstOrDefaultAsync();

            return new OkObjectResult(result is null ? result : new MatchingDto(result));
        }

        #endregion

    }
}
