/// <summary>
/// using static 指令导⼊类型type⽽不是命名空间。然后可以⽆限制地使⽤所有导⼊类型的静态成员。
/// using static 指令导⼊该类型的所有可访问静态成员，包括字段、属性和嵌套类型。
/// </summary>
using static System.Console;

namespace Animal.Zone.Earth;
class Lion
{
    string name;
    public Lion(string name)
    {
        this.name = name;
        WriteLine(name);
    }

}