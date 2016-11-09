using System.Collections.Generic;
using System.Linq;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

using NBAStatistics.Data.FillMongoDB.Models;
using NBAStatistics.Data.Repositories.Contracts;

namespace NBAStatistics.Data.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly IMongoDatabase db;
        private readonly string collectionName;

        public MongoRepository(IMongoDatabase db)
        {
            this.db = db;
            this.collectionName = typeof(T).Name.ToLower() + "s";
        }

        public void Add(T value)
        {
            var collection = db.GetCollection<BsonDocument>(this.collectionName);
            var valueAsBson = value.ToBsonDocument();
            collection.InsertOne(valueAsBson);
        }

        public void Delete(T value)
        {
            var objectId = new ObjectId(value.Id);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            var collection = db.GetCollection<BsonDocument>(this.collectionName);
            collection.DeleteOne(filter);
        }

        public IEnumerable<T> GetAll()
        {
            var collection = db.GetCollection<BsonDocument>(this.collectionName);
            var bsonValues = collection.Find(new BsonDocument()).ToList();
            var values = bsonValues.Select(bsonValue => BsonSerializer.Deserialize<T>(bsonValue));

            return values;
        }
    }
}
