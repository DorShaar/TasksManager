using Database.Contracts;
using Logger.Contracts;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using YamlDotNet.RepresentationModel;

[assembly: InternalsVisibleTo("Database.Configuration.Tests")]
namespace Database.Configuration
{
     /// <summary>
     /// Configuration injection:
     /// https://dev.to/justinjstark/injecting-configuration-variables-into-components-4knh
     /// </summary>
     public class Configuration : IConfiguration
     {
          private readonly ILogger mLogger;

          internal string ConfigurationYamlFilePath =
               Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config", "Config.yaml");

          public string DatabaseDirectoryPath { get => GetDatabasePathNode(LoadYamlStream()).Value; }
          public string NotesDirectoryPath { get => GetNotesPathNode(LoadYamlStream()).Value; }

          public Configuration(ILogger logger)
          {
               mLogger = logger;
          }

          public void SetDatabaseDirectoryPath(string newDatabasePath)
          {
               if (string.IsNullOrEmpty(newDatabasePath))
               {
                    mLogger.LogError($"Invalid new database path: {newDatabasePath}");
                    return;
               }

               YamlStream yamlStream = LoadYamlStream();
               YamlScalarNode pathNode = GetDatabasePathNode(yamlStream);
               pathNode.Value = newDatabasePath;
               SaveYamlStream(yamlStream);

               mLogger.LogError($"New database path set to: {newDatabasePath}");
          }

          public void SetNotesDirectoryPath(string newNotesDirectoryPath)
          {
               if (string.IsNullOrEmpty(newNotesDirectoryPath))
               {
                    mLogger.LogError($"Invalid new database path: {newNotesDirectoryPath}");
                    return;
               }

               YamlStream yamlStream = LoadYamlStream();
               YamlScalarNode pathNode = GetNotesPathNode(yamlStream);
               pathNode.Value = newNotesDirectoryPath;
               SaveYamlStream(yamlStream);

               mLogger.LogError($"New database path set to: {newNotesDirectoryPath}");
          }

          private YamlScalarNode GetDatabasePathNode(YamlStream yamlStream)
          {
               YamlMappingNode rootNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;
               var pathKeyValueNode = rootNode.Children[new YamlScalarNode("database")];
               return (YamlScalarNode)(pathKeyValueNode as YamlMappingNode).Children[new YamlScalarNode("directory_path")];
          }

          private YamlScalarNode GetNotesPathNode(YamlStream yamlStream)
          {
               YamlMappingNode rootNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;
               var pathKeyValueNode = rootNode.Children[new YamlScalarNode("database")];
               return (YamlScalarNode)(pathKeyValueNode as YamlMappingNode).Children[new YamlScalarNode("notes_path")];
          }

          private YamlStream LoadYamlStream()
          {
               using (StreamReader reader = new StreamReader(ConfigurationYamlFilePath))
               {
                    YamlStream yamlStream = new YamlStream();
                    yamlStream.Load(reader);
                    return yamlStream;
               }
          }

          private void SaveYamlStream(YamlStream yamlStream)
          {
               using (TextWriter writer = File.CreateText(ConfigurationYamlFilePath))
               {
                    yamlStream.Save(writer, assignAnchors: false);
               }
          }
     }
}