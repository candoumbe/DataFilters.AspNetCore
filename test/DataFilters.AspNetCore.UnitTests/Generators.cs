// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.UnitTests
{
    using DataFilters.Casing;

    using FsCheck;

    using System.Collections.Generic;

    /// <summary>
    /// Collection of FsScheck generators.
    /// </summary>
    public static class Generators
    {
        /// <summary>
        /// Generator of <see cref="PropertyNameResolutionStrategy"/> instances.
        /// </summary>
        public static Arbitrary<PropertyNameResolutionStrategy> Arbitrary_PropertyNameResolutionStrategies()
        {
            IEnumerable<Gen<PropertyNameResolutionStrategy>> generators = new[]
            {
                Gen.Constant(PropertyNameResolutionStrategy.Default),
                Gen.Constant(PropertyNameResolutionStrategy.CamelCase),
                Gen.Constant(PropertyNameResolutionStrategy.PascalCase),
                Gen.Constant(PropertyNameResolutionStrategy.SnakeCase),
            };

            return Gen.OneOf(generators).ToArbitrary();
        }
    }
}
