using System;
using System.Runtime.Serialization;

namespace DataFilters.AspNetCore;

/// <summary>
/// Exception thrown whenever a property of <see cref="DataFilterOptions"/> is set with a invalid value.
/// </summary>
[Serializable]
public class DataFiltersOptionsInvalidValueException : Exception
{
    ///<inheritdoc/>
    public DataFiltersOptionsInvalidValueException()
    {
    }

    ///<inheritdoc/>
    public DataFiltersOptionsInvalidValueException(string message) : base(message)
    {
    }

    ///<inheritdoc/>
    public DataFiltersOptionsInvalidValueException(string message, Exception innerException) : base(message, innerException)
    {
    }

    ///<inheritdoc/>
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "S1123")]
#endif
    protected DataFiltersOptionsInvalidValueException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}