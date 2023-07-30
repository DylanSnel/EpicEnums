using EpicEnums.Tests.FruitsTests;
using FluentAssertions;

namespace EpicEnums.Tests;

public class EnumGeneratorTests
{
    [Fact]
    public void FruitsEnum_Exists_ShouldNotCrash()
    {
        var x = FruitsEnum.Apple;
        x.Should().BeOneOf(FruitsEnum.Apple, FruitsEnum.Banana, FruitsEnum.Orange, FruitsEnum.DragonFruit);
    }

    [Fact]
    public void FruitsEnum_Conversion()
    {
        Fruit apple = FruitsEnum.Apple;
        apple.Should().Be(Fruits.Apple);

        FruitsEnum banana = Fruits.Banana;
        banana.Should().Be(FruitsEnum.Banana);

        var dragonFruit = (FruitsEnum)Fruits.DragonFruit;
        dragonFruit.Should().Be(FruitsEnum.DragonFruit);

        var orange = (Fruit)FruitsEnum.Orange;
        orange.Should().Be(Fruits.Orange);
    }

    [Fact]
    public void FruitsEnum_Enumeration()
    {
        int expectedCount = 4;
        Fruits.Values.Count().Should().Be(expectedCount);
        Enum.GetValues(typeof(FruitsEnum)).Length.Should().Be(expectedCount);


    }

    [Fact]
    public void FruitsEnum_Comparison()
    {
        (Fruits.Apple == FruitsEnum.Apple).Should().BeTrue();
        (Fruits.Apple != FruitsEnum.Banana).Should().BeTrue();

        (Fruits.Apple == FruitsEnum.Banana).Should().BeFalse();
        (Fruits.Apple != FruitsEnum.Apple).Should().BeFalse();

        (FruitsEnum.Apple == Fruits.Apple).Should().BeTrue();
        (FruitsEnum.Apple != Fruits.Banana).Should().BeTrue();

        (FruitsEnum.Apple == Fruits.Banana).Should().BeFalse();
        (FruitsEnum.Apple != Fruits.Apple).Should().BeFalse();

        (Fruits.Apple == Fruits.Apple).Should().BeTrue();
        (Fruits.Apple != Fruits.Banana).Should().BeTrue();

    }
}
