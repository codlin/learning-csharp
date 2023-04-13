var rect = new Rectangle(3, 4);
(float width, float height) = rect; // Deconstruction
Console.WriteLine(width + " " + height); // 3 4

// 对象初始化器 Object Initializers
Octopus oc = new Octopus("abd", 8) { Size = 3 };

Console.WriteLine(Foo2.X); // 3

/// <summary>
/// nameof 运算符以字符串的形式返回任何符号的名称（类型、成员、变量和依此类推）
/// </summary>
void NameofOperator()
{
    string name = nameof(PartialClass);
    Console.WriteLine(name);

    name = nameof(System.Text.StringBuilder.Length);
    Console.WriteLine(name);
}
NameofOperator();