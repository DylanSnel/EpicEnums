namespace SampleApp.Dummy;

public partial record Dummies
{
    public static Dummy Apple { get; } = new() { Name = "Apple", Description = "A red fruit" };
    public static Dummy Banana { get; } = new() { Name = "Banana", Description = "A yellow fruit" };

    static Dummies()
    {
        Apple = Apple with { Test = "aa" };
        Banana = Banana with { Test = "aa" };
    }
    public static IEnumerable<Dummy> Enumerable()
    {
        yield return Apple;
        yield return Banana;
    }


}

