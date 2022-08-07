
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinkVault.Context;
using LinkVault.Stores;
using LinkVault.ViewModels;
using LinkVault.Views;
using LinkVault.Services;
using ReactiveUI;
using Splat;
using Microsoft.Extensions.DependencyInjection;

namespace LinkVault
{
    public static class Bootstrapper
    {

        public static void Register(IMutableDependencyResolver services)
        {
            services.RegisterLazySingleton<AppDbContext>(CreateWithConstructorInjection<AppDbContext>);

            services.Register(() => new CollectionExplorerView(), typeof(IViewFor<CollectionExplorerViewModel>));
            services.Register(() => new CollectionView(), typeof(IViewFor<CollectionViewModel>));
            services.Register(() => new CreateCollectionWindow(), typeof(IViewFor<CreateCollectionViewModel>));

            // Stores
            services.RegisterLazySingleton<CollectionStore>(() => new CollectionStore());
            services.RegisterLazySingleton<LinkStore>(() => new LinkStore());

            // Services
            services.RegisterLazySingleton<ServerService>(() => new ServerService());
            services.RegisterLazySingleton<MessageBusService>(() => new MessageBusService());
        }

        public static T CreateWithConstructorInjection<T>() where T : class
        {
            ConstructorInfo[] constructors = typeof(T).GetConstructors();
            if (constructors.Count() > 1)
                throw new InvalidOperationException($"Unable to create required dependency for {typeof(T).FullName}: type can not have more than one constructor, found {constructors.Count()}");

            // must not be null
            IEnumerable<Type> types = constructors.Single().GetParameters().Select(p => p.ParameterType).ToArray();
            if (Activator.CreateInstance(typeof(T), types.Select(GetService).ToArray()) is T t)
                return t;

            throw new InvalidOperationException($"Unable to create required dependency of type {typeof(T).FullName}: Activator.CreateInstance() returned null");
        }

        public static object GetService(Type type)
        {
            if (Locator.Current.GetService(type) is Object obj)
                return obj;

            throw new InvalidOperationException($"Unable to create required dependency of type {type.FullName}: IReadonlyDependencyResolver.GetService() returned null");
        }
    }
}