using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Builders;
using RssReader.Model.Contracts;

namespace RssReader.DAO.Mongo.Repositories
{
    public class GuidKeyedRepository<T> : Repository<T>, IGuidKeyedRepository<T> where T : class
    {
        public GuidKeyedRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public T GetById(Guid id)
        {
            return base.Collection.FindOneByIdAs<T>(id);
        }

        public void Delete(Guid id)
        {
            base.Collection.Remove(Query.EQ("_id", id));
        }
    }
}
