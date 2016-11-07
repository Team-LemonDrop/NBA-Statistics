using MongoDB.Bson;
using MongoDB.Driver;
using NBAStatistics.Models;
using NBAStatistics.Data.FillMongoDB.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBAStatistics.Data.FillMongoDB
{
    public class FillMongoDB
    {
        private const string User = "miro";
        private const string Pass = "1qazcde3";
        private const string DbHost = "ds029565.mlab.com";
        private const int DbPort = 29565;
        private const string DbName = "appharbor_5cwg75nh";

        public async static Task FillDatabase(IEnumerable<BsonDocument> bsonDocument, string collectionName)
        {
            await Task.Run(() =>
            {
                var credentials = MongoCredential.CreateCredential(DbName, User, Pass);
                var settings = new MongoClientSettings
                {
                    Server = new MongoServerAddress(DbHost, DbPort),
                    Credentials = new List<MongoCredential> { credentials }
                };

                var client = new MongoClient(settings);
                var db = client.GetDatabase(DbName);

                if (bsonDocument.Count() > 0)
                {
                    var collection = db.GetCollection<BsonDocument>(collectionName);
                    collection.InsertMany(bsonDocument);
                }
            });
        }
    }
}
