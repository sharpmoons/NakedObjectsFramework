using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Practices.Unity;
using NakedObjects;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter.Map;
using NakedObjects.Core.Context;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Security;
using NakedObjects.EntityObjectStore;
using NakedObjects.Objects;
using NakedObjects.Persistor;
using NakedObjects.Persistor.Objectstore;
using NakedObjects.Reflector.DotNet;
using NakedObjects.Reflector.DotNet.Facets;
using NakedObjects.Reflector.DotNet.Reflect;
using NakedObjects.Reflector.DotNet.Reflect.Strategy;
using NakedObjects.Service;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;
using RestfulObjects.Test.Data;

namespace MvcTestApp
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion


        public static Type[] Services() {
            return new[] {
                typeof (RestDataRepository),
                typeof (WithActionService),
                typeof (ContributorService),
                typeof (TestTypeCodeMapper)
            };
        }

        private static object[] MenuServices {
            get {
                return new object[] {
                    new RestDataRepository(),
                    new WithActionService()
                };
            }
        }


        private static object[] ContributedActions {
            get {
                return new object[] {
                    new ContributorService()
                };
            }
        }


        private static object[] SystemServices {
            get {
                return new object[] {
                    new TestTypeCodeMapper()
                };
            }
        }


        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            container.RegisterType<IClassStrategy, DefaultClassStrategy>();
            container.RegisterType<IFacetFactorySet, FacetFactorySetImpl>();
            container.RegisterType<INakedObjectReflector, DotNetReflector>(new ContainerControlledLifetimeManager());

            container.RegisterType<IPrincipal>(new InjectionFactory(c => HttpContext.Current.User));

            var config = new EntityObjectStoreConfiguration();

            //config.UsingEdmxContext("Model").AssociateTypes(AdventureWorksTypes);
            //config.SpecifyTypesNotAssociatedWithAnyContext(() => new[] { typeof(AWDomainObject) });

            config.UsingCodeFirstContext(() => new CodeFirstContext("RestTest"));

            container.RegisterInstance(typeof(EntityObjectStoreConfiguration), config, new ContainerControlledLifetimeManager());

            var serviceConfig = new ServicesConfiguration();

            serviceConfig.AddMenuServices(MenuServices);
            serviceConfig.AddContributedActions(ContributedActions);
            serviceConfig.AddSystemServices(SystemServices);

            container.RegisterInstance(typeof(ServicesConfiguration), serviceConfig, new ContainerControlledLifetimeManager());

            container.RegisterType<IPocoAdapterMap, PocoAdapterHashMap>(new PerResolveLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new PerResolveLifetimeManager(), new InjectionConstructor(10));


            container.RegisterType<IContainerInjector, DotNetDomainObjectContainerInjector>(new PerResolveLifetimeManager());

            container.RegisterType<IOidGenerator, EntityOidGenerator>(new PerResolveLifetimeManager());
            container.RegisterType<IPersistAlgorithm, EntityPersistAlgorithm>(new PerResolveLifetimeManager());
            container.RegisterType<INakedObjectStore, EntityObjectStore>(new PerResolveLifetimeManager());
            container.RegisterType<IIdentityMap, EntityIdentityMapImpl>(new PerResolveLifetimeManager());

            container.RegisterType<IAuthorizationManager, NullAuthorizationManager>(new PerResolveLifetimeManager());
            container.RegisterType<INakedObjectPersistor, ObjectStorePersistor>(new PerResolveLifetimeManager());
            container.RegisterType<ISession, WindowsSession>(new PerResolveLifetimeManager());
            container.RegisterType<IUpdateNotifier, SimpleUpdateNotifier>(new PerResolveLifetimeManager());
            container.RegisterType<IMessageBroker, SimpleMessageBroker>(new PerResolveLifetimeManager());

            container.RegisterType<INakedObjectsFramework, NakedObjectsFramework>(new PerResolveLifetimeManager());

            container.RegisterType<IOidStrategy, ExternalOid>(new PerResolveLifetimeManager());

            container.RegisterType<INakedObjectsSurface, NakedObjectsSurface>(new PerResolveLifetimeManager());
        }
    }
}