using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReader.Model.Mongo.Infrastructure.Contracts;

namespace RssReader.Model.Mongo.Infrastructure
{
    public class IndexInitializer
    {
        private readonly IMongoCollectionProvider _collectionProvider;

        public IndexInitializer(IMongoCollectionProvider mongoCollectionProvider)
        {
            _collectionProvider = mongoCollectionProvider;
        }

        public void CreateIndexes()
        {
            _collectionProvider.GetCollection<New>().EnsureIndex(new string[] { "FeedId" });
            _collectionProvider.GetCollection<New>().EnsureIndex(new string[] { "Tags" });
            _collectionProvider.GetCollection<New>().EnsureIndex(new string[] { "LastItemTimesStamp" });
        }
    }
}
