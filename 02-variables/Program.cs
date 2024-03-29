﻿using System.Xml;

using BasicTypes;

/// <h1>
/// Storing text
/// </h1>

// 1. literal strings 
string firstName = "Bob"; // assigning literal strings
Console.WriteLine(firstName);

// 2. 字符
string horizontalLine = new('-', count: 16);
Console.WriteLine(horizontalLine);

// assigning an emoji by converting from Unicode
string grinningEmoji = char.ConvertFromUtf32(0x1F600);
Console.WriteLine(grinningEmoji);

// 3. Verbatim strings
string filePath = "C:\televisions\\sony\bravia.txt";
Console.WriteLine(filePath);
// 编译器会把上面路径中的\t转义成一个tab字符，然后得到了错误的路径
// 可以在逐字字符串（Verbatim strings）前面加上@符号，这样编译器不会对该字符串进行转义
string filePath2 = @"C:\televisions\sony\bravia.txt";
Console.WriteLine(filePath2);

// 4. Raw string literals
// 原始字符串是以3个双引号包括的字符串
string xml = """
            <person age="50">
                <first_name>Mark</first_name>
            </person>
            """;
// 当上面的字符串对齐时，编译器会自动移除顶层前导空格，但缩进关系依然保持
Console.WriteLine(xml);

// 5. Raw interpolated string literals
// 只需要在字符串前面加上$就可以在字符串中插入变量或表达式，大括号包括的即为要解析的变量或表达式
string greetings = $"Hello {firstName}!";
Console.WriteLine(greetings);
// 可以通过在字符串前面加不同数量的$来告诉编译器如何解析插入的变量或表达式，几个$对应大括号的数量，只有匹配的才会被解析。
// 如下面有两个$，那么只有两个{的才会被解析为变量或表达式
string lastName = "Dylan";
int age = 18;
string person = $$"""
                {
                    "first_name": "{{firstName}}",
                    "last_name": "{{lastName}}",
                    "age": "{{age}}",
                    "calculation", "{{{1 + 2}}}"
                }
                """;
Console.WriteLine(person);

// three variables that store the number 2 million
int decimalNotation = 2_000_000;
int binaryNotation = 0b_0001_1110_1000_0100_1000_0000;
int hexadecimalNotation = 0x_001E_8480;
// check the three variables have the same value
// both statements output true
Console.WriteLine($"{decimalNotation == binaryNotation}"); // True
Console.WriteLine($"{decimalNotation == hexadecimalNotation}"); // True

/// <h1>
/// Storing real numbers
/// </h1>

// float and double
Console.WriteLine($"int uses {sizeof(int)} bytes and can store numbers in the range {int.MinValue:N0} TO {int.MaxValue:N0}.");
Console.WriteLine($"double uses {sizeof(double)} bytes and can store numbers in the range {double.MinValue:N0} TO {double.MaxValue:N0}.");
Console.WriteLine($"decimal uses {sizeof(decimal)} bytes and can store numbers in the range {decimal.MinValue:N0} TO {decimal.MaxValue:N0}.");

/// Comparing double and decimal types
Console.WriteLine("Using doubles:");
double a = 0.1;
double b = 0.2;
// Never compare double values using ==
if (a + b == 0.3)
{
    Console.WriteLine($"{a} + {b} equals {0.3}");
}
else
{
    Console.WriteLine($"{a} + {b} does NOT equal {0.3}");
}

Console.WriteLine("Using decimals:");
decimal c = 0.1M; // M suffix means a decimal literal value
decimal d = 0.2M;
// The decimal type is accurate because it stores the number as a large integer and shifts the decimal
// point. For example, 0.1 is stored as 1, with a note to shift the decimal point one place to the left. 12.75
// is stored as 1275, with a note to shift the decimal point two places to the left.
if (c + d == 0.3M)
{
    Console.WriteLine($"{c} + {d} equals {0.3M}");
}
else
{
    Console.WriteLine($"{c} + {d} does NOT equal {0.3M}");
}

/// <h1>
/// Storing any type of object
/// </h1>
/// There is a special type named object that can store any type of data, but its flexibility comes at the
/// cost of messier code and possibly poor performance. Because of those two reasons, you should avoid
/// it whenever possible.
/// 有一种名为 object 的特殊类型可以存储任何类型的数据，但它的灵活性在于更混乱的代码的成本和可能的性能不佳。
/// 由于这两个原因，你应该尽可能避免使用。
object height = 1.88; // storing a double in an object
object name = "Amir"; // storing a string in an object
Console.WriteLine($"{name} is {height} metres tall.");
// int length1 = name.Length; // gives compile error!
int length2 = ((string)name).Length; // tell compiler it is a string
Console.WriteLine($"{name} has {length2} characters.");

