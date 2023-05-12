# Models and the Data Context
## What is the database context and why do we need it?
在深入了解 DBContext 及其重要性之前，确定您之前可能从未使用过 DBContext 也很重要。如果您正在阅读本文并且习惯于使用旧版本的 EF（EF4.1 之前的版本），或者如果您在以前的数据库工作中一直使用非代码优先方法，那么您可能从未见过或使用 DBContext。在此类用例中，您可能熟悉称为 ObjectContext 的对象。 ObjectContext 包含对数据库进行操作所需的所有方法，例如 CreateDatabase、SaveChanges 等。
请注意，今后您将不再使用 ObjectContext，而是使用 DBContext 对象。

### DBContext vs. ObjectContext
在早期版本的 EF 中，在某些情况下，DbContext 可以充当 ObjectContext 上的装饰器，因为可以通过包装 ObjectContext 创建 DBContext。必要时也可以从 DBContext 获取对底层 ObjectContext 的访问权。您将无法再在 EFCore6 中采用这种方法，您也不想这样做。在大多数情况下，您现在将利用 DBContextOptionsBuilder。  
在 EF6 中，DBContext 和 ObjectContext 都是同一接口 IObjectContextAdapter 的实现。通过这个通用定义，以及 DBContext 像装饰器一样工作的能力，可以从现有数据库中使用 *.edmx 文件的旧式 EF 过渡到没有 *.edmx 的代码优先方法文件，同时仍然能够支持原始的 *.edmx 实现。  
在 EF6（EFCore 之前的早期 EF 版本）和 EFCore 中，DbContext 对象是代码优先实现的关键组件。 DBContext 包含对数据库进行操作所需的所有关键方法。对于使用 DBContext 的 EF，许多底层模式都是默认实现的，不需要开发人员的手动干预。  
在本书的其余部分，我们将专注于使用 DBContext。此外，我们将专注于 EFCore6 以检查 DBContext，并且不会围绕 DBContext 实现任何遗留代码。但是，在此过程中，我们仍会在适当的时候花时间讨论 EF6 中的不同之处，以防万一您在遗留代码中使用 EF6，或者如果您从其他遗留版本升级到 EFCore6 并且需要了解两种实现之间的差异。

### What is the DBContext?
要开始查看 DBContext，让我们从 Microsoft 文档中获取有关 DbContext 类是什么的官方声明。官方文档对 DbContext 类有以下说明：   
> DbContext 实例表示与数据库的会话，可用于查询和保存实体的实例。 DbContext 是工作单元和存储库模式的组合。  

因此，使用 DbContext，我们可以围绕数据库开发中的两个重要模式进行编排，即工作单元 (UoW) 模式和存储库设计模式。这意味着通过使用 DBContext，我们在使用 DBContext 时不必显式管理简单事务，因为它们将由实现 UoW 模式的上下文处理。  
考虑这一点的另一种方法是理解当您使用来自 DBContext 的代码和对象时，已设置为要修改的所有内容都在同一个隐式事务中进行管理。因此，在对 SaveChanges 进行显式调用之前，所有操作都处于挂起状态并且不会应用。如果在对 SaveChanges 的最后一次调用期间出现问题，整个修改后的集合将被回滚，这对开发人员来说既是福也是祸。  
为了做好充分准备并更好地理解所有这些是如何工作的，我们将在本书后面更详细地深入了解 UoW 和存储库模式。届时，我们还将讨论使用显式事务以及开发人员何时适合使用显式事务。在那之前，我们只会利用内置的 UoW 和存储库模式。  
尽管在许多应用程序中我们与 DBContext 的大部分交互将仅限于添加 DBSet 和一些其他小的代码修改，但花时间了解更多有关 DBContext 的工作原理很划算。在了解  DBContext 的构造方式时，通过了解 DBContext 为我们提供了哪些可用的选项也是一个好主意。我们可以通过深入了解 DBContext 的内部工作原理来更详细地检查这一点，我们将在接下来进行。

### Constructing a new DBContext
在 EFCore6 中，DBContext 只有两个构造函数。在大多数情况下，在创建 DBContext 时，我们将使用复杂的构造函数，它采用 DbContextOptions 对象，但在某些特定情况下，将使用不带参数的默认构造函数。主要是，在为上下文运行迁移或脚手架控制器时使用默认构造函数。   
DBContext 类使我们能够通过 DBContextOptions 类注入选项，以便在数据库与 EF 交互的正常操作期间使用。使用 DBContextOptions 类时，我们通常会使用 DBContextOptionsBuilder 对象，因为 DBContextOptions 类通常是组合和/或注入的，而不是直接创建的。
DBContextOptionsBuilder 为我们提供了几个我们将利用的关键操作。我们可以设置想要使用的数据库类型，并通过 DBContextOptionsBuilder 和 DBContextOptions 为 DBContext 注入连接字符串，如下所示：
```cs
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        _configuration = builder.Build();
        var cnstr = _configuration.GetConnectionString("InventoryManager");
        optionsBuilder.UseSqlServer(cnstr);
    }
}
```
最重要的是，在这个实现中，我们没有在运行时通过服务利用依赖注入的启动类或方法。因此，没有 DBContextOptions 被注入到 DBContext 中。为了解决这个问题，我们通过重写 OnConfiguring 方法来配置选项，如前所示。作为我们在本例中覆盖 OnConfiguring 方法并构建选项构建器的结果，如果我们需要实现任何其他自定义功能（例如添加拦截器或启用日志记录），我们还可以进一步配置 DBOptionsBuilder。  
通过这个检查我们还应该注意到，任何 DBContext 的创建都将使用 OnConfiguring 方法，因此我们可以继续修改 DBContext 的选项，即使系统正在利用依赖注入。  
作为覆盖 OnConfiguring 方法的替代方法，我们可以构建内联选项并将它们直接注入 DBContextOptions 的构造函数，如以下代码块所示（通过创建新的 ASP.Net MVC 项目很容易生成）：
```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
    services.AddDatabaseDeveloperPageExceptionFilter();
    services.AddDefaultIdentity<IdentityUser>(options =>
        options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();
    services.AddControllersWithViews();
}
```
此处需要注意的重要一点是，在 ASP.Net MVC 项目中，项目模板将 DBContextOptions 设置为使用 SQL Server，并通过名称利用配置条目来获取连接字符串。  
在前面两种情况下，我们都将数据库设置为使用 SQL Server。如果您的组织或项目无法利用 SQL Server，还有许多其他数据库选项可用。 

## Critical properties and methods available when working with the DBContext
### Important properties on the DbContextOptionsBuilder object
Table 4-1. Properties of the DbContextOptionsBuilder class  
| Property | Purpose |
|-|-|
| IsConfigured | 获取一个值，该值指示是否已配置任何选项。|
| Options | 通过授予对 DBContextOptions 对象的直接访问权限来获取正在配置的选项。|

### Important properties on the DBContextOptions object
Table 4-2. Properties of the DbContextOptions class.  
| Property | Purpose |
|-|-|
| ContextType | 获取上下文的类型；如果未定义类型，则返回 DBContext。|
| Extensions | 获取配置的扩展列表，例如所利用的数据库类型。|
| IsFrozen | 用于确定 DBContext 是否打开以进行进一步配置。如果为真，则系统无法进一步覆盖 OnConfiguring 方法中的上下文选项。|
