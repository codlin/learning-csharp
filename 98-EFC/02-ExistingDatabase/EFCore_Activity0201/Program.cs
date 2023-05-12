using EFCore_DBLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

IConfigurationRoot configurationRoot = builder.Build();
Console.WriteLine($"CNSTR: {configurationRoot.GetConnectionString("AdventureWorks")}");

DbContextOptions<>

ListPeople();

void ListPeople()
{
    using (var db = new AdventureWorksContext(configurationRoot))
    {
        var people = db.People.OrderByDescending(x => x.LastName).Take(20).
        ToList();
        foreach (var person in people)
        {
            Console.WriteLine($"{person.FirstName} {person.LastName}");
        }
    }
}
}