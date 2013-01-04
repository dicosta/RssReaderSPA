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
using Rhino.Mocks;
using RssReader.Model;
using RssReader.Model.Contracts;
using RssReader.Services.Contracts;

namespace MongoTestApp
{
    class Program
    {
        public static void Main(string[] args)
        {
            var p = new Program();

            var kernel = p.CreateKernel();
            
            
            var userRepository = kernel.Get<IGuidKeyedRepository<User>>();
            var userService = kernel.Get<IUserService>();
            var feedService = kernel.Get<IFeedService>();
            var newService = kernel.Get<INewService>();

            userRepository.Add(new User()
            {
                UserName = "Diego"                
            });

            var currentUserProvider = kernel.Get<ICurrentUserProvider>();

            userService.AddCategory("Noticias");
            userService.AddCategory("Deportes");
            userService.AddCategory("Espectaculos");
                        
            feedService.SuscribeFeed("http://www.43folders.com/rss.xml");

            var feedId = currentUserProvider.GetCurrentUser().Feeds.First();
            
            feedService.RefreshFeed(feedId);

            var firstNewId = newService.GetNews().First().Id;
            var lastNewId = newService.GetNews().Last().Id;

            newService.TagNew(firstNewId, "Noticias");
            newService.TagNew(firstNewId, "Deportes");
            newService.TagNew(lastNewId, "Espectaculos");

            newService.UnTagNew(firstNewId, "Deportes");

            var noticias = newService.GetNewsByTag("Noticias");
            var deportes = newService.GetNewsByTag("Deportes");
            var espectaculos = newService.GetNewsByTag("Espectaculos");
            
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
