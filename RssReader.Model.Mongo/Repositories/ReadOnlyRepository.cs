using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RssReader.Model.Contracts;
using RssReader.Model.Mongo.Infrastructure;

namespace RssReader.Model.Mongo.Repositories
{
    public class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class
    {
        private MongoCollection<T> _collection;

        protected readonly IUnitOfWork _unitOfWork;

        public ReadOnlyRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected MongoCollection<T> Collection
        {
            get
            {
                if (_collection == null)
                {
                    _collection = ((MongoUnitOfWork)_unitOfWork).GetMongoCollection<T>();
                }

                return _collection;
            }
        }
        
        public virtual IQueryable<T> GetAll()
        {
            return Collection.AsQueryable<T>();
        }
    }
}
