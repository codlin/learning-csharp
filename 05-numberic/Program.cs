/// <summary>
/// 数值类型
///
/// 预定义的数值类型大概分为三类：
/// 1. 有符号
///   sbyte (8 bits), short(16 bits), int(32 bits), long(64 bits, suffix: L), nint(=IntPtr 32/64 bits)
/// 2. 无符号
///   byte (8 bits), ushort(16 bits), uint(32 bits, suffix: U), ulong(64 bits, suffix: UL), unint(=UIntPtr 32/64 bits)
/// 3. 实数
///   float (32 bits, suffix: F), double (64bits, suffix: D), decimal(128 bits, suffix: D)
///
/// 在整数类型中，int 和 long 是⼀等公⺠，受到 C# 及其运⾏时的⻘睐。
/// 其他整数类型通常⽤于互操作性或当空间效率⾄关重要时。
/// nint 和 nuint 是本机⼤⼩的整数类型（在 C# 9 中引⼊），在帮助指针运算⽅⾯最有⽤。
/// 在实数类型中，float 和 double 被称为浮点类型，通常⽤于科学和图形计算。
/// decimal类型是通常⽤于财务计算，或以 10 为基数的精确算术和⾼精度计算。
/// </summary>
void NumericLiterals()
{
    int x = 127;
    long y = 0x7F;

    // 可以在数字字面值的任何位置插入下划线以使其更容易阅读
    int million = 1_000_000;

    // 可以使用0b前缀指定二进制数字
    var b = 0b_1010_1011_1100_1101_1110_1111;

    // 实数字面值也可以用十进制或者指数法表示
    double d = 1.5;
    double emillion = 1E06;

    Console.WriteLine($"{x}, {y}, {million}, {b}, {d}, {emillion}");
}

/// <summary>
/// 默认情况下，编译器将字面值推断为双精度double或整型(int, uint, long, ulong)
///    - 如果字面值包含小数点或指数符号E，则它是一个double的
///    - 否则，字面值的类型是下面合适字面值类型的第一个类型（按顺序）：int, uint, long, ulong 
/// </summary>
void NumericLiteralTypeInference()
{
    Console.WriteLine(1.0.GetType());                // Double (double)
    Console.WriteLine(1E06.GetType());               // Double (double)
    Console.WriteLine(1.GetType());                  // Int32 (int)
    Console.WriteLine(0xF0000000.GetType());         // UInt32 (uint)
    Console.WriteLine(0x100000000.GetType());        // Int64 (long)
    Console.WriteLine(0xF000000000000000.GetType()); // UInt64 (ulong)
}

/// <summary>
/// 数字后缀明确定义⽂字的类型。后缀可以是⼩写或⼤写
/// </summary>
void NumericSuffixes()
{
    // 后缀 U 和 L 很少需要，因为 uint、long 和 ulong 类型⼏乎总是可以从 int 推断或隐式转换：
    long l = 5;
    Console.WriteLine(l);

    //D 后缀在技术上是多余的，因为所有带⼩数点的⽂字都是推断为double。⽽且您始终可以在数字字⾯值上添加⼩数点
    double x = 4D;
    Console.WriteLine(x);
    x = 4.0;
    Console.WriteLine(x);

    // F 和 M 后缀是最有⽤的，在以下情况指定浮点数或⼗进制字⾯值时应该始终使⽤。
    float f = 3.14F;
    Console.WriteLine(f);

    decimal d = -1.23M;
    Console.WriteLine(d);

    // 如果没有 F 后缀，以下⾏将⽆法编译，因为 4.5 会被推断为 double 类型，它没有隐式转换为浮点数
    // f = 4.5; // compiling error
    // decimal 类型同意适用
    // d = -1.23; // compiling error
}

