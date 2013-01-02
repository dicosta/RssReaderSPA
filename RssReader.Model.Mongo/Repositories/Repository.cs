using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReader.Model.Contracts;
using MongoDB.Driver.Linq;

namespace RssReader.Model.Mongo.Repositories
{
    public class Repository<T> : ReadOnlyRepository<T>, IRepository<T> where T : class
    {
        public virtual void Add(T newItem)
        {
            base._collection.Insert(newItem);
        }

        public virtual void Add(IEnumerable<T> newItems)
        {
            base._collection.Insert(newItems);
        }

        public virtual void Update(T item)
        {
            base._collection.Save(item);
        }

        public virtual void Update(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                base._collection.Save(item);
            }
        }

        public virtual void DeleteAll()
        {
            base._collection.RemoveAll();
        }

        public virtual int Count()
        {
            return (int)base._collection.Count();
        }

        public virtual bool Exists(System.Linq.Expressions.Expression<Func<T, bool>> criteria)
        {
            return base._collection.AsQueryable<T>().Any(criteria);
        }
    }
}
