﻿namespace EpicEnums.Tests.FruitsTests;

public partial record Fruit : IEpicEnumValue
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool LikeAble { get; init; }
}
