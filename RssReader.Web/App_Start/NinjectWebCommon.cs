[assembly: WebActivator.PreApplicationStartMethod(typeof(RssReader.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(RssReader.Web.App_Start.NinjectWebCommon), "Stop")]

namespace RssReader.Web.App_Start
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Security;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Common;
    using RssReader.Services.Contracts;
    using RssReader.Web.Infrastructure.Auth;
    using RssReader.Web.Infrastructure.IOC;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
                        
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load("RssReader*.dll");

            //fix for webapi & ninject
            //http://www.strathweb.com/2012/05/using-ninject-with-the-latest-asp-net-web-api-source/
            GlobalConfiguration.Configuration.DependencyResolver = new NinjectResolver(kernel);

            //sets user service on mongo membership provider          
  
            /*
            (Membership.Provider as MongoExtendedMemberShipProvider).UserServiceProvider = () =>
            {
                return kernel.Get<IUserService>();
            };
             * 
            */
 
        }        
    }
}
