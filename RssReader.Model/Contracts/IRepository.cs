using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RssReader.Model.Contracts
{
    public interface IRepository<T> : IReadOnlyRepository<T> where T : class
    {
        void Add(T newItem);

        void Add(IEnumerable<T> newItems);

        void Update(T item);

        void Update(IEnumerable<T> items);

        void DeleteAll();

        int Count();

        bool Exists(Expression<Func<T, bool>> criteria);
    }
}
