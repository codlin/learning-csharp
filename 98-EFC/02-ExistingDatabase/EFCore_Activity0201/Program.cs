using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfigurationRoot configurationRoot = builder.Build();
Console.WriteLine($"CNSTR: {configurationRoot.GetConnectionString("AdventureWorks")}");
