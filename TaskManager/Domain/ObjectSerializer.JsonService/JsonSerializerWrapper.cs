using Logger;
using Logger.Contracts;
using Newtonsoft.Json;
using ObjectSerializer.Contracts;
using System;
using System.IO;
using TaskData;
using TaskData.Contracts;

namespace Database.JsonService
{
     public class JsonSerializerWrapper : IObjectSerializer
     {
          public void Serialize<T>(T objectToSerialize, string databasePath)
          {
               string jsonText = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented);
               File.WriteAllText(databasePath, jsonText);
          }

          public T Deserialize<T>(string databasePath)
          {
               JsonSerializerSettings settings = new JsonSerializerSettings();
               settings.Converters.Add(new TaskGroupConverter());
               settings.Converters.Add(new TaskConverter());
               settings.Converters.Add(new LoggerConverter());
               settings.Converters.Add(new NoteConverter());
               return JsonConvert.DeserializeObject<T>(File.ReadAllText(databasePath), settings);
          }

          private class TaskConverter : JsonConverter
          {
               public override bool CanConvert(Type objectType)
               {
                    return objectType == typeof(ITask);
               }

               public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
               {
                    return serializer.Deserialize(reader, typeof(Task));
               }

               public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
               {
                    serializer.Serialize(writer, value, typeof(Task));
               }
          }

          private class TaskGroupConverter : JsonConverter
          {
               public override bool CanConvert(Type objectType)
               {
                    return objectType == typeof(ITaskGroup);
               }

               public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
               {
                    return serializer.Deserialize(reader, typeof(TaskGroup));
               }

               public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
               {
                    serializer.Serialize(writer, value, typeof(TaskGroup));
               }
          }

          private class LoggerConverter : JsonConverter
          {
               public override bool CanConvert(Type objectType)
               {
                    return objectType == typeof(ILogger);
               }

               public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
               {
                    return serializer.Deserialize(reader, typeof(ConsoleLogger));
               }

               public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
               {
                    serializer.Serialize(writer, value, typeof(ConsoleLogger));
               }
          }

          private class NoteConverter : JsonConverter
          {
               public override bool CanConvert(Type objectType)
               {
                    return objectType == typeof(INote);
               }

               public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
               {
                    return serializer.Deserialize(reader, typeof(Note));
               }

               public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
               {
                    serializer.Serialize(writer, value, typeof(Note));
               }
          }
     }
}