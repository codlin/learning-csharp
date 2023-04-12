/// <summary>
/// 命名空间是类型名称的域。类型通常组织成层级名空间，使它们更容易找到并防⽌冲突。
/// 命名空间独⽴于程序集，程序集是以 .dll ⽂件作为部署单位。
/// 命名空间对成员可⻅性也没有影响——public、internal、private 等。
/// 一个文件中可以包含多个命名空间，一个命名空间也可以跨越多个文件
/// </summary>

// using 指令导⼊⼀个命名空间，允许您引⽤类型⽽⽆需他们的完全限定名称
using Outer.Middle.Inner;
using Animal.Zone.Earth;

void Namespaces()
{
    Class1 c = new();
    Console.WriteLine(c);

    // 可以使⽤其完全限定名称引⽤类型，其中包括从最外层到最内层的所有命名空间
    Outer.Middle.Inner.Class2 d = new();
    Console.WriteLine(d);

    // Animal.Zone.Earth 命名空间跨越了不同的文件
    Dog dog = new();
    Panda panda = new();
    Lion lion = new("ababa");
}
Namespaces();

/// <summary>
/// 从 C# 10 开始，如果您在 using 指令前加上 global 关键字，该指令将应⽤于项⽬或编译单元中的所有⽂件：
/// global using System;
/// global using System.Collection.Generic;
/// 这使您可以集中常⻅的导⼊并避免在中重复相同的指令每个⽂件。
/// 全局使⽤指令必须在⾮全局指令之前并且不能出现内部命名空间声明。 global 指令可以与 using static ⼀起使⽤。
/// 从 .NET 6 开始，项⽬⽂件允许隐式全局使⽤指令。如果ImplicitUsings 元素在项⽬⽂件中设置为 true（新的默
/// 认值projects），会⾃动导⼊以下命名空间：
/// System
/// System.Collections.Generic
/// System.IO
/// System.Linq
/// System.Net.Http
/// System.Threading
/// System.Threading.Tasks
/// </summary>



