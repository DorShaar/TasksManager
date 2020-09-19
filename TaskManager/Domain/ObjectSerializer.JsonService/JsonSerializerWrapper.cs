using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using TaskData.Notes;
using TaskData.TasksGroups;
using TaskData.TaskStatus;
using TaskData.WorkTasks;

[assembly: InternalsVisibleTo("Composition")]
namespace ObjectSerializer.JsonService
{
    internal class JsonSerializerWrapper : IObjectSerializer
    {
        public void Serialize<T>(T objectToSerialize, string databasePath)
        {
            string jsonText = JsonConvert.SerializeObject(objectToSerialize, Formatting.Indented, new JsonSerializerSettings());
            File.WriteAllText(databasePath, jsonText);
        }

        public T Deserialize<T>(string databasePath)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new TaskGroupConverter());
            settings.Converters.Add(new TaskConverter());
            settings.Converters.Add(new NoteConverter());
            settings.Converters.Add(new TaskStatusHistoryConverter());
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(databasePath), settings);
        }

        private class TaskConverter : JsonConverter
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

        private class TaskGroupConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ITasksGroup);
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

        private class TaskStatusHistoryConverter : JsonConverter
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