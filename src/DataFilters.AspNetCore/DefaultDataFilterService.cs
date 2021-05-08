// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore
{
    /// <summary>
    /// Default <see cref="IDataFilterService"/> implementation with a local L.R.U cache
    /// </summary>
    internal class DefaultDataFilterService : IDataFilterService
    {
        private readonly DataFilterOptions _options;

        /// <summary>
        /// Builds a new <see cref="DefaultDataFilterService"/>
        /// </summary>
        /// <param name="options"></param>
        public DefaultDataFilterService(DataFilterOptions options) => _options = options;
    }
}
