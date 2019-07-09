namespace Database.Contracts
{
     public interface IConfiguration
     {
          string DatabaseDirectoryPath { get; }
          void SetDatabaseDirectoryPath(string newDatabaseDirectoryPath);
     }
}