using System.Collections.Generic;
using TaskData.Contracts;

namespace Database.Contracts
{
     public interface IRepository<T> where T : ITaskGroup
     {
          IEnumerable<T> GetAll();
          T GetById(string id);
          T GetByName(string id);
          void Insert(T entity);
          void Update(T entity);
          void Remove(T entity);
          void RemoveById(string id);
          void RemoveByName(string name);
     }
}