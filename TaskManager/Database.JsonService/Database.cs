using Logger.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TaskData.Contracts;

namespace Database.JsonService
{
     public class Database : IRepository<ITaskGroup>
     {
          private readonly ILogger mLogger;
          private List<ITaskGroup> 

          public Database(string databasePath, ILogger logger)
          {
               mLogger = logger;

               if(!File.Exists(databasePath))
               {
                    mLogger.LogError($"No database found in path {databasePath}");
                    return;
               }

               LoadFromFile(databasePath);
          }

          private void LoadFromFile(string databasePath)
          {
               try
               {
                    mDatabase = JsonConvert.DeserializeObject<>(File.ReadAllText(databasePath));
               }
               catch (Exception ex)
               {
                    mLogger.LogError($"Unable to serialize database in {databasePath}", ex);
               }
          }

          public void Delete(ITaskGroup entity)
          {
               throw new System.NotImplementedException();
          }

          public ITaskGroup Get(long id)
          {
               throw new System.NotImplementedException();
          }

          public IEnumerable<ITaskGroup> GetAll()
          {
               throw new System.NotImplementedException();
          }

          public void Insert(ITaskGroup entity)
          {
               throw new System.NotImplementedException();
          }

          public void Remove(ITaskGroup entity)
          {
               throw new System.NotImplementedException();
          }

          public void SaveChanges()
          {
               throw new System.NotImplementedException();
          }

          public void Update(ITaskGroup entity)
          {
               throw new System.NotImplementedException();
          }
     }
}