using Microsoft.Extensions.DependencyInjection;
using System;
namespace SES.Store
{
    
    public class StoreConfiguration
    {
        public string ConnectionString{get;set;}
    }
    public static class DependencyInjectionSupport
    {
        public static void AddSESStore<T>(this IServiceCollection services) where T:class,IEventStore,new()
        {
            services.AddScoped<IEventStore,T>();
        }

        public static void AddSESStore<T>(this IServiceCollection services, Func<IServiceProvider,T> eventStoreFactory) where T:class,IEventStore
        {
            if(eventStoreFactory==null)
            {
                throw new ArgumentNullException(nameof(eventStoreFactory));
            }
            services.AddScoped<IEventStore,T>(eventStoreFactory);
        }
    }
}