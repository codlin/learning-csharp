/// <summary>
/// 参数的传递
/// 函数参数的传递分为：
/// 1. 按值传递
/// 2. 按引用传递 
/// 可以通过对参数使用关键字ref、in、out 来控制引用传递的方式。
/// 参数可以通过引⽤或值传递，和参数是引⽤类型还是值类型⽆关。
/// </summary>

// 按值传递 1：值类型作为参数，传递的是值本身，值会复制给参数变量
void PassParamsByValues(Point p)
{
    p.X += 1;
    p.Y += 1;
    Console.WriteLine($"point({p.X}, {p.Y})");
}
var p = new Point { X = 1, Y = 1 };
PassParamsByValues(p);
Console.WriteLine($"point({p.X}, {p.Y})");

// 按值传递 2：引用类型作为参数，传递的是指向对象的引用，引用会复制给参数变量，相当于C语言的复制指针
void PassRefType(Bird? bird)
{
    Console.WriteLine(bird?.Name);
    bird!.Name = "InPassRefType";
    bird = null;
}
var bird = new Bird("Polly");
PassRefType(bird);
Console.WriteLine(bird.Name);

// 按引用传递 1：使用 ref 修饰符传递参数，相当于直接引用了传入的argument本身，不会产生复制，相当于 C++ 的 & 引用
void PassParamsByRef(ref Bird? bird)
{
    Console.WriteLine(bird?.Name);
    bird!.Name = "PassTypeByRef";
    Console.WriteLine(bird.Name);

    bird = null;
}
PassParamsByRef(ref bird);
Console.WriteLine(bird == null); // True

void PassParamsByRef2(ref Point p)
{
    p.X += 2;
    p.Y += 2;
    Console.WriteLine($"point({p.X}, {p.Y})");
}
PassParamsByRef2(ref p);
Console.WriteLine($"point({p.X}, {p.Y})");

// 按引用传递 2：使用 in 修饰符传递参数，属于 ref 引用传递，但是参数在函数内部不能被修改
void PassParamsByRefIn(in Bird bird)
{
    bird.Name = "引用类型的参数使用in，不能修改它指向其它对象，但是其本身内容可以被修改。";
    // bird = null; // <-- 不能修改bird指向为其它对象
}
bird = new Bird("Totity");
PassParamsByRefIn(bird);
Console.WriteLine(bird.Name);

// in 在将大的值类型传递给方法时很有用，可以避免值复制的开销，同时保证原始值不被修改
void PassParamsByRefIn2(in Point p)
{
    /// <summary>
    /// 和引用类型的参数不同，值类型的参数在用了 in 关键字时，除了不能被指向其它变量外，其本身的值也不能被修改
    /// </summary>
    // p.X += 2; // <-- 编译错误，X值不能被修改
    Console.WriteLine("和引用类型的参数不同，值类型的参数在用了 in 关键字时，除了不能被指向其它变量外，其本身的值也不能被修改");
}
PassParamsByRefIn2(p);

// 按引用传递 3：使用 out 修饰符传递参数类似于ref，但有以下特点：
// 1. 在进入函数之前不需要分配对象作为argument
// 2. 它必须在它从函数出来之前被赋值
// out 修饰符最常用的是从一个方法返回多个值
void PassParamsByRefOut(in Point p, out Point p1, out Point p2)
{
    p1 = p;
    p1.X += 1;
    p1.Y += 1;

    p2 = p;
    p2.X += 10;
    p2.Y += 10;
}
PassParamsByRefOut(p, out Point p1, out Point p2);
Console.WriteLine($"p1({p1.X}, {p1.Y}), p2({p2.X}, {p2.Y})");

/// <summary>
/// params修饰符，如果应用于方法的最后一个参数，允许方法接受任意数量的特定类型的参数。
/// 参数必须声明为一维数组。 
/// </summary>
/// <param name="ints"></param>
/// <returns></returns>
int Sum(params int[] ints)
{
    int sum = 0;
    for (int i = 0; i < ints.Length; i++)
        sum += ints[i]; // Increase sum by ints[i]
    return sum;
}
int total = Sum(1, 2, 3, 4);
Console.WriteLine(total); // 10
// The call to Sum above is equivalent to:
int total2 = Sum(new int[] { 1, 2, 3, 4 });
Console.WriteLine(total2); // 10

/// <summary>
//// 如果可选参数在其位置指明了默认值，则该参数是可选的，它需要放在非可选参数之后，但是在params参数之前，paprams始终放在最后。
/// </summary>
int OptionalParams(int x, int y = 1, params int[] ints)
{
    int sum = Sum(ints);
    return sum + x + y;
}
Console.WriteLine(OptionalParams(1));
Console.WriteLine(OptionalParams(1, 2));
Console.WriteLine(OptionalParams(1, 2, 3, 4, 5));
// 可以通过命名来识别参数，而不是通过顺序，命名参数可以以任何顺序出现
OptionalParams(y: 2, ints: new int[] { 3, 4, 5 }, x: 1);
// 可以混合命名参数和位置参数，但是限制是位置参数必须出现在命名参数之前，除非它们用在正确的位置
OptionalParams(x: 1, 2);
OptionalParams(1, y: 2);
// OptionalParams(y: 2, 1); // <-- error

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