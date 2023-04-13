/// <summary>
/// class 之前可以放的修饰符: 
/// 1. public, internal, 
/// 2. abstract, sealed, 
/// 3. static 
/// 4. partial
/// 5. unsafe
/// 不能放这些修饰符：private, protected, protected internal, or private protected 
/// 可以组合部分修饰符，其中 static 不能和 sealed，abstract 组合 
/// 任何合法的类前面都可以用 partial 修饰
/// </summary>
///
// public 修饰的类对程序集外开放
public class PubClass { }

// internal 修饰的类只对程序集内开放
internal class InternalClass { }

// private class PrivateClass { }
// protected class PrivateClass { }

// abstract 修饰的类是抽象类, 不能被实例化
abstract class AbstractClass { }

// sealed 修饰的类不能被继承
sealed class SealedClass { }

// static 标记类为静态类
static class StaticClass
{
    /// <summary>
    /// 标记为静态的类不能被实例化或⼦类化，且必须仅由静态成员组成。
    /// </summary>

    static string Message = "Hello";
}

/// <summary>
/// partial 修饰符表明此类为部分类, 该类可能还有其它地方定义。
/// 部分类的每个参与者必须有partial声明。
/// 部分类还可以有部分方法，每个部分方法的参与者必须都存在于每个部分类中。
/// 如果该部分方法没有被实现，则会被编译器在编译时去掉。
/// 部分方法适合于代码自动生成的情景，定义由代码生成器生成，实现通常是手动编写。
/// 
/// </summary>
partial class PartialClass
{
    partial void ValidatePayment(decimal amount) { }

    // 如果部分⽅法声明以可访问性修饰符开头，则它是可扩展的
    // 可访问性修饰符的存在不仅仅影响可访问性：它告诉编译器以不同⽅式处理声明
    // 扩展的部分⽅法必须有实现；如果未实施它们不会消融
    // 因为它们不能消失，扩展的部分⽅法可以返回任何类型，也可以包括out参数
    public partial bool IsValid(string identifier) { return false; }
    internal partial bool TryParse(string number, out int result) { result = 1; return true; }
}

// unsafe class UnsafeClass { } // <- 待补充用法

// 可以将这些修饰符进行组合, 其中 static 不能和 sealed/abstract 组合
public abstract class PubAbsClass { }
internal abstract class InternalAbsClass { }
public sealed class PubSealedClass { }
internal sealed class InternalSealedClass { }
public static class PubStaticClass { }
// internal static sealed class InternalStaticSealedClass { }
// internal static abstract class InternalStaticAbsClass { }

/// <summary>
/// 可以接在 class 后面的元素包括：
/// 泛型参数和约束、基类和接口
/// </summary>
class GenericClass<T> { }
class GenericClass2<T> where T : PubClass { }
class SubClass : PubAbsClass { }
interface IAnimal { }
class Bird : IAnimal { }

/// <summary>
/// class 的大括号内可以包含如下内容：
/// 字段、属性、索引器、方法、事件、构造函数、终结器、重载运算符和嵌套类型
///
/// </summary>
class Octopus
{
    /// <summary>
    /// 字段 允许以下修饰符：
    /// 静态修饰符：static
    /// 访问修饰符：public, internal, protected, private，以及组合 internal protected 和 private protected
    /// 继承修饰符：new
    /// 不安全的代码修饰符：unsafe
    /// 只读修饰符：readonly
    /// 线程修饰符：volatile
    /// 默认访问修饰符是 private
    /// </summary>
    // private 访问仅限于它的包含类 
    string name = "";

    // public 访问不受限制
    public int Age = 0;

    /// <summary>
    /// internal protected 和 private protected 提供了横向和纵向的可访问控制
    /// 其中 internal 横向扩展到程序集内，protected 纵向限制为派生类，private 则横向和纵向限制为自身
    /// </summary>
    // 仅限于当前程序集或派生自包含类的类型
    internal protected string Title = "Engineer";

