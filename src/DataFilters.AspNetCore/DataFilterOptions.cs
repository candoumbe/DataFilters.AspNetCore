
namespace DataFilters.AspNetCore
{
    using DataFilters.Casing;

    /// <summary>
    /// <see cref="DataFilterOptions"/> allows to customize the behavior of <see cref="IDataFilterService"/>
    /// </summary>
    public class DataFilterOptions
    {
        private const int DefaultCacheSize = 1_000;


        private PropertyNameResolutionStrategy _strategy;

        /// <summary>
        /// Defines the number of elements to keep in the local cache
        /// </summary>
        public int MaxCacheSize { get; set; }

        /// <summary>
        /// Default options to use when computing <see cref="IFilter"/> instances.
        /// </summary>
        public FilterOptions FilterOptions
        {
            get => _filterOptions;
#if NET6_0_OR_GREATER
            init
#else
            set
#endif
                => _filterOptions = value ?? new ();
        }

        private FilterOptions _filterOptions;

        /// <summary>
        /// Builds a new <see cref="DataFilterOptions"/> instance with default values for each property.
        /// </summary>
        public DataFilterOptions()
        {
            MaxCacheSize = DefaultCacheSize;
            FilterOptions = new();
        }
    }
}
