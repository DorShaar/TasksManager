using Microsoft.Extensions.DependencyInjection;

namespace TaskData.ObjectSerializer.JsonService
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