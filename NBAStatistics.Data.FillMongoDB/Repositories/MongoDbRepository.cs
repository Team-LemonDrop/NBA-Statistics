using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBAStatistics.Data.FillMongoDB.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;

    using MongoDB.Driver;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using Models;

    //using MongDB.Driver.Demos.Models;

    internal class MongoDbRepository<T> : IRepository<T>
           where T : IEntity
    {
        private string collectionName;
        private IMongoDatabase db;

        public MongoDbRepository(IMongoDatabase db)
        {
            this.db = db;
            this.collectionName = typeof(T).Name.ToLower() + "s";
        }

        public async Task Add(T value)
        {
            var collection = db.GetCollection<BsonDocument>(this.collectionName);
            var valueAsBson = value.ToBsonDocument();
            await collection.InsertOneAsync(valueAsBson);
        }

        public async Task<IQueryable<T>> All()
        {
            var collection = db.GetCollection<BsonDocument>(this.collectionName);
            var bsonValues = await collection.Find(new BsonDocument()).ToListAsync();
            var values = bsonValues.Select(bsonValue => BsonSerializer.Deserialize<T>(bsonValue));

            return values.AsQueryable();
        }

        public async Task Delete(T obj)
        {
            await this.Delete(obj.Id);
        }

        public async Task Delete(object id)
        {
            var objectId = new ObjectId(id.ToString());
            var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);

            var collection = db.GetCollection<BsonDocument>(this.collectionName);
            await collection.DeleteOneAsync(filter);
        }
    }
}
