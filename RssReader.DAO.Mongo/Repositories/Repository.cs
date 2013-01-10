using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReader.Model.Contracts;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace RssReader.DAO.Mongo.Repositories
{
    public class Repository<T> : ReadOnlyRepository<T>, IRepository<T> where T : class
    {
        public Repository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public virtual void Add(T newItem)
        {
            base.Collection.Insert(newItem);
        }

        public virtual void Add(IEnumerable<T> newItems)
        {
            base.Collection.Insert(newItems);
        }

        public virtual void Update(T item)
        {
            base.Collection.Save(item);
        }

        public virtual void Update(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                base.Collection.Save(item);
            }
        }

        public virtual void DeleteAll()
        {
            base.Collection.RemoveAll();
        }

        public virtual int Count()
        {
            return (int)base.Collection.Count();
        }

        public virtual bool Exists(System.Linq.Expressions.Expression<Func<T, bool>> criteria)
        {
            return base.Collection.AsQueryable<T>().Any(criteria);
        }
    }
}
