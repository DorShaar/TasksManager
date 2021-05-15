using Newtonsoft.Json;
using System.Threading.Tasks;

namespace TaskData.ObjectSerializer.JsonService
{
    public interface IObjectSerializer
    {
        Task Serialize<T>(T objectToSerialize, string outputPath);
        Task<T> Deserialize<T>(string inputPath);
        void RegisterConverters(JsonConverter converter);
    }
}