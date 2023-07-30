﻿using EpicEnums;

namespace SampleApp.NotVegetables;

public partial record Fruit : IEpicEnumValue
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool LikeAble { get; init; }
}
