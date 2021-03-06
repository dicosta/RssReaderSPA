﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using RssReader.Model;
using RssReader.Model.Contracts;
using Ninject.Web.Common;
using RssReader.DAO.Mongo.Infrastructure;
using RssReader.Services.Contracts;
using MongoTestApp.TestHelpers;

namespace MongoTestApp.Modules
{
    public class PersistenceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<MongoUnitOfWork>()
                .InRequestScope();

            Bind<ICurrentUserProvider>().To<CurrentUserProvider>()
                .InRequestScope();
        }
    }
}
