using System.Collections.Generic;
using TaskData.Contracts;

namespace Database.JsonService
{
     public interface IRepository<T> where T : ITaskGroup
     {
          IEnumerable<T> GetAll();
          T Get(string id);
          void Insert(T entity);
          void Update(T entity);
          void Remove(T entity);
     }
}