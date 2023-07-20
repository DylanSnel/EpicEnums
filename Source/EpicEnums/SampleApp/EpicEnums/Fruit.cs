using EpicEnums;

namespace SampleApp.EpicEnums;

public partial record Fruit : EpicEnumValue
{
    public required string Name { get; init; }
    public required string Description { get; init; }


    #region ShouldBeGenerated

    public FruitsEnum EnumValue { get; init; }

    #endregion
}
