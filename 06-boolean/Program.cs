/// <summary>
/// C# 的 bool 类型（别名为 System.Boolean 类型）是⼀个逻辑值，可以通过字⾯值 true 或 false赋值。
/// 尽管布尔值只需要⼀位存储，但运⾏时将使⽤⼀个字节的内存空间，因为这是运⾏时和处理器可以⾼效访问和
/// ⼯作的最⼩块。为了避免数组的空间效率低下，.NET在 System.Collections 命名空间中提供⼀个 BitArray 类，
/// 旨在每个布尔值只使⽤⼀位。
/// </summary>
void BooleanTypeAndOperators()
{
    // 不能从 bool 类型转换为数字类型，或者反之亦然。
    bool a = false;
    Console.WriteLine(a);
    // int b = (int)a; // Cannot convert type 'bool' to 'int' [boolean]
}

void EqualityAndComparisonOperators()
{
    int x = 1;
    int y = 2;
    int z = 1;
    Console.WriteLine(x == y); // False
    Console.WriteLine(x == z); // True

    // 对于引⽤类型，默认情况下，相等性基于引⽤，⽽不是底层对象的实际值
    Dude d1 = new Dude("John");
    Dude d2 = new Dude("John");
    Console.WriteLine(d1 == d2); // False
    Dude d3 = d1;
    Console.WriteLine(d1 == d3); // True
}

void ConditionalOperators()
{
    bool a = UseUmbrella(true, false, false);
    Console.WriteLine(a); // True

    // 三元运算符
    Console.WriteLine(Max(3, 5));
}

BooleanTypeAndOperators();
EqualityAndComparisonOperators();
ConditionalOperators();

static bool UseUmbrella(bool rainy, bool sunny, bool windy)
{
    return !windy && (rainy || sunny);
}

static int Max(int a, int b)
{
    return (a > b) ? a : b;
}

public class Dude
{
    public string Name;
    public Dude(string n) { Name = n; }
}
