// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.UnitTests
{
    using DataFilters.Casing;

    using FsCheck;
    using FsCheck.Xunit;

    using System;

    using Xunit.Categories;

    [UnitTest]
    public class DataFilterOptionsTests
    {
        private readonly DataFilterOptions _sut;

        public DataFilterOptionsTests()
        {
            _sut = new();
        }

        [Property]
        public Property Given_positive_integer_value_MaxCacheSize_should_be_set_with_value(PositiveInt input)
        {
            // Act
            _sut.MaxCacheSize = input.Item;

            // Assert
            return (_sut.MaxCacheSize == input.Item).ToProperty();
        }

        [Property(Arbitrary = new[] { typeof(Generators) })]
        public Property Given_options_Validate_should_check_all_values_are_valid(int maxCacheSize, PropertyNameResolutionStrategy strategy)
        {
            // Arrange
            Lazy<DataFilterOptions> lazy = new(() =>
            {
                _sut.PropertyNameResolutionStrategy = strategy;
                _sut.MaxCacheSize = maxCacheSize;

                _sut.Validate();

                return _sut;
            });

            // Assert
            return Prop.Throws<DataFiltersOptionsInvalidValueException, DataFilterOptions>(lazy)
                       .When(maxCacheSize < 1).Label($"{nameof(maxCacheSize)} is '{maxCacheSize}'")
                       .Or((_sut.PropertyNameResolutionStrategy == strategy).And(_sut.MaxCacheSize == maxCacheSize));
        }
    }
}
