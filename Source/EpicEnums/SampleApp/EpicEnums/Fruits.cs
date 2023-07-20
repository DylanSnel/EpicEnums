using EpicEnums;
using System.Collections;

namespace SampleApp.EpicEnums;

public partial record Fruits : EpicEnum<Fruit>, IEnumerable<Fruit>
{
    public static Fruit Apple { get; } = new() { Name = "Apple", Description = "A red fruit" };
    public static Fruit Banana { get; } = new() { Name = "Banana", Description = "A yellow fruit" };
    public static Fruit Orange { get; } = new() { Name = "Orange", Description = "An orange fruit" };
    public static Fruit Papaya { get; } = new() { Name = "Papaya", Description = "An also orange fruit" };
    public static Fruit DragonFruit { get; } = new() { Name = "Dragon Fruit", Description = "A pink fruit" };

    #region ShouldBeGenerated

    static Fruits()
    {
        Apple = Apple with { EnumValue = FruitsEnum.Apple };
        Banana = Banana with { EnumValue = FruitsEnum.Banana };
        Orange = Orange with { EnumValue = FruitsEnum.Orange };
        Papaya = Papaya with { EnumValue = FruitsEnum.Papaya };
        DragonFruit = DragonFruit with { EnumValue = FruitsEnum.DragonFruit };
    }

    public Fruit this[FruitsEnum fruit]
        => FromEnum(fruit);

    public static Fruit FromEnum(FruitsEnum brand)
        => Enumerable().FirstOrDefault(x => x.EnumValue == brand) ?? throw new FruitNotSupportedException(brand);

    public static IEnumerable<Fruit> Enumerable()
    {
        yield return Apple;
        yield return Banana;
        yield return Orange;
        yield return Papaya;
        yield return DragonFruit;
    }

    public IEnumerator<Fruit> GetEnumerator()
    {
        return Enumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Enumerable().GetEnumerator();
    }
    #endregion
}

