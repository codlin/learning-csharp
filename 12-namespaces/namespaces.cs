/// <summary>
/// namespace 关键字为该块中的类型定义了⼀个命名空间
/// 命名空间中的点表⽰嵌套命名空间的层次结构
/// </summary>
namespace Outer.Middle.Inner
{
    class Class1 { }
    class Class2 { }
}
// 相当于
namespace Outer
{
    namespace Middle
    {
        namespace Inner
        {
            class Class3 { }
        }
    }
}

/// <summary>
/// 一个文件中可以包含多个命名空间，一个命名空间也可以跨越多个文件: animal_namespace.cs
/// </summary>
namespace Animal.Zone.Earth
{
    class Cat { }
    class Dog { }
}

