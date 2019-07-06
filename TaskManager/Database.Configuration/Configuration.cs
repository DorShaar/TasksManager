using Database.Contracts;

namespace Database.Configuration
{
     /// <summary>
     /// Configuration injection:
     /// https://dev.to/justinjstark/injecting-configuration-variables-into-components-4knh
     /// </summary>
     public class Configuration : IConfiguration
     {
          public string DatabasePath { get; set; }
     }
}