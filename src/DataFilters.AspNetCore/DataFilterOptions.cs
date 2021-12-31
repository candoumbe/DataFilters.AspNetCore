
namespace DataFilters.AspNetCore
{
    using DataFilters.Casing;

    /// <summary>
    /// <see cref="DataFilterOptions"/> allows to customize the behavior of <see cref="IDataFilterService"/>
    /// </summary>
    public class DataFilterOptions
    {
        private const int DefaultCacheSize = 1_000;

        /// <summary>
        /// Defines how the IDataFilterService implementation will handle property names.
        /// </summary>
        /// <remarks>
        ///     The default value is <see cref="PropertyNameResolutionStrategy.Default"/>
        /// </remarks>
        public PropertyNameResolutionStrategy PropertyNameResolutionStrategy
        {
            get => _strategy;
            set => _strategy = value ?? PropertyNameResolutionStrategy.Default;
        }

        private PropertyNameResolutionStrategy _strategy;

        /// <summary>
        /// Defines the number of elements to keep in the local cache
        /// </summary>
        public int MaxCacheSize { get; set; }

        /// <summary>
        /// Builds a new <see cref="DataFilterOptions"/> instance with default values for each property.
        /// </summary>
        public DataFilterOptions()
        {
            MaxCacheSize = DefaultCacheSize;
            PropertyNameResolutionStrategy = PropertyNameResolutionStrategy.Default;
        }

        /// <summary>
        /// Validates all properties
        /// </summary>
        /// <remarks>
        /// This method should be called right after setting all properties.
        /// <para>
        /// <example>
        /// The last line will throw a <see cref="DataFiltersOptionsInvalidValueException"/> because <see cref="MaxCacheSize"/> must be a positive integer
        /// <code>
        /// DataFiltersOptions options = new ()
        /// {
        ///     MaxCacheSize = -3,
        ///     Strategy = PropertyNameResolutionStrategy.CamelCase
        /// };
        ///
        /// options.Validate();
        /// </code>
        /// </example>
        /// </para>
        /// </remarks>
        /// <exception cref="DataFiltersOptionsInvalidValueException">when <see cref="MaxCacheSize"/>'s value is negative or zero</exception>
        public void Validate()
        {
            if (MaxCacheSize < 1)
            {
                throw new DataFiltersOptionsInvalidValueException($"{MaxCacheSize} is not a valid value for {nameof(MaxCacheSize)}");
            }
        }
    }
}
