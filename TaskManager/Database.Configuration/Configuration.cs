using Database.Contracts;
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
          internal string ConfigurationYamlFilePath = 
               Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config", "Config.yaml");

          public string DatabaseDirectoryPath
          {
               get => GetDatabasePath();
          }

          private string GetDatabasePath()
          {
               using (StreamReader reader = new StreamReader(ConfigurationYamlFilePath))
               {
                    YamlStream yamlStream = new YamlStream();
                    yamlStream.Load(reader);
                    return GetDatabasePathNode(yamlStream).Value;
               }
          }

          public void SetDatabaseDirectoryPath(string newDatabasePath)
          {
               YamlScalarNode pathNode;
               YamlStream yamlStream = new YamlStream();

               using (StreamReader reader = new StreamReader(ConfigurationYamlFilePath))
               {
                    yamlStream.Load(reader);
                    pathNode = GetDatabasePathNode(yamlStream);
               }

               pathNode.Value = newDatabasePath;
               using (TextWriter writer = File.CreateText(ConfigurationYamlFilePath))
               {
                    yamlStream.Save(writer, assignAnchors: false);
               }
          }

          private YamlScalarNode GetDatabasePathNode(YamlStream yamlStream)
          {
               YamlMappingNode rootNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;
               var pathKeyValueNode = rootNode.Children[new YamlScalarNode("database")];
               return (YamlScalarNode)(pathKeyValueNode as YamlMappingNode).Children[new YamlScalarNode("directory_path")];
          }
     }
}