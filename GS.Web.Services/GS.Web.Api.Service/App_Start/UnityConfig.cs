using System;
using GS.Interfaces.Service;
using GS.Web.Api.Service01.Controllers;
using Unity;
using Unity.Lifetime;

namespace GS.Web.Api.Service01
{
    /// <summary>
    /// Specifies the Unity configuration for the main _container.
    /// </summary>
    /// 

    public static class UnityConfig
    {
        public static IUnityContainer Container => _container;
        private static Unity.UnityContainer _container;
        public static IUnityContainer GetConfiguredContainer()
        {
            _container = new UnityContainer();
            // Register controller
            //  _container.RegisterType<WebApiService>();
            //  _container.RegisterType<WhoAreYouController>();
            // _container.RegisterType<WhoAreYouController>();
            // Register interface
            //_container.RegisterType<IMyService, MyService>(TypeLifetime.Singleton);
            _container.RegisterType<IMyService, MyService>
                        (new ContainerControlledLifetimeManager());
            _container.Resolve<WebApiService>();
            _container.Resolve<WhoAreYouController>();

            //This is done in Startup instead.
            //GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(_container);
            return _container;
        }
}

public static class UnityConfigOld
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<WhoAreYouController>();
            container.RegisterType<IMyService, MyService>();
        }
    }
}