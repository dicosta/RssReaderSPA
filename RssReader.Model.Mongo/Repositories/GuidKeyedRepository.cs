using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Builders;
using RssReader.Model.Contracts;

namespace RssReader.Model.Mongo.Repositories
{
    public class GuidKeyedRepository<T> : Repository<T>, IGuidKeyedRepository<T> where T : class
    {
        public T GetById(Guid id)
        {
            return base._collection.FindOneByIdAs<T>(id);
        }

        public void Delete(Guid id)
        {
            base._collection.Remove(Query.EQ("_id",id));
        }
    }
}
