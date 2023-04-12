/// <summary>
/// 在外部名称空间中声明的名称可以在内部名称空间中不加限定地使⽤
/// </summary>
namespace Outer
{
    class Class1 { }
    namespace Inner
    {
        class Class2 : Class1 { }
    }
}

/// <summary>
/// 如果你想在命名空间层次结构的不同分⽀中引⽤⼀个类型，你可以使⽤部分限定的名称
/// </summary>
namespace MyTradingCompany
{
    namespace Common
    {
        class ReportBase { }
    }

    namespace ManagementReporting
    {
        class SalesReport : Common.ReportBase { }
    }
}

/// <summary>
/// 如果相同的类型名称同时出现在内部和外部命名空间中，则内部名胜出。
/// 要引⽤外部命名空间中的类型，您必须限定其名称。
/// 在编译时所有类型名称都转换为完全限定名称。中间语⾔ (IL) 代码不包含不限定或部分限定的名称。
/// </summary>
namespace Outer
{
    class Foo { }
    namespace Inner
    {
        class Foo { }
        class Test
        {
            Foo? f1; // = Outer.Inner.Foo
            Outer.Foo? f2; // = Outer.Foo
        }
    }
}

/// <summary>
/// 您可以重复命名空间声明，只要名称空间不冲突
/// </summary>
namespace Outer.Middle.Inner
{
    class Class4 { }
}
namespace Outer.Middle.Inner
{
    class Class5 { }
}

/// <summary>
/// 您可以在命名空间中嵌套 using 指令。这允许您在命名空间声明中确定使⽤指令的使⽤范围。
/// </summary>
namespace N1
{
    class Class1 { }
}
namespace N2
{
    using N1; // <-- HERE
    class Class2 : Class1 { }
}
namespace N2
{
    // class Class3 : Class1 { } // Compile-time error
}