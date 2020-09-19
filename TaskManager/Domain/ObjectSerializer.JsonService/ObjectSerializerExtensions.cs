using Microsoft.Extensions.DependencyInjection;

namespace ObjectSerializer.JsonService
{
    public static class ObjectSerializerExtensions
    {
        public static void UseJsonObjectSerializer(this IServiceCollection services)
        {
            services.AddSingleton<IObjectSerializer, JsonSerializerWrapper>();
        }
    }
}