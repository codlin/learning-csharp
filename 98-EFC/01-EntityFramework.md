# Introduction to Entity Framework
## Entity Framework and LINQ
EF 和 LINQ 不仅使我们能够使用 C# 或 VB.Net 代码处理我们的数据库对象，而且使我们能够直接在代码中定义数据库结构。使用代码而不是传统的 SQL 脚本来创建数据库对象被称为在**代码优先数据库方法**中工作。  
能够在内存中定义和使用对数据库对象建模的对象，同时还可以直接跟踪对数据库的更改，这是一场非常强大的革命。对于我们这些习惯于处理断开连接的数据的人来说，直接跟踪内存中的变化也会使我们对并发问题的理解达到一个新的水平。从断开连接到连接数据的这种转变是一件非常好的事情，即使这是一个稍微痛苦的转变。此外，EF 提供了以断开连接的方式轻松处理数据的能力，这也是一个有效且有价值的选项。我们将在本文中检查如何处理断开连接和连接的数据。  
虽然 EF 和 LINQ 是一些更重要的数据库工具，随着 .Net Framework 的每次迭代，我们都可以使用这些工具，但除了这些语言和范例的变化之外，还有更多的事情在发生。最终，新 CEO 的上任将开始让微软走上一条完全不同的道路。

## A new direction and a new visionary leader
2014 年初，微软任命了萨蒂亚·纳德拉 (Satya Nadella) 为新任首席执行官。纳德拉先生让微软走上了一条过渡路线，这在一定程度上震惊了开发者社区。几乎在上任后，他就简单地宣布将采用 Linux 操作系统（在当时被视为 Microsoft Windows 的直接竞争对手）。之后，微软迅速开始发布工具，这些工具不仅可以在 Windows 上运行，还可以在 Mac 和 Linux 计算机等其他平台上运行。虽然这些最初的步骤是微软标准操作程序的革命性变化，但接下来发生的事情完全出乎意料。 
### Microsoft goes all in for all developers
2016 年底，微软宣布 .Net 将开源。这意味着在未来，开发人员日常使用的所有工具和代码都可以直接扩展，并开放给全世界的扩展建议。任何有想法的开发人员都可以创建拉取请求，并要求将他们的更改直接实施到 .Net Framework 的一些基础库中。从那时起，Microsoft 和 .Net Framework 不再是一个不透明的操作，所有开发都在闭门造车。从那天起，Microsoft 的运作方式一直是全面和有意识地与整个开发人员社区接触，而不仅仅是它的核心 .Net 开发人员。

### A new vision requires a new path
使 .Net 开源是一个非常具有战略意义并且可以说是非常成功的决定。然而，随着巨大的变化，通常会产生对新工具和新流程的巨大需求。成为一种开源语言是不够的。同样明显的是，代码本身，就像微软最近发布的一些工具一样，也必须在任何平台上运行。也许正是由于这些变化，您才发现自己正在阅读这本书。为了使编写的代码能够在任何操作系统的任何服务器上运行，甚至在像 Docker 这样的容器框架中，.Net 框架必须独立于任何特定于 Windows 的 API 调用。虽然可以在 Linux 或 Mac 上的 Mono 或 Xamarin 等平台上运行编译的 .Net 代码，但使用 .Net Framework 根本不可能直接开发、编译和执行代码。因此，伴随着微软要开源的消息发布，“Core”平台也随之发布。在 .Net Core 1.0 的原始版本中，还引入了一种称为 .Net 标准库的新类库类型。  
.Net Core 的初始版本旨在供 Web 开发人员使用，特别是那些使用 .Net MVC Web 开发框架的开发人员。由于框架所能做的事情的局限性，以及整体变化并不是非常有利可图，.Net 开发人员和组织最初采用 .Net Core 平台的速度相当缓慢。  
随着核心平台 2.0 的主要发布，.Net Core 的采用开始增加。然而，.Net Core 的最终版本 3.1 不仅为 Web 开发打开了大门，还加速了所有项目向 .Net Core 的迁移，而不仅仅是 Web 开发项目。  
微软这条新路径的另一个副作用是这些变化对实体框架方向的影响。随着将 .Net Framework 重写为 .Net Core，出现了一个新的 EF，也称为 Entity Framework Core。因此，在撰写本文时以及在接下来的几年可预见的未来，将至少有两个活跃的 EF 版本在起作用，即 EF6 和 EFCore（EFCore 有多个可能仍然活跃的版本） 。

