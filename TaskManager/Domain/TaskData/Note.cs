using Newtonsoft.Json;
using System.IO;
using TaskData.Contracts;

namespace TaskData
{
     public class Note : INote
     {
          private readonly string Extension;

          [JsonProperty]
          private readonly string mNoteDirecoryPath;

          [JsonProperty]
          private readonly string mNoteName;

          [JsonIgnore]
          public string NotePath => Path.Combine(mNoteDirecoryPath, mNoteName + Extension);

          [JsonIgnore]
          public string NoteText => File.ReadAllText(NotePath);

          [JsonConstructor]
          public Note(string directoryPath, string noteName)
          {
               Extension = ".txt";
               mNoteDirecoryPath = directoryPath;
               mNoteName = noteName;

               if(!string.IsNullOrEmpty(directoryPath) && !string.IsNullOrEmpty(noteName))
               {
                    Directory.CreateDirectory(directoryPath);
                    File.Create(NotePath);
               }
          }

          public Note(string directoryPath, string noteName, string content)
          {
               Extension = ".txt";
               mNoteDirecoryPath = directoryPath;
               mNoteName = noteName;

               if (!string.IsNullOrEmpty(directoryPath) && !string.IsNullOrEmpty(noteName))
               {
                    Directory.CreateDirectory(directoryPath);
                    File.WriteAllText(NotePath, content);
               }
          }
     }
}
