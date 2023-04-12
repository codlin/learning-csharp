/// <summary>
/// 空值运算符
/// C# 提供了 3 个运算符来简化处理空值操作：空值合并运算符、空合并赋值运算符 和 空条件运算符。 
/// </summary>
void NullCoalescingOperators()
{
    /// <summary>
    /// 空值合并运算符 ?? ，含义：如果左边的操作数是⾮空，给我；否则，给我另⼀个值
    /// 如果左边的表达式不为空，则永远不会计算右边的表达式。
    /// 空合并运算符也适⽤于可为空的值类型。 
    /// </summary>
    string? s1 = null;
    string s2 = s1 ?? "nothing"; // s2 evaluates to "nothing"
    Console.WriteLine(s2);
}
NullCoalescingOperators();

/// <summary>
/// 空合并赋值运算符
/// ??= 运算符（在 C# 8 中引⼊）是 null 合并赋值运算符。
/// 它表⽰，“如果左边的操作数为空，则将右边的操作数赋值给左边的操作数。”
/// </summary>
void NullCoalescingAssignmentOperator()
{
    Point p = new Point { X = 1, Y = 1 };
    Point? m = new Point { X = 2, Y = 2 };
    Point? t = null;
    t ??= p; // -> if (t == null) t = p;
    m ??= p;
    Console.WriteLine("{0}, {1}", t, m);
}
NullCoalescingAssignmentOperator();

/// <summary>
/// ?. 运算符是空条件运算符或“Elvis”运算符（猫王运算符）。
/// 它允许您像标准点运算符⼀样调⽤⽅法和访问成员，除了如果左边的操作数为空，则表达式的计算结果为空，
/// ⽽不是抛出 NullReferenceException异常
/// </summary>
void NullConditionalOperator()
{
    System.Text.StringBuilder? sb = null;
    string? s = sb?.ToString(); // -> string? s = (sb == null ? null : sb.ToString());

    // Null 条件表达式也适⽤于索引器
    string? foo = null;
    char? c = foo?[1]; // c is null

    // 遇到空值时，Elvis 运算符将其余部分短路表达
    s = sb?.ToString().ToUpper(); // s evaluates to null without error

    // 只有在每一个左侧的操作数都可能为null时才需要重复使用Elvis
    // x?.y?.z

    // 最终表达式需要接受空值
    // int len = sb?.ToString().Length; // Illegal : int cannot be null
    int? len = sb?.ToString().Length;

    // 可以将 null 条件运算符与常⽤的类型成员⼀起使⽤，包括⽅法、字段、属性和索引器。
    // 它还与空合并运算符结合得很好
    s = sb?.ToString() ?? "nothing"; // s evaluates to "nothing"
}
NullConditionalOperator();

struct Point
{
    public int X;
    public int Y;

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

class Bird
{
    public string Name;

    public Bird(string name)
    {
        Name = name;
    }
}