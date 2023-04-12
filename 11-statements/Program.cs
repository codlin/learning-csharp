using System.Text;
/// <summary>
/// 局部变量或局部常量的范围扩展到整个当前块，你不能在当前块或在任何嵌套块中声明另一个具有同名的局部变量。
/// </summary>
void DeclarationStatements()
{
    const double pi = 3.1415926;
    string someWord = "hello";
    int someNumber = 42;
    Console.WriteLine($"{someWord}, {someNumber}, {pi}");

    if (someNumber > 10)
    {
        // 不能声明和封闭块同样名称的变量，即 C# 不支持作用域覆盖
        // string someWord = "world"; // <- ERROR
        const int age = 18;
        if (age > 18)
        {
            // 不能声明和封闭块同样名称的变量，即 C# 不支持作用域覆盖
            // string someWord = "adult"; // <- ERROR
        }
    }
    else
    {
        // 在并列级别的块中可以定义同样的变量
        const int age = 18;
        if (age > 0)
        {
            Console.WriteLine("在并列级别的块中可以定义同样的变量");
        }
    }

    int i = 0;
    // 变量 i 冲突，for语句声明的变量作用域虽然在循环语句内，但是不能和其封闭类型的变量重名
    // for (int i = 0; i < 3; i++) // <- ERROR 
    for (i = 0; i < 3; i++)
        Console.WriteLine(i);
}
DeclarationStatements();

/// <summary>
/// ⼀种表达语句必须要么改变状态要么调⽤可能改变状态的东⻄。改变状态本质上意味着改变⼀个变量。
/// 以下是可能的表达式语句：
/// 1. 赋值表达式（包括递增和递减表达式）
/// 2. ⽅法调⽤表达式（⽆效和⾮⽆效）
/// 3. 对象实例化表达式
/// </summary>
void ExpressionStatements()
{
    int x, y;
    System.Text.StringBuilder sb;

    // Expression statements
    x = 1 + 2; // Assignment expression
    x++;       // Increment expression
    y = Math.Max(x, 5);   // Assignment expression
    Console.WriteLine(y); // Method call expression

    sb = new StringBuilder(); // Assignment expression

    //当您调⽤构造函数或⽅法返回值⼀个值时，您没有义务使⽤这个返回值。
    // 但是，除⾮构造函数或⽅法改变状态，否则语句完全没⽤：
    new StringBuilder();      // Legal, but useless
    x.Equals(y); // Legal, but useless
}
ExpressionStatements();

void IfElsestatement(int a)
{
    if (a < 0)
    {
        Console.WriteLine($"{a} < 0");
    }
    else if (a < 18)
    {
        Console.WriteLine($"{a} < 18");
    }
    else
    {
        Console.WriteLine($"{a}");
    }
}
IfElsestatement(-1);
IfElsestatement(8);
IfElsestatement(28);

void SwitchStatement()
{
    ShowCard(-1);
    ShowCard(10);
    ShowCard(11);
    ShowCard(12);
    ShowCard(13);

    // Switching on types
    TellMeTheType(12);
    TellMeTheType("hello");
    TellMeTheType(3.14);
    SwitchOnTypes(true);
    SwitchOnTypes(false);
    SwitchOnTypes(1001);
    SwitchOnTypes(1001.00);

    SwitchExpressions();
}

void ShowCard(int cardNumber)
{
    switch (cardNumber)
    {
        case 13:
            Console.WriteLine("King");
            break;
        case 12:
            Console.WriteLine("Queen");
            break;
        case 11:
            Console.WriteLine("Jack");
            break;
        case -1: // Joker is -1
            goto case 12; // In this game joker counts as queen
        default: // Executes for any other cardNumber
            Console.WriteLine(cardNumber);
            break;
    }
}

