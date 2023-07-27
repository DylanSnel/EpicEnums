namespace SampleApp.Dummy;

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
