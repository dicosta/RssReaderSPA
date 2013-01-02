using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReader.Model.Contracts
{
    public interface IReadOnlyRepository<T> where T : class
    {
        IQueryable<T> GetAll();
    }
}
