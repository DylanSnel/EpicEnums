using EpicEnums;

namespace SampleApp.EpicEnums;

public class Fruits : EpicEnum<Fruit>
{
    public static Fruit Apple { get; } = new() { Name = "Apple", Description = "A red fruit" };
    public static Fruit Banana { get; } = new() { Name = "Banana", Description = "A yellow fruit" };
    public static Fruit Orange { get; } = new() { Name = "Orange", Description = "An orange fruit" };

}

