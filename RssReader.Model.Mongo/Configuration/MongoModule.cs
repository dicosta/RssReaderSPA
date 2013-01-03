using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using RssReader.Model.Contracts;
using RssReader.Model.Mongo.Infrastructure;
using RssReader.Model.Mongo.Infrastructure.Contracts;

namespace RssReader.Model.Mongo.Configuration
{
    public class MongoModule : NinjectModule
    {
        public override void Load()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ReaderRssMongoDB"].ConnectionString;

            IMongoCollectionProvider collectionProvider = new MongoCollectionProvider(connectionString);

            var indexInitializer = new IndexInitializer(collectionProvider);
            indexInitializer.CreateIndexes();

            Bind<IMongoCollectionProvider>().ToConstant(collectionProvider)
                .InSingletonScope();

            //In Web Project.
            /*
            Bind<IUnitOfWork>().To<MongoUnitOfWork>()
                .InRequestScope();
            */
        }
    }
}
