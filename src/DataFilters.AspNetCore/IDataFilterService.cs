// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore
{
    /// <summary>
    /// A service that can built and cache <see cref="IFilter"/>
    /// </summary>
    public interface IDataFilterService
    {
        /// <summary>
        /// Computes the filter.
        /// </summary>
        /// <typeparam name="T">Type onto which the resulting filter should be applied</typeparam>
        /// <param name="filter">Query string that describe the filter to apply</param>
        /// <returns>an <see cref="IFilter"/> instance</returns>
        IFilter Compute<T>(string filter);
    }
}
