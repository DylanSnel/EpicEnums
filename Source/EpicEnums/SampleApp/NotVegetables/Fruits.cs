using EpicEnums;

namespace SampleApp.NotVegetables;

public partial record Fruits : EpicEnum<Fruit>
{
    public static Fruit Apple { get; } = new() { Name = "Apple", Description = "A red fruit", LikeAble = true };
    public static Fruit Banana { get; } = new() { Name = "Banana", Description = "A yellow fruit", LikeAble = true };
    public static Fruit Orange { get; } = new() { Name = "Orange", Description = "An orange fruit", LikeAble = false };
    public static Fruit DragonFruit { get; } = new() { Name = "Dragon Fruit", Description = "A pink fruit", LikeAble = true };

}

