using System.Runtime.Serialization;

namespace SampleApp.EpicEnums;
[Serializable]
internal class FruitNotSupportedException : Exception
{
    private FruitsEnum _fruit;

    public FruitNotSupportedException()
    {
    }

    public FruitNotSupportedException(FruitsEnum fruit)
    {
        _fruit = fruit;
    }

    public FruitNotSupportedException(string? message) : base(message)
    {
    }

    public FruitNotSupportedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected FruitNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
