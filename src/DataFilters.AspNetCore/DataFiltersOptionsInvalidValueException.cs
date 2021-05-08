
namespace DataFilters.AspNetCore
{

    using System;
    using System.Runtime.Serialization;

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
        protected DataFiltersOptionsInvalidValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
