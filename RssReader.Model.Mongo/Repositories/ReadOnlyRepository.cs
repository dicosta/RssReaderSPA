using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RssReader.Model.Contracts;

namespace RssReader.Model.Mongo.Repositories
{
    public class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class
    {
        protected MongoCollection<T> _collection { get; set; }

        public virtual IQueryable<T> GetAll()
        {
            return _collection.AsQueryable<T>();
        }
    }
}
