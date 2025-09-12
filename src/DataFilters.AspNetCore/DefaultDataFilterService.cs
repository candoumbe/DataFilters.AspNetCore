// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

using Microsoft.Extensions.Caching.Memory;

using System;

namespace DataFilters.AspNetCore
{
    /// <summary>
    /// <see cref="IDataFilterService"/> implementation that uses a local LRU (Least Recently Used) cache.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This service is designed for scenarios where you need to efficiently build <see cref="IFilter"/> instances from various inputs.
    /// </para>
    /// <para>
    /// Here's how to use the service:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <term>Define the <see cref="DataFilterOptions"/> to use when building <see cref="IFilter"/> instances.</term>
    /// <description>
    /// <code language="csharp">
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
    /// </description>
    /// </item>
    /// <item>
    /// <term>Create a <see cref="DefaultDataFilterService"/> instance with the <see cref="DataFilterOptions"/>.</term>
    /// <description>
    /// <code language="csharp">
    /// IDataFilterService service = new DefaultDataFilterService(options);
    /// </code>
    /// </description>
    /// </item>
    /// <item>
    /// <term>Use the service to create <see cref="IFilter"/> instances.</term>
    /// <description>
    /// <code language="csharp">
    /// string query = "Firstname=B*&amp;Lastname=Wayne";
    /// 
    /// IFilter filter = service.Compute&lt;Person&gt;(query);
    /// </code>
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class DefaultDataFilterService : IDataFilterService
    {
        private readonly DataFilterOptions _options;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Builds a new <see cref="DefaultDataFilterService"/> instance.
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is <c>null</c>.</exception>
        public DefaultDataFilterService(DataFilterOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = options.MaxCacheSize });
        }

        /// <inheritdoc />
        public IFilter Compute<T>(string input) => Compute<T>(input, null);

        ///<inheritdoc/>
        public IFilter Compute<T>(string input, FilterOptions filterComputationOptions)
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