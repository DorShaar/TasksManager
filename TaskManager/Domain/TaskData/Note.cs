using TaskData.Contracts;

namespace TaskData
{
     internal class Note : INote
     {
          public string NotePath => throw new System.NotImplementedException();

          string INote.Note => throw new System.NotImplementedException();
     }
}
