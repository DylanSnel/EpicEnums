using System.Runtime.Serialization;

namespace EpicEnums.Exceptions;
public class UnsupportedValueException : EpicEnumException
{
    public UnsupportedValueException()
    {
    }

    public UnsupportedValueException(string message) : base(message)
    {
    }

    public UnsupportedValueException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public UnsupportedValueException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
