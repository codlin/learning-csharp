﻿using System;
using static System.Console;

/// <h1>
/// Formatting using numbered positional arguments
/// </h1>
int numberOfApples = 12;
decimal pricePerApple = 0.35M;
Console.WriteLine(format: "{0} apples cost {1:C}", arg0: numberOfApples, arg1: pricePerApple * numberOfApples);
string formatted = string.Format(format: "{0} apples cost {1:C}", arg0: numberOfApples, arg1: pricePerApple * numberOfApples);
//WriteToFile(formatted); // writes the string into a file
/// 这种通过arg0，arg1，arg2，……，这种方式会比较繁琐，因此可以使用另一种形式
Console.WriteLine(format: "{0} {1} lived in {2}, {3} and worked in the {4} team at {5}.",
"Roger", "Cevung", "Stockholm", "Sweden", "Education", "Optimizely");

/// <h1>
/// Formatting using interpolated strings
/// </h1>
/// C# 6.0 and later have a handy feature named interpolated strings. A string prefixed with $ can use
/// curly braces around the name of a variable or expression to output the current value of that variable
/// or expression at that position in the string。
/// C# 6.0 及更高版本有一个方便的功能，称为内插字符串。
/// 以 $ 为前缀的字符串可以使用将变量或表达式的名称用大括号括起来以输出该变量的当前值或字符串中该位置的表达式。
// The following statement must be all on one line.
Console.WriteLine($"{numberOfApples} apples cost {pricePerApple * numberOfApples:C}");
string firstname = "Omar";
string lastname = "Rudberg";
string fullname = $"{firstname} {lastname}";
Console.WriteLine(fullname);

/// <h1>
/// Understanding format strings
/// </h1>
/// A variable or expression can be formatted using a format string after a comma or colon.
/// An N0 format string means a number with thousands separators and no decimal places, while a C
/// format string means currency. The currency format will be determined by the current thread.
/// For instance, if you run code that uses the number or currency format on a PC in the UK, you’ll get
/// pounds sterling with commas as the thousands separators, but if you run it on a PC in Germany, you
/// will get euros with dots as the thousands separators.
/// The full syntax of a format item is:
/// { index [, alignment ] [ : formatString ] }
/// Each format item can have an alignment, which is useful when outputting tables of values, some of
/// which might need to be left- or right-aligned within a width of characters. Alignment values are integers.
/// Positive integers mean right-aligned and negative integers mean left-aligned.
/// 可以在逗号或冒号后使用格式字符串来格式化变量或表达式。
/// N0 格式字符串表示带有千位分隔符且没有小数位的数字（注：N1则代表有一个小数位），而 C 格式字符串表示货币。货币格式将由当前线程确定。
/// 例如，如果您在英国的 PC 上运行使用数字或货币格式的代码，您将获得英镑以逗号作为千位分隔符，但是如果您在德国的 PC 上运行它，您
/// 将得到带有点作为千位分隔符的欧元。
/// 格式项的完整语法是：
/// { index [, alignment ] [ : formatString ] }
/// 每个格式项都可以有一个对齐方式，这在输出值表时很有用，一些需要在字符宽度内左对齐或右对齐。
/// 对齐值是整数。正整数表示右对齐，负整数表示左对齐。
string applesText = "Apples";
int applesCount = 1234;
string bananasText = "Bananas";
int bananasCount = 56789;
Console.WriteLine(format: "{0,-10} {1,6}", arg0: "Name", arg1: "Count");
Console.WriteLine(format: "{0,-10} {1,6:N0}", arg0: applesText, arg1: applesCount);
Console.WriteLine(format: "{0,-10} {1,6:N0}", arg0: bananasText, arg1: bananasCount);

/// <h1>
/// Getting text input from the user 
/// </h1>
/// We can get text input from the user using the ReadLine method. This method waits for the user to
/// type some text. Then, as soon as the user presses Enter, whatever the user has typed is returned as
/// a string value.
/// 我们可以使用 ReadLine 方法从用户那里获取文本输入。此方法等待用户输入一些文字。
/// 然后，一旦用户按下 Enter，无论用户输入什么，都会返回为一个字符串值。
Console.Write("Type your first name and press ENTER: ");
string firstName = Console.ReadLine();
Console.Write("Type your age and press ENTER: ");
string age = Console.ReadLine();
Console.WriteLine($"Hello {firstName}, you look good for {age}.");

/// 上面 firstName 和 age 会产生编译告警，因为ReadLine方法可能返回null而不是字符串。
/// 可以在类型string后面加上问号，用于告诉编译器我们期望一个空值。
string? lastName = Console.ReadLine();

/// 我们也可以在句尾分号前加上叹号（称之为null-forgiving operator），告诉编译器我们希望不要返回空值。
string name = Console.ReadLine()!;
Console.WriteLine(name);

/// In C# 6.0 and later, the using statement can be used not only to import a namespace but also to further
/// simplify our code by importing a static class. Then, we won’t need to enter the Console type name
/// throughout our code.
/// C# 6.0 以后，using语言不但可以导入命名空间，而且可以使用using static导入静态类。所以我们可以不必写Console
WriteLine("No need Console");

/// Instead of statically importing the Console class just for one code file, it would probably be better to
/// import it for all code files in the project:
/// 更好的方法是在项目的配置文件里配置为所有的文件都导入Console类，而不是在每个代码文件里分别导入。
/// 在.csproj加入：
/// <ItemGroup>
///   <Using Include="System.Console" Static="true" />
/// </ItemGroup>

/// <h1>
/// Getting key input from the user
/// </h1>
/// We can get key input from the user using the ReadKey method. This method waits for the user to press
/// a key or key combination that is then returned as a ConsoleKeyInfo value.
/// 我们能用ReadKey方法获取用户输入的key。这个方法等待用户按下的Key或者键值组合，然后返回一个ConsoleKeyInfo类型的值。
Console.Write("Press any key combination: ");
ConsoleKeyInfo key = Console.ReadKey();
Console.WriteLine();
Console.WriteLine("Key: {0}, Char: {1}, Modifiers: {2}", arg0: key.Key, arg1: key.KeyChar, arg2: key.Modifiers);
