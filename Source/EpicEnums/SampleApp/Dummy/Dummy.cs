namespace SampleApp.Dummy;
using NotVegetables;

public partial record Dummy
{
    private string? _test;

    public required string Name { get; init; }
    public required string Description { get; init; }

    public string? Bla
    {
        get
        {
            return _test;
        }
    }

    internal string? Test
    {
        init
        {
            _test = value;
        }
    }

}

public class Foo
{
    public FruitsEnum MyFruit { get; set; } = FruitsEnum.Apple;

    public void DisplayName()
    {
        Fruit x = MyFruit;
        Console.WriteLine(x.Name);
    }
}
