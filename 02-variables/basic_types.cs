namespace BasicTypes;

// C# 的预定义类型：数字、字符串、布尔
// C# 的自定义类型：类、结构、接口、枚举、元组、记录等等
/* 所有C# 类型都属于以下类型：
    1. 值类型
    2. 引用类型
    3. 泛型类型
    4. 指针类型
*/
// <值类型> 包括⼤多数内置类型（特别是所有数字类型、char 类型和 bool 类型）以及⾃定义结构和枚举类型。
// <引⽤类型> 包括所有类、数组、委托和接⼝类型（这包括预定义的字符串类型）等。 

class BasicTypesTest
{
    public void Create()
    {
        var p1 = new Point();
        Console.WriteLine($"{p1.X}, {p1.Y}");

        Point p2 = new Point { X = 1, Y = 2 };
        Console.WriteLine($"{p2.X}, {p2.Y}");

        Point p3 = new();
        Console.WriteLine($"{p3.X}, {p3.Y}");

        // Point p4 = new(3, 4);          // compiling error
        // Point p5 = new(X = 5, Y = 6);  // compiling error
    }

    // 值类型放在栈上，复制产生新的值
    public void ValueCopy()
    {
        var p1 = new Point { X = 1, Y = 2 };
        Console.WriteLine($"p1: ({p1.X}, {p1.Y})");

        Point p2 = p1;
        Console.WriteLine($"p2: ({p2.X}, {p2.Y})");

        p2.X = 3;
        p2.Y = 4;

        Console.WriteLine($"p1: ({p1.X}, {p1.Y})");
        Console.WriteLine($"p2: ({p2.X}, {p2.Y})");
    }

    /// <summary>
    /// 引⽤类型⽐值类型更复杂，有两部分：对象和对该对象的引⽤。
    /// 把引用类型变量赋值给另一个变量，会复制引用而不是复制对象本身。
    /// 可以为引⽤分配字⾯值 null，表⽰引⽤指向没有对象
    /// </summary>
    public void ReferenceCopy()
    {
        Bird a = new Bird("Polly");
        Console.WriteLine($"Bird Name: {a.Name}");

        Bird? b = a;
        b.Name = "Totiy";
        Console.WriteLine($"Bird Name: {a.Name}");

        b = null;
    }

    /// <summary>
    /// C# 可以在兼容类型的实例之间进行转换。 
    /// 转换总是从现有值创造新值。
    /// 转换可以是隐式的，也可以是显式的。隐式转换自动发生，显式转换需要强制转换。
    /// 编译器不能保证转换总是成功
    /// 转换过程中可能会有信息丢失 
    /// </summary>
    public void Conversion()
    {
        // 整型数值之间的转换
        int a = 12345678;
        Console.WriteLine($"int 类型： a = {a}");

        short b = (short)a; // 截断
        Console.WriteLine($"从 int 类型到 short 的强制转换产生了数值截断： b = {b}");

        int c = b;
        Console.WriteLine($"从 short 到 int 的隐式类型转换： c = {c}");

        // 整型 和 浮点型 的转换
        float f = 3.1415926f;
        Console.WriteLine("float 类型： f = {0}", f);

        f = (float)a;
        Console.WriteLine("从 int 到 float 的隐式类型转换： f = {0:F2}", f);

        double d = 123456.78d;
        f = (float)d;
        Console.WriteLine($"从 double 到 float 需要执行强制类型转换： f = {f}");
    }
}


struct Point
{
    public int X;
    public int Y;
}

class Bird
{
    public string Name;

    public Bird(string name)
    {
        Name = name;
    }
}