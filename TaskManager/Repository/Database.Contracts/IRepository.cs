using System.Collections.Generic;

namespace Database.Contracts
{
    public interface IRepository<T>
    {
        string DatabasePath { get; }

        IEnumerable<T> GetAll();
        T GetEntity(string entity);
        void Insert(T entity);
        void Update(T entity);
        void AddOrUpdate(T entity);
        void Remove(T entity);

        void SetDatabasePath(string newDatabasePath);
    }
}