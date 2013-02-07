using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Modules;
using RssReader.DAO.Mongo.Infrastructure;
using RssReader.Model.Contracts;
using RssReader.Services.Contracts;
using Ninject.Web.Common;
using RssReader.Web.Infrastructure.Auth;

namespace RssReader.Web.Infrastructure.Modules
{
    public class WebAppModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<MongoUnitOfWork>()
                .InRequestScope();

            Bind<ICurrentUserNameProvider>().To<CurrentUserNameProvider>()
                .InRequestScope();

            Bind<ICurrentUserProvider>().To<CurrentUserProvider>()
                .InRequestScope();
        }
    }
}