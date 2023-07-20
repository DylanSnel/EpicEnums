namespace EpicEnums;

public abstract class EpicEnum : EpicEnum<EpicEnumValue> { }
public abstract class EpicEnum<TEnum> where TEnum : IEpicEnumValue
{

}
