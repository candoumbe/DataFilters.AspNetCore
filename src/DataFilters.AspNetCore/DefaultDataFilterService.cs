// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

using Microsoft.Extensions.Caching.Memory;

using System;

namespace DataFilters.AspNetCore
{
    /// <summary>
    /// Default <see cref="IDataFilterService"/> implementation with a local L.R.U cache
    /// </summary>
    public class DefaultDataFilterService : IDataFilterService
    {
        private readonly DataFilterOptions _options;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Builds a new <see cref="DefaultDataFilterService"/>
        /// </summary>
        /// <param name="options"></param>
        public DefaultDataFilterService(DataFilterOptions options)
        {
            _options = options;
            _cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = options.MaxCacheSize });
        }

        ///<inheritdoc/>
        public IFilter Compute<T>(string input)
        {
            string key = $"{typeof(T).FullName}_{input}";
            if (!_cache.TryGetValue(key, out IFilter filter))
            {
                filter = input.ToFilter<T>();
                _cache.Set(key, filter, new MemoryCacheEntryOptions { Priority = CacheItemPriority.Low, Size = 1 });
            }
            return filter;
        }
    }
}
