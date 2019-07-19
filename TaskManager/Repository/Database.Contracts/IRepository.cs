using System.Collections.Generic;
using TaskData.Contracts;

namespace Database.Contracts
{
     public interface IRepository<T> where T : ITaskGroup
     {
          string DatabasePath { get; }
          string NotesDirectoryPath { get; }

          IEnumerable<T> GetAll();
          T GetEntity(string entity);
          void Insert(T entity);
          void Update(T entity);
          void AddOrUpdate(T entity);
          void Remove(T entity);

          void SetDatabasePath(string newDatabasePath);
     }
}