/// <h1>
/// Storing dynamic types
/// </h1>
/// There is another special type named dynamic that can also store any type of data, but even more than
/// object, its flexibility comes at the cost of performance. The dynamic keyword was introduced in C#
/// 4.0. However, unlike object, the value stored in the variable can have its members invoked without
/// an explicit cast.
/// 还有另一种名为动态的特殊类型——dynamic关键字，它也可以存储任何类型的数据，但比对象，其灵活性是以性能为代价的。 
/// C# 4.0 中引入了dynamic关键字。但是，与对象不同，存储在变量中的值可以调用其成员而无需显式转换。
// storing a string in a dynamic object
// string has a Length property
dynamic something = "Ahmed";
Console.WriteLine(something); // Ahmed
// int does not have a Length property
something = 12;
Console.WriteLine(something); // 12
// an array of any type has a Length property
something = new[] { 3, 5, 7 };
Console.WriteLine(something.Length); // 3

// good use of var because it avoids the repeated type
// as shown in the more verbose second statement
// var 正确使用，因为它避免了类型信息的重复。下面第二行的显得冗长。
var xml1 = new XmlDocument(); // C# 3 and later
XmlDocument xml2 = new XmlDocument(); // all C# versions

// bad use of var because we cannot tell the type, so we
// should use a specific type declaration as shown in
// the second statement
// var 的错误使用，因为我们无法分辨类型，所以我们应该使用特定的类型声明，
// 如第二个陈述所示是较好的用法：
var file1 = File.CreateText("something1.txt");
StreamWriter file2 = File.CreateText("something2.txt");

/// <h2>
/// Using target-typed new to instantiate objects
/// 用目标类型 new 去实例化对象
/// </h2>
/// With C# 9, Microsoft introduced another syntax for instantiating objects known as target-typed new.
/// When instantiating an object, you can specify the type first and then use new without repeating the
/// type, as shown in the following code:
/// 在 C# 9 中，Microsoft 引入了另一种用于实例化对象的语法，称为目标类型 new。
/// 实例化对象时，可以先指定类型，再new，不用重复类型，如以下代码所示：
XmlDocument xml3 = new(); // target-typed new in C# 9 or later
// 如果类型有字段或属性需要被初始化，则其类型可以推断。如：
Person kim = new();
kim.BirthDate = new(1967, 12, 26); // instead of: new DateTime(1967, 12, 26)

// List类型的初始化
List<Person> people = new()
{
    new() { BirthDate = new(1967, 12, 26) },
    new() { BirthDate = new(1967, 12, 26) }
};

/// <h2>
/// Getting and setting the default values for types
/// </h2>
/// Most of the primitive types except string are value types, which means that they must have a value.
/// You can determine the default value of a type by using the default() operator and passing the type
/// as a parameter. You can assign the default value of a type by using the default keyword.
/// The string type is a reference type. This means that string variables contain the memory address
/// of a value, not the value itself. A reference type variable can have a null value, which is a literal that
/// indicates that the variable does not reference anything (yet). null is the default for all reference types.
/// 除了字符串之外，大多数基本类型都是值类型，这意味着它们必须有一个值。您可以通过使用 default() 运算符并传递类型来确定类型的默认值
/// 作为参数。您可以使用 default 关键字分配类型的默认值。字符串类型是引用类型。这意味着字符串变量包含内存地址一个值，而不是值本身。
/// 引用类型变量可以有一个空值，它是一个文字表示变量没有引用任何东西（还）。 null 是所有引用类型的默认值。
Console.WriteLine($"default(int) = {default(int)}");
Console.WriteLine($"default(bool) = {default(bool)}");
Console.WriteLine($"default(DateTime) = {default(DateTime)}");
Console.WriteLine($"default(string) = {default(string)}");
int number = 13;
Console.WriteLine($"number has been set to: {number}");
number = default;
Console.WriteLine($"number has been reset to its default: {number}");

var basicTypeTest = new BasicTypesTest();
basicTypeTest.Create();
basicTypeTest.ValueCopy();
basicTypeTest.ReferenceCopy();
basicTypeTest.Conversion();
basicTypeTest.RefLocals();
ref string xRef = ref basicTypeTest.RefReturns();
xRef = "New Value";
Console.WriteLine(basicTypeTest.X);
string x = basicTypeTest.RefReturns();
x = "Magic Value";
Console.WriteLine(basicTypeTest.X);

ref readonly string sr = ref basicTypeTest.RefReadonlyReturns();
// sr = "Const Value"; // <-- HERE
Console.WriteLine(sr);

basicTypeTest.ImplicitlyTypedLocalVariables();

class Person
{
    public DateTime BirthDate;
}
