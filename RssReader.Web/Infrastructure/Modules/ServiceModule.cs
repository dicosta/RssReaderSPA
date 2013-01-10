using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject.Modules;
using RssReader.Services;
using RssReader.Services.Contracts;
using Ninject.Web.Common;

namespace RssReader.Web.Infrastructure.Modules
{
    public class ServiceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFeedService>().To<FeedService>()
                .InRequestScope();

            Bind<IUserService>().To<UserService>()
                .InRequestScope();

            Bind<INewService>().To<NewService>()
                .InRequestScope();
        }
    }
}