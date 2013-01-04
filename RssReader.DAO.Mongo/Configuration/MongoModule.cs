using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using RssReader.Model.Contracts;
using RssReader.DAO.Mongo.Infrastructure;
using RssReader.DAO.Mongo.Infrastructure.Contracts;
using NLog;

namespace RssReader.DAO.Mongo.Configuration
{
    public class MongoModule : NinjectModule
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public override void Load()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ReaderRssMongoDB"].ConnectionString;

            logger.Debug("initializing mongo collection provider");
            IMongoCollectionProvider collectionProvider = new MongoCollectionProvider(connectionString);

            logger.Debug("ensuring mongo indexes");
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
