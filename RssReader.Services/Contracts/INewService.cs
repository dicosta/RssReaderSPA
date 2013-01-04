using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReader.Model;

namespace RssReader.Services.Contracts
{
    public interface INewService
    {
        void TagNew(Guid newId, string categoryName);

        void UnTagNew(Guid newId, string categoryName);

        IQueryable<New> GetNews();

        IQueryable<New> GetNewsByTag(string categoryName);
    }
}
