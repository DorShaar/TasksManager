namespace Database.Contracts
{
    public interface ILocalRepository<T> : IRepository<T>, IDatabaseLocalConfiguration
    {
    }
}