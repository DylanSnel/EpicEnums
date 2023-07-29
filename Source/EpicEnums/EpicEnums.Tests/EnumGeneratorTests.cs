using EpicEnums.Tests.FruitsTests;
using FluentAssertions;

namespace EpicEnums.Tests;


public class EnumGeneratorTests
{
    [Fact]
    public void FruitsEnum_Exists()
    {
        (Fruits.Apple == FruitsEnum.Apple).Should().BeTrue();
    }

    //[Fact]
    //public Task GeneratesEnumCorrectly()
    //{
    //    var source = """

    //                 using EpicEnums;

    //                 namespace SampleApp.Fruits;

    //                 public partial record Fruits : EpicEnum<Fruit>
    //                 {
    //                     public static Fruit Apple { get; } = new() { Name = "Apple", Description = "A red fruit", LikeAble = true };
    //                     public static Fruit Banana { get; } = new() { Name = "Banana", Description = "A yellow fruit", LikeAble = true };
    //                     public static Fruit Orange { get; } = new() { Name = "Orange", Description = "An orange fruit", LikeAble = false };
    //                     public static Fruit DragonFruit { get; } = new() { Name = "Dragon Fruit", Description = "A pink fruit", LikeAble = true };
    //                 }


    //                 public partial record Fruit : IEpicEnumValue
    //                 {
    //                     public required string Name { get; init; }
    //                     public required string Description { get; init; }
    //                     public required bool LikeAble { get; init; }
    //                 }


    //                 """;

    //    return EnumGeneratorHelper.Verify(source);
    //}
}
