using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TaskData.Notes;
using TaskData.TasksGroups;
using TaskData.TaskStatus;
using TaskData.WorkTasks;

[assembly: InternalsVisibleTo("Composition")]
[assembly: InternalsVisibleTo("ObjectSerializer.JsonService.Tests")]
namespace TaskData.ObjectSerializer.JsonService
{
    public class JsonSerializerWrapper : IObjectSerializer
    {
        private readonly JsonSerializerSettings mDeserializtionSettings = new JsonSerializerSettings();

        public async Task Serialize<T>(T objectToSerialize, string databasePath)
        {
            string jsonText = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented, new JsonSerializerSettings());
            await File.WriteAllTextAsync(databasePath, jsonText).ConfigureAwait(false);
        }

        public async Task<T> Deserialize<T>(string databasePath)
        {
            string databaseTaxt = await File.ReadAllTextAsync(databasePath).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(databaseTaxt, mDeserializtionSettings);
        }

        public void RegisterConverters(JsonConverter converter)
        {
            mDeserializtionSettings.Converters.Add(converter);
        }

        public class TaskConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(IWorkTask);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return serializer.Deserialize(reader, typeof(WorkTask));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value, typeof(WorkTask));
            }
        }

        public class TaskGroupConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ITasksGroup);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return serializer.Deserialize(reader, typeof(TasksGroup));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value, typeof(TasksGroup));
            }
        }

        public class NoteConverter : JsonConverter
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

        public class TaskStatusHistoryConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ITaskStatusHistory);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return serializer.Deserialize(reader, typeof(TaskStatusHistory));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value, typeof(TaskStatusHistory));
            }
        }
    }
}