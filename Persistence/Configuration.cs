
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace Persistence.Contexts

{
    static class Configuration
    {
        static public string ConnectionString
        {
            get
            {

                ConfigurationManager configurationManager = new();
                configurationManager.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "C:\\Users\\akare\\OneDrive\\Masaüstü\\OrganizationApp\\OrganizationApp"));
                configurationManager.AddJsonFile("appsettings.json");

                return configurationManager.GetConnectionString("MSSQL");
            }
        }
    }

    //public static class Configuration
    //{
    //    private static IConfigurationRoot _configuration;

    //    static Configuration()
    //    {
    //        var builder = new ConfigurationBuilder()
    //            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../OrganizationApp"))
    //            .AddJsonFile("appsettings.json");

    //        _configuration = builder.Build();
    //    }

    //    public static string ConnectionString => _configuration.GetConnectionString("MSSQL");
    //}


}
