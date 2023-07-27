using EpicEnums;

namespace SampleApp.Fruits;

public partial record Fruits : EpicEnum<Fruit>
{
    public static Fruit Apple { get; } = new() { Name = "Apple", Description = "A red fruit" };
    public static Fruit Banana { get; } = new() { Name = "Banana", Description = "A yellow fruit" };
    public static Fruit Orange { get; } = new() { Name = "Orange", Description = "An orange fruit" };
    public static Fruit Papaya { get; } = new() { Name = "Papaya", Description = "An also orange fruit" };
    public static Fruit DragonFruit { get; } = new() { Name = "Dragon Fruit", Description = "A pink fruit" };
    public static Fruit Grape { get; } = new() { Name = "Grape", Description = "A purple fruit" };
    public static Fruit StarFruit { get; } = new() { Name = "Star Fruit", Description = "A yellow fruit" };
    public static Fruit Durian { get; } = new() { Name = "Durian", Description = "A smelly fruit" };

}