/// <summary>
/// 数值间的转换分几类：
///     1. 整数类型之间的转换
///     2. 浮点类型之间的转换
///     3. 整数和浮点类型之间的转换
///     4. 十进制类型转换
/// </summary>
void NumericConversions()
{
    // 1. 整数类型之间的转换
    // 当目标类型可以表示每一个原类型可能的值时，整数类型转换是隐式的。否则需要显式转换
    int x = 12345;
    long y = x;
    short z = (short)x;
    Console.WriteLine($"{x}, {y}, {z}");

    // 2. 浮点类型之间的转换
    // 双精度可以表示每个可能的float类型的值，所以浮点数可以隐式转换为双精度数。反之，转换必须是显式的。
    float f = 3.14F;
    double d = f;
    float t = (float)d;
    Console.WriteLine($"{f}, {d}, {t}");

    // 3. 在浮点类型和整数类型之间转换
    // 所有整数类型都可以隐式转换为浮点类型
    int i = 1;
    f = i;
    Console.WriteLine($"{f}");

    // 从浮点类型转换为整型必须是显式的
    int i2 = (int)f;
    Console.WriteLine(i2);

    // 当大整数转换为浮点类型时可以保留大小，但是可能会损失精度。因为浮点数总是比整数类型具有更大的数量级，但精度较低。
    ulong ul = 0xF000000000000000UL;
    f = ul;
    Console.WriteLine($"{ul}, {f}");  // 17293822569102704640, 1.7293823E+19

    // 当从浮点数转换为整数时类型，任何⼩数部分都被截断；不执⾏舍⼊。
    ul = (ulong)3.9F;
    Console.WriteLine(ul); // 3

    // 静态类 System.Convert 提供⽅法在各种数字类型之间转换时的round。
    Console.WriteLine(System.Convert.ToInt64(3.9F)); // 4

    // 4. 十进制类型转换
    // 所有整数类型都可以隐式转换为十进制类型
    decimal dc = 1000_000;
    Console.WriteLine(dc);

    // 所有非整数类型和十进制类型的转换必须是显式的
    dc = (decimal)3.14F;
    Console.WriteLine(dc); // 3.14
}

/// <summary>
/// 算术运算符 (+, -, *, /, %)
/// 算法运算符是为除 8- 和 16- 位整数类型（byte, sbyte, short, ushort）之外的所有数字类型定义的。
/// </summary>
void ArithmeticOperators()
{
    int x = 10, y = 20;
    Console.WriteLine($"{x + y}, {x - y}, {x * y}, {x / y}, {x % y}");
}

/// <summary>
/// The integral types are int, uint, long, ulong, short, ushort, byte, and sbyte.
/// </summary>
void IntegralTypesSpecializedOperations()
{
    /* 除法
       整数类型的除法总是消除余数（向 0）。除以值为 0 的变量会产生运行时错误 
    */
    int a = 2 / 3;
    Console.WriteLine(a);

    // System.DivideByZeroException
    // int b = 0;
    // int c = 5 / b; // <-- System.DivideByZeroException

    /* 溢出
    */
    int c = int.MinValue;
    c--;
    Console.WriteLine(c);
    Console.WriteLine(c == int.MaxValue); // True

    /* Overflow check operators
    在整型表达式或语句超出了该类型的算术限制时，checked 运算符指示运行时生成 OverflowException 而不是静默地溢出。
    溢出检查会产生很小的性能成本。
    */
    a = 1_000_000;
    try
    {
        c = checked(a * a); // Checks just the expression
        checked // Checks all expressions
        {
            int b = a * a;
            Console.WriteLine(b);
        }
    }
    catch (System.OverflowException)
    {
        Console.WriteLine("OverflowException");
    }

    // 如果对于特定的表达式或语句需要禁用溢出检查，则可以使用 unchecked 运算符。
    c = int.MaxValue;
    int x = unchecked(c + 1);
    Console.WriteLine(x);

    // x = int.MaxValue + 1; // Compile-time error
    x = unchecked(int.MaxValue + 1); // No errors
    Console.WriteLine(x);
}

