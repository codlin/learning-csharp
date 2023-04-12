using model;

/// <summary>
/// 当元素类型是值类型时，每个元素值作为数组的⼀部分分配 
/// </summary>
void DefaultElementInitialization()
{
    char[] vowels = new char[5]; // Declare an array of 5 characters
    vowels[0] = 'a';
    vowels[1] = 'e';
    vowels[2] = 'i';
    vowels[3] = 'o';
    vowels[4] = 'u';
    Console.WriteLine(vowels[1]); // e
    for (int i = 0; i < vowels.Length; i++)
        Console.Write(vowels[i]); // aeiou
}

/// <summary>
/// 当元素类型是值类型时，每个元素值作为数组的⼀部分分配 
/// </summary>
void ValueTypeArrays()
{
    int[] a = new int[100];
    Console.WriteLine(a[30]);
}

/// <summary>
/// 如果元素类型是引用类型，创建数组只会分配数组个数的 null 引⽤：
/// </summary>
void RefTypeArrays()
{
    Point[] p = new Point[100];
    // Console.WriteLine(p[10].X); // System.NullReferenceException
    p[10] = new();
    p[10].X = 10;
    p[10].Y = 10;
    Console.WriteLine($"({p[10].X}, {p[10].Y})");
}

/// <summary>
/// 索引和范围
/// </summary>
/// <value></value>
void IndexAndRange()
{
    char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
    // 索引
    Console.WriteLine($"{vowels[0]}, {vowels[^1]}");
    // 范围
    Console.WriteLine(vowels[..2]);
    Console.WriteLine(vowels[3..]);
    Console.WriteLine(vowels[1..4]);
    Console.WriteLine(vowels[^3..^1]);
    // Console.WriteLine(vowels[^1..^3]); // System.ArgumentOutOfRangeException
}

/// <summary>
/// 多维数组
/// 多维数组有两种类型：矩形数组 和 锯齿数组
///  
/// </summary>
void MultidimensionalArrays()
{
    // 矩形数组
    int[,] matrix = new int[3, 3];
    matrix[1, 1] = 1;
    Console.WriteLine(matrix[1, 1]);
    for (int i = 0; i < matrix.GetLength(0); i++)
        for (int j = 0; j < matrix.GetLength(1); j++)
            matrix[i, j] = i * 3 + j;

    // 使⽤显式值初始化矩形数组
    int[,] matrix2 = new int[,]
    {
        {0,1,2},
        {3,4,5},
        {6,7,8}
    };

    // 锯齿数组
    // 锯⻮状数组使⽤连续的⽅括号表⽰每个维度
    int[][] jaggedMatrix = new int[3][];
    for (int i = 0; i < jaggedMatrix.Length; i++)
    {
        int n = (i + 1) * 2;
        jaggedMatrix[i] = new int[n];
        for (int j = 0; j < jaggedMatrix[i].Length; j++)
            jaggedMatrix[i][j] = i * n + j;
    }
    for (int i = 0; i < jaggedMatrix.Length; i++) // <-- 注意不能用 GetLength
    {
        Console.Write('[');
        for (int j = 0; j < jaggedMatrix[i].Length; j++) // <-- 注意不能用 GetLength
        {
            Console.Write($"{jaggedMatrix[i][j]} ");
        }

        Console.WriteLine(']');
    }

    // 使⽤显式值初始化锯⻮状数组
    var jaggedMatrix2 = new int[][]
    {
        new int[] {0, 1},
        new int[] {4, 5, 6, 7},
        new int[] {12, 13, 14, 15, 16, 17}
    };
    Console.WriteLine(jaggedMatrix2);
}

/// <summary>
/// 简化的数组初始化表达式 
/// </summary>
void SimplifiedArrayInitializationExpressions()
{
    // 有两种⽅法可以缩短数组初始化表达式。
    // 第⼀个是省略new 运算符和类型
    char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
    int[,] rectangularMatrix =
    {
        {0,1,2},
        {3,4,5},
        {6,7,8}
    };

    int[][] jaggedMatrix =
    {
        new int[] {0,1,2},
        new int[] {3,4,5},
        new int[] {6,7,8,9}
    };

    // 第⼆种⽅法是使⽤ var 关键字，它指⽰编译器隐式键⼊局部变量
    var rectMatrix = new int[,] // rectMatrix is implicitly of type int[,]
    {
        {0,1,2},
        {3,4,5},
        {6,7,8}
    };
    var jaggedMat = new int[][] // jaggedMat is implicitly of type int[][]
    {
        new int[] {0,1,2},
        new int[] {3,4,5},
        new int[] {6,7,8,9}
    };
    var vowels2 = new[] { 'a', 'e', 'i', 'o', 'u' };
    // 元素必须都可以隐式转换为单⼀类型（并且⾄少有⼀个元素必须是那种类型，并且必须恰好有⼀个最好的类型）
    var x = new[] { 1, 10000000000 }; // <-- all convertible to long
    var y = new[] { 'A', 65 }; // System.Int32[]
    Console.WriteLine(y);
    // var z = new[] { "error", 65 }; // compile-time error
}

void BoundsChecking()
{
    int[] arr = new int[3];
    try
    {
        arr[3] = 1; // IndexOutOfRangeException thrown
    }
    catch (System.IndexOutOfRangeException)
    {
        Console.WriteLine("IndexOutOfRangeException");
    }
}

DefaultElementInitialization();
ValueTypeArrays();
RefTypeArrays();
IndexAndRange();
MultidimensionalArrays();
SimplifiedArrayInitializationExpressions();
BoundsChecking();
