//[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Multi_Language.DataApi.App_Start.NinjectWebCommon), "Start")]
//[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Multi_Language.DataApi.App_Start.NinjectWebCommon), "Stop")]

using Data;
using Microsoft.Owin.Security.OAuth;
using Ninject;
using Ninject.Extensions.Conventions;
using TCM.WebAPI.Providers;
using TCM.WebAPI.Tasks;

namespace TCM.WebAPI
{
    public static class NinjectWebCommon
    {
        private static StandardKernel kernel;
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        public static IKernel CreateKernel()
        {
            kernel = new StandardKernel();
            try
            {
                RegisterServices(kernel);
                // GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);
                //DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        public static IKernel GetKernel()
        {
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(typeof(IRepository<>)).To(typeof(EfGenericRepository<>));


            kernel.Bind<IBearerTokenExpirationTask>().To<BearerTokenExpirationTask>();


            kernel.Bind<IOAuthAuthorizationServerProvider>().To<AuthorizationServerProvider>().WithConstructorArgument("bearerTokenExpirationTask", kernel.Get<IBearerTokenExpirationTask>());
           
        }


    }
}
