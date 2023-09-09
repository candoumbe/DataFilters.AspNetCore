// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.UnitTests
{
    using FsCheck;
    using FsCheck.Xunit;

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
    }
}
