public class Asset
{
    public string? Name;
}
public class Stock : Asset // inherits from Asset
{
    public long SharesOwned;
}
public class House : Asset // inherits from Asset
{
    public decimal Mortgage;
}