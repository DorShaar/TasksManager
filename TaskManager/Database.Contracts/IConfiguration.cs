namespace Database.Contracts
{
     public interface IConfiguration
     {
          string DatabasePath { get; }
          void SetDatabasePath(string newDatabasePath);
     }
}