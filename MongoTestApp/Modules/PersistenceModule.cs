﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using RssReader.Model;
using RssReader.Model.Contracts;
using RssReader.Model.Mongo.Infrastructure;
using RssReader.Model.Mongo.Repositories;
using Ninject.Web.Common;

namespace MongoTestApp.Modules
{
    public class PersistenceModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUnitOfWork>().To<MongoUnitOfWork>()
                .InRequestScope();
        }
    }
}
