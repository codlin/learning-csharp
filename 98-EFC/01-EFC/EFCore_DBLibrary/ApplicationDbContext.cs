
using Microsoft.EntityFrameworkCore;

namespace EFCore_DBLibrary;

public class AdventureWorksContext : DbContext
{
    //Add a default constructor if scaffolding is needed
    public AdventureWorksContext() { }
    //Add the complex constructor for allowing Dependency Injection
    public AdventureWorksContext(DbContextOptions<AdventureWorksContext> options)
    : base(options)
    {
        //intentionally empty.
    }
}
