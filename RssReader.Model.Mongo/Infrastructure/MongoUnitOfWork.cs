using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using RssReader.Model.Contracts;
using RssReader.Model.Mongo.Infrastructure.Contracts;

namespace RssReader.Model.Mongo.Infrastructure
{
    public class MongoUnitOfWork : IUnitOfWork
    {
        private readonly IMongoCollectionProvider _collectionProvider;

        public MongoUnitOfWork(IMongoCollectionProvider collectionProvider)
        {
            _collectionProvider = collectionProvider;
        }

        public MongoCollection<T> GetMongoCollection<T>()
        {
            return _collectionProvider.GetCollection<T>();
        }

        public void Commit()
        {
            //nothing
        }

        public void Rollback()
        {
            //nothing
        }
    }
}
