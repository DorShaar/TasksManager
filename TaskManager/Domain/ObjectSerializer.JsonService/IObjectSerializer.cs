namespace ObjectSerializer.JsonService
{
     public interface IObjectSerializer
     {
          void Serialize<T>(T objectToSerialize, string outputPath);
          T Deserialize<T>(string inputPath);
     }
}