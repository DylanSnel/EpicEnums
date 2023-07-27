using EpicEnums;

namespace SampleApp.Fruits;

public partial record Fruit : IEpicEnumValue
{
    public required string Name { get; init; }
    public required string Description { get; init; }

    //public bool IsFruit()
    //{
    //    return _FruitsValue is not null;
    //}
    //public static implicit operator FruitsEnum(Fruit fruit)
    //{
    //    return fruit._FruitsValue ?? throw new UnsupportedValueException();
    //}

    //public static bool operator ==(Fruit left, FruitsEnum right)
    //{
    //    return left._FruitsValue == right;
    //}
    //public static bool operator !=(Fruit left, FruitsEnum right)
    //{
    //    return left._FruitsValue != right;
    //}

}