/// <summary>
/// 位运算符
/// C# 支持以下位运算符：~、&、|、^、<<、>>
/// </summary>
void BitwiseOperators()
{
    int a = 0;
    Console.WriteLine(~a); // -1
    int b = 0x13579;
    int c = 0x24680;
    Console.WriteLine("0x{0:X}", b & c);
    Console.WriteLine("0x{0:X}", b | c);
    Console.WriteLine("0x{0:X}", b ^ c);
    Console.WriteLine("0x{0:X}", b << 1);
    Console.WriteLine("0x{0:X}", c >> 1);
}

/// <summary>
/// 8- and 16-Bit Integral Types
/// 8 位和 16 位整数类型是 byte、sbyte、short 和 ushort。
/// 这些类型缺少自己的算术运算符，因此 C# 根据需要将它们隐式转换为更大的类型。
/// </summary>
void Bits8And16IntegralTypes()
{
    short x = 1, y = 1;
    // short z = x + y; // <-- compile-time error
    short z = (short)(x + y);
    Console.WriteLine(z);
}

/// <summary>
/// 与整数类型不同，浮点类型有些值其操作需要特别对待。这些特殊值是 NaN（⾮数字）、+∞、−∞ 和 −0。 
/// float 和 double 类具有 NaN、+∞、-∞ 以及其它常量（MaxValue、MinValue 和 Epsilon）。
/// </summary>
void SpecialFloatDoubleValues()
{
    Console.WriteLine(double.Epsilon);
    Console.WriteLine(double.MaxValue);
    Console.WriteLine(double.MinValue);

    Console.WriteLine(double.NaN == float.NaN); // False
    Console.WriteLine(double.PositiveInfinity == float.PositiveInfinity); // True
    Console.WriteLine(double.NegativeInfinity == float.NegativeInfinity); // True

    Console.WriteLine(double.PositiveInfinity == 1.0 / 0.0); // True
    Console.WriteLine(double.NegativeInfinity == 1.0 / -0.0); // True
    Console.WriteLine(0.0 / 0.0); // NaN
    Console.WriteLine(0.0 / -0.0); // NaN
    Console.WriteLine((1.0 / 0.0) - (1.0 / 0.0)); // NaN
    Console.WriteLine(double.NegativeInfinity == 1.0 / -0.0); // True

    // 使⽤ == 时，NaN 值永远不会等于另⼀个值，即使是另⼀个 NaN值：
    Console.WriteLine(double.NaN == 0.0 / 0.0); // False
    Console.WriteLine(double.NaN == 0.0 / 0.0); // False

    // 要测试一个值是否位NaN，你必须使用float.IsNaN 或 double.IsNaN 方法
    Console.WriteLine(double.IsNaN(0.0 / 0.0)); // True

    // 当使⽤ object.Equals 时，两个 NaN 值是相等的
    Console.WriteLine(object.Equals(0.0 / 0.0, double.NaN));
}

/// <summary>
/// 实数舍⼊误差
/// </summary>
void RealNumberRoundingErrors()
{
    float x = 0.1f; // Not quite 0.1
    Console.WriteLine(x + x + x + x + x + x + x + x + x + x); // 1.0000001

    decimal m = 1M / 6M; // 0.1666666666666666666666666667M
    Console.WriteLine(m);

    double d = 1.0 / 6.0; // 0.16666666666666666
    Console.WriteLine(d);

    // 这会导致累积的舍⼊误差，从⽽破坏相等性和⽐较操作
    decimal notQuiteWholeM = m + m + m + m + m + m; // 1.0000000000000000000000000002M
    double notQuiteWholeD = d + d + d + d + d + d; // 0.99999999999999989
    Console.WriteLine(notQuiteWholeM == 1M); // False
    Console.WriteLine(notQuiteWholeD < 1.0); // True
}

NumericLiterals();
NumericLiteralTypeInference();
NumericSuffixes();
NumericConversions();
ArithmeticOperators();
IntegralTypesSpecializedOperations();
BitwiseOperators();
Bits8And16IntegralTypes();
SpecialFloatDoubleValues();
RealNumberRoundingErrors();
