using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace RssReader.Model.Mongo.Infrastructure.Contracts
{
    public interface IMongoCollectionProvider
    {
        MongoCollection<T> GetCollection<T>();
    }
}
