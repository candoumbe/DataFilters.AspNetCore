// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.UnitTests.Attributes;

using DataFilters.AspNetCore.Attributes;

using FluentAssertions;

using System;

using Xunit;
using Xunit.Categories;

[UnitTest]
public class MinimalAttributeTests
{
    [Fact]
    public void Should_be_attribute_applicable_to_properties_and_fields()
    {
        // Act
        Type minimalAttribute = typeof(MinimalAttribute);

        // Assert
        AttributeUsageAttribute attr = minimalAttribute.Should()
                                                       .BeDecoratedWith<AttributeUsageAttribute>().Which;
        attr.AllowMultiple.Should().BeFalse();
        attr.Inherited.Should().BeFalse();
        attr.ValidOn.Should()
                    .Be(AttributeTargets.Property);
    }
}
