// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.UnitTests
{
    using DataFilters.Grammar.Syntax;

    using FluentAssertions;

    using System;
    using System.Collections.Generic;
    using System.Text;

    using Xunit;
    using Xunit.Abstractions;

    public class DefaultDataFilterServiceTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly DefaultDataFilterService _sut;

        public DefaultDataFilterServiceTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _sut = new DefaultDataFilterService(new DataFilterOptions());
        }

        public static IEnumerable<object[]> ComputeCases
        {
            get
            {
                yield return new object[]
                {
                    "Nickname=Bat",
                    new Filter("Nickname", @operator: FilterOperator.EqualTo, "Bat")
                };
            }
        }

        [Theory]
        [MemberData(nameof(ComputeCases))]
        public void Given_input_Compute_should_build_expected_Filter(string input, IFilter expected)
        {
            // Act
            IFilter actual = _sut.Compute<SuperHero>(input);

            // Assert
            actual.Should()
                  .Be(expected);
        }

        [Fact]
        public void Given_options_is_null_Constructor_should_throw_ArgumentNullException()
        {
            // Act
            Action ctorWhereOptionsIsNull = () => new DefaultDataFilterService(null);

            // Assert
            ctorWhereOptionsIsNull.Should()
                                  .ThrowExactly<ArgumentNullException>();
        }
    }
}
