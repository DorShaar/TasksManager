using System.Threading.Tasks;

namespace ObjectSerializer.JsonService
{
     public interface IObjectSerializer
     {
          Task Serialize<T>(T objectToSerialize, string outputPath);
          Task<T> Deserialize<T>(string inputPath);
     }
}