    /// <summary>
    /// 属性
    /// 属性允许以下修饰符：
    /// 静态修饰符 static
    /// 访问修饰符 public internal private protected
    /// 继承修饰符 new virtual abstract override sealed
    /// 属性可以有只读属性、可读写属性、init-only属性，以及自动属性
    /// </summary>
    // 可读写属性 
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    // 只读属性
    public int Legs { get { return legs; } }
    public int Maximum { get; } = 999;

    /// <summary>
    /// 自动属性
    /// 属性最常⻅的实现是⼀个简单的 getter 和/或 setter读取和写⼊与属性相同类型的私有字段。
    /// ⼀个⾃动属性声明指⽰编译器提供此实现。
    /// 编译器会⾃动⽣成私有字段，但我们⽆法引⽤它的名字。 
    /// set 访问器可以标记为private或者protected如果你想将属性以只读⽅式公开给其他类型。
    /// </summary>
    /// <value></value>
    public decimal CurrentPrice { get; set; }

    // init-only 属性
    public int Eyes { get; init; } = 3;
    public string Gender { get => gender; init => gender = value; }

    // 仅限于包含类或当前程序集中派生自包含类的类型
    private protected string gender = "male";

    //readonly 修饰符防⽌字段在构造后被修改。 只读字段只能在其声明或封闭类型的构造函数中赋值。
    static readonly int legs = 8, eyes = 2;

    // unsafe // <-- 待补充

    /// <summary>
    /// 字段初始化是可选的。未初始化的字段具有默认值 (0, '\0', null，false）。
    /// -> 字段初始化器在构造函数之前运⾏ <-
    /// 字段初始化器可以包含表达式和调⽤⽅法
    /// </summary>
    public int Size;
    string? color;
    static readonly string TempFolder = System.IO.Path.GetTempPath();

    // 常量⽤ const 关键字声明，并且必须⽤⼀个值初始化。
    public const string Message = "Hello World";

    // 常量也不同于静态只读字段，因为对常量发⽣在编译时；
    // 相反，静态只读字段的值在程序运⾏时每次都可能不同：
    static readonly DateTime StartupTime = DateTime.Now;

    /// <summary>
    /// 构造函数
    /// 实例构造函数允许使⽤以下修饰符：
    /// 访问修饰符      public, internal, private, protected
    /// ⾮托管代码修饰符 unsafe, extern
    /// -> 字段初始化发⽣在构造函数执⾏之前，并且按照字段的声明顺序 <-
    /// 构造函数不需要公开。拥有⾮公共构造函数的常⻅原因是通过静态⽅法调⽤来控制实例创建。
    /// 静态⽅法可⽤于从池中返回对象⽽不是创建新对象，或者根据输⼊参数返回各种⼦类。
    /// </summary>
    // 隐式⽆参数构造函数：当且仅当您不定义任何构造函数时C# 编译器会⾃动⽣成⼀个⽆参数的公共构造函数。
    // 然⽽，⼀旦你⾄少定义了⼀个构造函数，⽆参构造函数不再⾃动⽣成 
    public Octopus() { }
    // 重载构造函数
    public Octopus(string name) { this.name = name; }
    // 为了避免代码重复，⼀个构造函数可以调⽤另⼀个，使⽤ this 关键字
    public Octopus(string name, int age) : this(name) { this.Age = age; }

    /// <summary>
    /// 解构函数 Deconstructors
    /// 解构器（也称为析构⽅法）充当近似构造函数的对⽴⾯：⽽构造函数通常采⽤⼀组值（如
    /// 参数）并将它们分配给字段，解构函数执⾏相反的操作并分配字段返回⼀组变量。
    /// ⼀个解构⽅法必须被称为 Deconstruct 并且有⼀个或多个 out参数
    /// </summary>
    public void Deconstruct(out string name, out int age)
    {
        name = this.name;
        age = this.Age;
    }

