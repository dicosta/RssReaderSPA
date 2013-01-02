using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RssReader.Model.Contracts
{
    public interface IGuidKeyedRepository<T> : IRepository<T> where T : class
    {
        T GetById(Guid id);

        void Delete(Guid id);
    }
}
