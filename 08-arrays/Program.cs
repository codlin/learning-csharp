int[] a = new int[100];
Console.WriteLine(a[30]);

Point[] p = new Point[100];
// Console.WriteLine(p[10].X); // System.NullReferenceException
p[10] = new();
p[10].X = 10;
p[10].Y = 10;
Console.WriteLine($"({p[10].X}, {p[10].Y})");

char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
Console.WriteLine($"{vowels[0]}, {vowels[^1]}");
Console.WriteLine("{0}, {1}", vowels[..2], vowels[3..]);

class Point
{
    public int X;
    public int Y;
}
