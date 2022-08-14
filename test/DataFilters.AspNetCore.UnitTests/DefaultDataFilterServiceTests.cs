// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.UnitTests
{

    using FluentAssertions;

    using System;
    using System.Collections.Generic;

    using Xunit;
    using Xunit.Abstractions;

    public class DefaultDataFilterServiceTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly DefaultDataFilterService _sut;

        public DefaultDataFilterServiceTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }


        public static IEnumerable<object[]> ComputeCases
        {
            get
            {
                yield return new object[]
                {
                    "Nickname=Bat",
                    new DataFilterOptions(),
                    new Filter("Nickname", @operator: FilterOperator.EqualTo, "Bat")
                };
                FilterLogic[] filterLogics = { FilterLogic.And, FilterLogic.Or };
                foreach (FilterLogic logic in filterLogics)
                {
                    yield return new object[]
                    {
                        $"{nameof(SuperHero.Nickname)}=Bat&{nameof(SuperHero.Nickname)}=*Wonder*",
                        new DataFilterOptions { FilterOptions = new FilterOptions() { Logic = logic } },
                        new MultiFilter
                        {
                            Logic = logic,
                            Filters = new[]
                            {
                                new Filter("Nickname", @operator: FilterOperator.EqualTo, "Bat"),
                                new Filter("Nickname", @operator: FilterOperator.Contains, "Wonder"),
                            }
                        }
                    };
                }
            }
        }

        [Theory]
        [MemberData(nameof(ComputeCases))]
        public void Given_input_and_options_Compute_should_build_expected_Filter_instance(string input, DataFilterOptions options, IFilter expected)
        {
            // Arrange
            DefaultDataFilterService sut = new(options);

            // Act
            IFilter actual = sut.Compute<SuperHero>(input);

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
