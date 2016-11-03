using MongoDB.Bson;
using MongoDB.Driver;
using NBAStatistics.Data.FillMongoDB.Models;
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

        public async static Task FillPlayersCollection(IEnumerable<BsonDocument> players)
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
                var playersCollection = db.GetCollection<BsonDocument>("Players");

                playersCollection.InsertMany(players);
            });
        }
    }
}
