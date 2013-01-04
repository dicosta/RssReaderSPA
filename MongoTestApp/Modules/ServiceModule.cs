using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using RssReader.Services;
using RssReader.Services.Contracts;
using Ninject.Web.Common;
using Ninject;
using Rhino.Mocks;
using RssReader.Model;
using RssReader.Model.Contracts;

namespace MongoTestApp.Modules
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
