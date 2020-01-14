using Microsoft.Extensions.DependencyInjection;
using SES.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SES.Serialization
{
    public static class DependencyInjectionSupport
    {
        public static void AddSESJsonSerialization(this IServiceCollection services)
        {
            services.AddSingleton<IAsyncEventSerializer, JsonSerializer>();
        }
    }
}
