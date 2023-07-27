using System.Runtime.Serialization;

namespace EpicEnums.Exceptions;
public abstract class EpicEnumException : Exception
{
    protected EpicEnumException()
    {
    }

    protected EpicEnumException(string message) : base(message)
    {
    }

    protected EpicEnumException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    protected EpicEnumException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
