// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

using Microsoft.Extensions.Caching.Memory;

using System;

namespace DataFilters.AspNetCore
{
    /// <summary>
    /// <see cref="IDataFilterService"/> implementation with a local L.R.U cache.
    /// </summary>
    /// <remarks>
    /// This service can be used wherever you need to build an <see cref="IFilter"/> instance for a given input as follow :
    /// <para>
    /// <example>
    /// 1. Define the <see cref="DataFilterOptions"/> to use when building <see cref="IFilter"/> instances.
    /// <code>
    /// DataFilterOptions options = new ()
    /// {
    ///     MaxCacheSize = 50,
    ///     FilterOptions = new ()
    ///     {
    ///         DefaultPropertyNameStrategyResolutionStrategy = PropertyNameResolutionStrategy.SnakeCase,
    ///         Logic = FilterLogic.And
    ///     }
    /// };
    /// </code>
    /// </example>
    /// </para>
    /// <para>
    /// <example>
    /// 2. Create a <see cref="DefaultDataFilterService"/> instance with the <see cref="DataFilterOptions"/>.
    /// <code>
    /// IDataFilterService service = new(options);
    /// </code>
    /// </example>
    /// </para>
    ///
    /// <para>
    /// <example>
    /// 3. The service can now be used to create <see cref="IFilter"/>s.
    ///
    /// <code>
    /// string query = "Firstname=B*&amp;Lastname=Wayne";
    ///
    /// IFilter filter = service.Compute&lt;Person&gt;(query);
    /// </code>
    /// </example>
    /// </para>
    /// </remarks>
    public class DefaultDataFilterService : IDataFilterService
    {
        private readonly DataFilterOptions _options;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Builds a new <see cref="DefaultDataFilterService"/>
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is <c>null</c>.</exception>
        public DefaultDataFilterService(DataFilterOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            _options = options;
            _cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = options.MaxCacheSize });
        }

        ///<inheritdoc/>
        public IFilter Compute<T>(string input, FilterOptions filterComputationOptions = null)
        {
            string key = $"{typeof(T).FullName}_{input}";

            if (!_cache.TryGetValue(key, out IFilter filter))
            {
                filter = input.ToFilter<T>(filterComputationOptions ?? _options.FilterOptions);
                _cache.Set(key, input, new MemoryCacheEntryOptions { Priority = CacheItemPriority.Low, Size = 1 });
            }

            return filter;
        }
    }
}