    /// <summary>
    /// 终结器 Finalizers
    /// 终结器finalizers是仅限类(class-only)的⽅法，它在垃圾收集器回收该对象之前执⾏。
    /// 终结器允许使⽤以下修饰符：
    /// ⾮托管代码修饰符  unsafe
    /// </summary>
    ~Octopus() { }

    /// <summary>
    /// ⽅法允许使⽤以下修饰符：
    /// 静态修饰符        static
    /// 访问修饰符        public, internal, private, protected
    /// 继承修饰符        new, virtual, abstract, override, sealed
    /// 部分⽅法修饰符     partial
    /// ⾮托管代码修饰符   unsafe, extern
    /// 异步代码修饰符     async
    /// </summary>
    public void SetColor() { }

    // 表达式方法
    int Foo(int x) => x * 2;

    // 重载方法
    void Foo(double x) { }
    void Foo(int x, float y) { }
    void Foo(float x, int y) { }

    // 局部⽅法 Local methods
    void WriteCubes()
    {
        Console.WriteLine(Cube(3));
        Console.WriteLine(Cube(4));
        Console.WriteLine(Cube(5));

        int Cube(int value) => value * value * value;
    }

    /// <summary>
    /// 索引器
    /// </summary>
    string[] words = "The quick brown fox".Split();
    public string this[int wordNum] // indexer
    {
        get { return words[wordNum]; }
        set { words[wordNum] = value; }
    }
}


class Rectangle
{
    public readonly float Width, Height;
    public Rectangle(float width, float height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// 静态构造器
    /// 静态构造函数对每个类型执⾏⼀次，⽽不是对每个实例执⾏⼀次。 
    /// ⼀个类型只能定义⼀个静态构造函数，并且必须是⽆参数的，并且和类型具有相同的名称。
    /// 运⾏时会在类型被使⽤之前⾃动调⽤静态构造函数。有两件事触发这个：
    /// - 实例化类型
    /// - 访问类型中的静态成员
    /// 静态构造函数允许的唯⼀修饰符是 unsafe 和 extern 。
    /// 如果静态构造函数抛出未处理的异常，那么该类型在应⽤程序的⽣命周期将变得不可⽤
    /// </summary>
    static Rectangle() { Console.WriteLine("Type Initialized"); }

    /// <summary>
    /// 从 C# 9 开始，您还可以定义模块初始值设定项module initializers，它会每个程序集执⾏⼀次（第⼀次加载程序集时）。
    /// 要定义模块初始值设定项，请编写静态 void ⽅法然后将[ModuleInitializer] 属性应⽤于该⽅法。
    /// </summary>
    [System.Runtime.CompilerServices.ModuleInitializer]
    internal static void InitAssembly() { }

    /// <summary>
    /// 解构函数 Deconstructors
    /// 解构器（也称为析构⽅法）充当近似构造函数的对⽴⾯：⽽构造函数通常采⽤⼀组值（如
    /// 参数）并将它们分配给字段，解构函数执⾏相反的操作并分配字段返回⼀组变量。
    /// ⼀个解构⽅法必须被称为 Deconstruct 并且有⼀个或多个 out参数， 
    /// </summary>
    public void Deconstruct(out float width, out float height)
    {
        width = Width;
        height = Height;
    }
}

/// <summary>
/// 静态构造函数和字段初始化顺序
/// 静态字段初始值设定项在静态构造函数之前运⾏。
/// 如果⼀个类型没有静态构造函数，静态字段初始化器将在类型被使⽤之前执⾏——或在运⾏时⼼⾎来潮时的任何更早的时间（？）。
/// 静态字段初始值设定项按字段声明的顺序运⾏。
/// </summary>
class Foo
{
    public static int X = Y; // 0
    public static int Y = 3; // 3
}

class Foo2
{
    // 先执行 Instance 的初始化，所以会先执行构造函数，这是打印 X 为 0
    public static Foo2 Instance = new Foo2();
    public static int X = 3;
    Foo2() => Console.WriteLine(X); // 0
}
