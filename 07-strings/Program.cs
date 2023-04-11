/* 字符 */
char a = 'A';
Console.WriteLine(a);

char copyrightSymbol = '\u00A9';
Console.WriteLine(copyrightSymbol);

// char --> unsigned short
Console.WriteLine("{0}", (short)a);
Console.WriteLine("to lower: {0}", Char.ToLower(a));

string p = "\\home\\codlin";
Console.WriteLine(p);

/* 字符串 */
// 逐字字符串
string home = @"\home\codlin";
Console.WriteLine(home);

string xml = "<customer id=\"123\"></customer>";
Console.WriteLine(xml); // <customer id="123"></customer>

xml = @"<customer id=""123""></customer>";
Console.WriteLine(xml); // <customer id="123"></customer>

string s = @"s: {{123}}";
Console.WriteLine(s);   // s: {{123}}

// 字符串插值
s = $"s: {123}";
Console.WriteLine(s); // s: 123

s = $"s: {{123}}";
Console.WriteLine(s); // s: {123}

// 内插字符串必须在⼀⾏中完成，除⾮您还指定逐字字符串运算符
int x = 2;
// Note that $ must appear before @ prior to C# 8:
s = $@"this interpolation spans {x} lines";

// 拼接
string b = "Hello" + " world!";

// 比较
string c = "Hello world!";
Console.WriteLine(b == c); // True
Console.WriteLine(String.Compare(b, c)); // 0
Console.WriteLine(b.CompareTo(c)); // 0

// 创建重复字符串
s = new string('*', 16);
Console.WriteLine(s);

// null & empty string
s = "";
Console.WriteLine(s == String.Empty);// True
Console.WriteLine(s.Length == 0); // True
Console.WriteLine(String.IsNullOrEmpty(s)); // True

string? nullString = null;
Console.WriteLine(nullString == null); // True
Console.WriteLine(nullString == ""); // False
Console.WriteLine(String.IsNullOrEmpty(nullString)); // True

// index
Console.WriteLine("c[6]={0}", c[6]);

// substring
Console.WriteLine("{0}", c.Substring(0, 5));