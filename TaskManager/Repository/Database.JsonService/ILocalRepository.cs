using Database.Configuration;
using System.Collections.Generic;

namespace Databases
{
    public interface ILocalRepository<T> : IRepository<T>, IDatabaseLocalConfiguration
    {
    }

    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T GetEntity(string entity);
        void Insert(T entity);
        void Update(T entity);
        void AddOrUpdate(T entity);
        void Remove(T entity);
    }
}