### What is .Net 5 and why is Entity Framework called EFCore5 instead of EF5, and why are we already on .Net 6 and EFCore6
2020 年 11 月，.Net 5 正式发布。 .Net 5 是两条路径融合的最终结果。原始的 .Net Framework 和新的 .Net Core 架构全部归于一处。与其将其称为 .Net Core Framework 5，更简单的解决方案是重新命名为 .Net 5。但是，不要太自在，因为 .Net 6 和 .Net 7 已在未来几年的计划中。所有这一切都表明，在撰写本文时的最新版本中，.Net Core 和 .Net Framework 现在都合并到 .Net 5 和 EFCore5 中，下一版本即将推出 —— .Net 6 和EFCore6。    
如前所述，EF 的原始版本是通过 EF6 发布的。默认情况下，这意味着已经有一个 EF5 版本（发生在 2012 年）和一个 EF6 版本发生在 2013 年。因此，使用与已创建版本相同的名称来调用新版本的 EF 可能会造成很多混乱，目前仍在大量使用。由于这些原因，我猜测唯一的选择是在新版本的名称中保留 Core 。但是，随着 .Net 6 和 7 的计划，一旦原始版本不再与 .Net 的活动版本冲突，如果在未来某个时候正式名称恢复为 EF6.5 或 EF7 之类的东西，请不要感到惊讶。

### The state of the union
尽管 EF6 的新功能已达到生命周期结束，但对 EF6 的支持可能会持续到 2029 年初。此外，.Net Core 3.1 的生命周期也可能会持续到 2030 年左右。对于大多数应用程序在撰写本文时，在现实世界中使用实体框架的非核心应用程序以及未来在 .Net 5 堆栈中编写的现实世界中的大多数应用程序，理解和了解两者这些框架（EF6 和 EFCore）对未来五到十年内的发展将非常重要。  
好消息是，在大多数情况下，这两个框架都在做同样的事情，并使用相同的架构概念实现相同的目标。坏消息是，这些框架在处理命令、它们如何处理代码优先迁移以及处理遗留对象（如 EDMX 文件（仅存在于旧版本的 EF 中））方面有些不同。此外，这两个版本的效率水平存在许多差异，EFCore 的性能通常优于 EF6。  
如前所述，本书将主要关注使用 EFCore6。目前，EF6、EFCore3.1 和 EFCore5 项目都应该在 .Net 5 或 .Net 6 项目中工作，但 .Net 6 版本中的新项目应该以EFCore6 为目标。此外，本书将涵盖的大部分使用 EFCore6 的内容也适用于以前版本的 EF。如果代码在以前的版本中无法运行，本书将尽一切努力指出代码仅在最新版本的 EFCore6 中运行的事实。

## The future
随着 .Net 不断发布并向未来发展，我希望我们在本文中涵盖的所有内容在未来几年仍然适用。此外，如前所述，我们在本文中涵盖的大部分内容确实会转换回 EF6 EFCore3 或 EFCore5 项目中使用。最后，我将尽最大努力使与本书相关的任何资源保持最新，只要这样做是有意义的。

## Activity 1-1: Getting started with EFCore6
在任何项目中，您都可以轻松设置实体框架 (EF)。但是，在执行此操作之前，作为开发人员/架构师要问自己一个很好的问题是数据库操作是否可能需要跨多个解决方案或项目使用。如果是这种情况，要跨多个解决方案或项目使用 EF，最好的方法是创建一个可重用的代码库来存储您的数据库代码，包括您的上下文、配置和迁移。无论使用单独的库还是仅包含在单个包中，初始设置都将完全相同，即使用 NuGet 将库引入您的解决方案或项目中。由于使用单独的库是一种更可靠且可重用的方法，因此在第一个活动中，您将了解如何构建一个利用 EFCore6 的可重用数据库库。您将从查看新建项目开始，然后继续导入实体框架库。

### Task 1: Create a new project and add the EF packages
使用vscode创建 project，命令：
```shell
dotnet new classlib -n EFCore_DBLibrary -o ./01-EFC/EFCore_DBLibrary -f net6.0
```
OmniSharp选择当项目为当前项目。

### Task 2: Add the EFCore6 packages to your project
转到 project 所在目录：
```shell
cd ./01-EFC/EFCore_DBLibrary
```
添加包：
```shell
dotnet add package Microsoft.EntityFrameworkCore --version 7.0.5
```
Create a DBContext
重命名 `Class1.cs` 为 `ApplicationDbContext.cs`

Alter your context to implement DbContext correctly:

