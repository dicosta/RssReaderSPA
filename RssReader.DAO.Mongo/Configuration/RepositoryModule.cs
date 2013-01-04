using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using RssReader.Model.Contracts;
using RssReader.DAO.Mongo.Repositories;
using RssReader.Model;

namespace RssReader.DAO.Mongo.Configuration
{
    public class RepositoryModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IGuidKeyedRepository<Feed>>().To<GuidKeyedRepository<Feed>>().InTransientScope();

            Bind<IGuidKeyedRepository<New>>().To<GuidKeyedRepository<New>>().InTransientScope();

            Bind<IGuidKeyedRepository<User>>().To<GuidKeyedRepository<User>>().InTransientScope();

            /*
            Bind<IGuidKeyedRepository<Feed>>().To<GuidKeyedRepository<Feed>>().InTransientScope();
            */
        }
    }
}
