using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RssReader.Model;

namespace RssReader.Services.Contracts
{
    public interface IFeedService
    {
        Feed Suscribe(string feedURL);

        void Unsuscribe(Guid feedId);

        void RefreshFeed(Guid feedId);

        Feed GetById(Guid feedId);
    }
}
