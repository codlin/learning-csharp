/// <summary>
/// 每个参与者必须有partial声明
/// 参与者不能有相互冲突的成员。例如，不能有多个具有相同参数（签名）的构造函数。
/// 部分类型完全由编译器“缝合”（resolved），这意味着每个参与者都必须在编译时可⽤，并且必须驻留在同⼀个程序集中。
/// 您可以在⼀个或多个部分类声明中指定⼀个基类，只要作为基类，如果指定的话，是相同的。
/// 此外，每个参与者都可以独⽴指定要实现的接⼝。
/// 对于多个部分类的声明，编译器不保证它们之间的字段初始化顺序。
/// </summary>
partial class PartialClass
{
    string? name;

    partial void ValidatePayment(decimal amount);
    public partial bool IsValid(string identifier);
    internal partial bool TryParse(string number, out int result);
}