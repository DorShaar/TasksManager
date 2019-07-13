using System.IO;
using TaskData.Contracts;

namespace TaskData
{
     internal class Note : INote
     {
          private const string Extension = ".txt";
          private readonly string mNoteDirecoryPath;
          private readonly string mNoteName;
          public string NotePath => Path.Combine(mNoteDirecoryPath, mNoteName + Extension);
          public string NoteText => File.ReadAllText(NotePath);

          public Note(string directoryPath, string noteName)
          {
               mNoteDirecoryPath = directoryPath;
               mNoteName = noteName;

               File.Create(NotePath);
          }
     }
}