/// <summary>
/// Switching on types
/// </summary>
void TellMeTheType(object x) // object allows any type.
{
    switch (x)
    {
        case int i:
            Console.WriteLine("It's an int!");
            Console.WriteLine($"The square of {i} is {i * i}");
            break;
        case string s:
            Console.WriteLine("It's a string");
            Console.WriteLine($"The length of {s} is {s.Length}");
            break;
        default:
            Console.WriteLine("I don't know what x is");
            break;
    }
}

/// <summary>
/// Switching on types
/// </summary>
void SwitchOnTypes(object x)
{
    switch (x)
    {
        // case ⼦句的顺序在 switching on type 时很重要（不像 when开启常量）。
        case bool b when b == true:
            Console.WriteLine("True");
            break;
        case bool:
            Console.WriteLine("False");
            break;
        // 如果你想switch⼀个类型，但对其值不感兴趣，你可以使⽤discard （_）：
        case DateTime _:
            Console.WriteLine("It's a DateTime");
            break;
        // 可以堆叠多个 case ⼦句
        case float f when f > 1000:
        case double d when d > 100:
        case int i when i > 1000:
        case decimal m when m > 1000:
            Console.WriteLine("We can refer to x here but not f or d or i or m"); // <-- HERE
            Console.WriteLine(x);
            break;
        case null:
            Console.WriteLine("Nothing here");
            break;
        default:
            Console.WriteLine("I don't know what x is");
            break;
    }
}

/// <summary>
/// Switch expressions
/// 从 C# 8 开始，您可以在表达式的上下⽂中使⽤ switch
/// 请注意，switch 关键字出现在变量名之后，并且case⼦句是表达式（以逗号结尾）⽽不是语句。
/// switch 表达式⽐对应的 switch 语句更紧凑，你可以在 LINQ 查询中使⽤它们。
/// 如果省略默认表达式 (_) 并且switch匹配失败，则抛出异常。
/// 还可以对多个值switch（元组模式）
/// </summary>
void SwitchExpressions()
{
    int cardNumber = 12;
    string cardName = cardNumber switch
    {
        13 => "King",
        12 => "Queen",
        11 => "Jack",
        _ => "Pip Card", // equivalent to 'default'
    };
    Console.WriteLine(cardName);

    // 对多个值switch（元组模式）
    string suite = "spades";
    cardName = (cardNumber, suite) switch
    {
        (13, "spades") => "King of spades",
        (13, "clubs") => "King of clubs",
        _ => "Pip Card", // equivalent to 'default' 
    };
    Console.WriteLine(cardName);
}
SwitchStatement();

void IterationStatements()
{
    int i = 0;
    while (i < 3)
    {
        Console.Write(i);
        i++;
    }

    i = 0;
    do
    {
        Console.WriteLine(i);
        i++;
    }
    while (i < 3);

    for (i = 0; i < 3; i++)
        Console.WriteLine(i);

    foreach (char c in "beer") // c is the iteration variable
        Console.WriteLine(c);
}
IterationStatements();

/// <summary>
/// 跳转语句包括：
/// break, continue, goto, return, throw 
/// </summary>
void JumpStatements()
{
    int x = 0;
    while (true)
    {
        if (x++ > 5)
            break; // break from the loop
    }

    int i = 0;
    for (i = 0; i < 10; i++)
    {
        if ((i % 2) == 0) // If i is even,
            continue; // continue with next iteration
        Console.Write(i + " ");
    }
    // OUTPUT: 1 3 5 7 9

    i = 1;
startLoop:
    if (i <= 5)
    {
        Console.Write(i + " ");
        i++;
        goto startLoop;
    }
    // OUTPUT: 1 2 3 4 5

    for (i = 0; i < 10; i++)
    {
        if ((i % 5) == 0)
            return;
        Console.Write(i + " ");
    }

    string? w = null;
    if (w == null)
        throw new ArgumentNullException("w is null");
}
JumpStatements();

void UsingStatements()
{

}
UsingStatements();

void LockStatements()
{

}
LockStatements();