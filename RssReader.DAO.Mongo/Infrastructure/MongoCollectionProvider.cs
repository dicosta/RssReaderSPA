using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using RssReader.DAO.Mongo.Infrastructure.Contracts;

namespace RssReader.DAO.Mongo.Infrastructure
{
    public class MongoCollectionProvider : IMongoCollectionProvider
    {
        private readonly string _connectionString;

        private readonly MongoDatabase _mongoDB;

        public MongoCollectionProvider(string connectionString)
		{
			_connectionString = connectionString;

            //lazy init?
            var client = new MongoClient(_connectionString);
            var server = client.GetServer();
            _mongoDB = server.GetDatabase("readerRss");
		}

        public MongoCollection<T> GetCollection<T>()
        {
            return _mongoDB.GetCollection<T>(typeof(T).FullName);
        }
    }
}
