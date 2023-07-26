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
    public static Fruit Grape { get; } = new() { Name = "Grape", Description = "A purple fruit" };
    public static Fruit StarFruit { get; } = new() { Name = "Star Fruit", Description = "A yellow fruit" };
    public static Fruit Durian { get; } = new() { Name = "Durian", Description = "A smelly fruit" };

    #region ShouldBeGenerated

    static Fruits()
    {
        Apple = Apple with { FruitsValue = FruitsEnum.Apple };
        Banana = Banana with { FruitsValue = FruitsEnum.Banana };
        Orange = Orange with { FruitsValue = FruitsEnum.Orange };
        Papaya = Papaya with { FruitsValue = FruitsEnum.Papaya };
        DragonFruit = DragonFruit with { FruitsValue = FruitsEnum.DragonFruit };
        Grape = Grape with { FruitsValue = FruitsEnum.Grape };
        StarFruit = StarFruit with { FruitsValue = FruitsEnum.StarFruit };
        Durian = Durian with { FruitsValue = FruitsEnum.Durian };
    }

    public Fruit this[FruitsEnum fruit]
        => FromEnum(fruit);

    public static Fruit FromEnum(FruitsEnum brand)
        => Enumerable().FirstOrDefault(x => x.FruitsValue == brand) ?? throw new FruitNotSupportedException(brand);

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

