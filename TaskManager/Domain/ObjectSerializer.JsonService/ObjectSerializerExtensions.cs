using Microsoft.Extensions.DependencyInjection;

namespace ObjectSerializer.JsonService
{
    public static class ObjectSerializerExtensions
    {
        public static IServiceCollection UseJsonObjectSerializer(this IServiceCollection services)
        {
            services.AddSingleton<IObjectSerializer, JsonSerializerWrapper>();

            return services;
        }
    }
}