// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore
{
    using Microsoft.Extensions.DependencyInjection;

    using System;

    /// <summary>
    /// Extension methods to register services into the dependency injection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers <see cref="IDataFilterService"/> into the dependency injection container.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        [Obsolete("This method will be removed soon. Use AddDataFilters(DataFilterOptions options) instead.")]
        public static IServiceCollection AddDataFilters(this IServiceCollection services, Action<DataFilterOptions> configureOptions = null)
        {
            DataFilterOptions options = new();

            configureOptions?.Invoke(options);

            services.AddSingleton<IDataFilterService>(new DefaultDataFilterService(options));

            return services;
        }

        /// <summary>
        /// Registers <see cref="IDataFilterService"/> into the dependency injection container.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options">Options used to configure the service.</param>
        public static IServiceCollection AddDataFilters(this IServiceCollection services, DataFilterOptions options)
            => services.AddSingleton<IDataFilterService>(new DefaultDataFilterService(options));
    }
}
