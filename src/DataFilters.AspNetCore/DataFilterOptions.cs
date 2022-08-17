
namespace DataFilters.AspNetCore
{
    using DataFilters.Casing;

    /// <summary>
    /// <see cref="DataFilterOptions"/> allows to customize the behavior of <see cref="IDataFilterService"/>
    /// </summary>
#if NET6_0_OR_GREATER
    public record DataFilterOptions
#else
    public class DataFilterOptions
#endif
    {
        private const long DefaultCacheSize = 1_000;

        /// <summary>
        /// Defines the number of elements to keep in the local cache
        /// </summary>
        /// <remarks>
        /// Setting this to a negative value means no cache will be used
        /// </remarks>
        public long MaxCacheSize { get; set; }

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
        /// Builds a new <see cref="DataFilterOptions"/> instance using the <see cref="DefaultCacheSize"/>.
        /// </summary>
        public DataFilterOptions()
        {
            MaxCacheSize = DefaultCacheSize;
            FilterOptions = new();
        }
    }
}
