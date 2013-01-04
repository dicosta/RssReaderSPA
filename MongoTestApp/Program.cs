using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Ninject;
using RssReader.Model;
using RssReader.Services.Contracts;

namespace MongoTestApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            var p = new Program();
            var kernel = p.CreateKernel();

            var feedService = kernel.Get<IFeedService>();

            Feed newFeed = new Feed()
            {
                URL = "http://www.43folders.com/rss.xml"
            };


            feedService.SuscribeFeed(newFeed);

            feedService.RefreshFeed(newFeed.Id);

            /*
            var connectionString = "mongodb://localhost";

            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test");

            BsonClassMap.RegisterClassMap<Feed>(cm =>
            {
                cm.AutoMap();                
                cm.SetIdMember(cm.GetMemberMap(c => c.Id));
            });



            var collection = database.GetCollection<Feed>("feeds");

            var entity = new Feed { URL = "www.google.com" };
            collection.Insert(entity);
            var id = entity.Id;

            
            var query = Query.EQ("_id", id);
            entity = collection.FindOne(query);                       
            
            entity.URL = "www.blabla.com";
            collection.Save(entity);
            */ 
              
            /*
            var update = Update.Set("Name", "Harry");
            collection.Update(query, update);
            
            collection.Remove(query);
            */
            
        }

        protected IKernel CreateKernel()
        {
            var settings = new NinjectSettings { LoadExtensions = false  };

            var kernel = new StandardKernel(settings);

            //kernel.Load(Assembly.GetExecutingAssembly());
            kernel.Load(AppDomain.CurrentDomain.GetAssemblies());
            kernel.Load("*.dll");
         
            return kernel;
        }
    }
}
