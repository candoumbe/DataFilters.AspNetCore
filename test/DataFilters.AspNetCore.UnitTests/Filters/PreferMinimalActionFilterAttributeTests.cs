namespace DataFilters.AspNetCore.UnitTests.Filters
{
    using DataFilters.AspNetCore.Attributes;
    using DataFilters.AspNetCore.Filters;

    using FluentAssertions;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Primitives;
    using NSubstitute;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq.Expressions;
    using FluentAssertions.Equivalency.Tracing;
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Categories;

    using static Microsoft.AspNetCore.Http.HttpMethods;

    [UnitTest]
    public class PreferMinimalActionFilterAttributeTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly PreferMinimalActionFilterAttribute _sut;

        public PreferMinimalActionFilterAttributeTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            _sut = new PreferMinimalActionFilterAttribute();
        }

        [Fact]
        public void Type_should_be_an_ActionFilterAttribute()
        {
            Type selectPropertiesAttribute = typeof(PreferMinimalActionFilterAttribute);

            // Assert
            selectPropertiesAttribute.Should()
                                     .NotBeAbstract().And
                                     .NotBeStatic().And
                                     .HaveDefaultConstructor().And
                                     .HaveAccessModifier(FluentAssertions.Common.CSharpAccessModifier.Public);

            selectPropertiesAttribute.Should()
                                     .BeDerivedFrom<ActionFilterAttribute>();
        }

        public static IEnumerable<object[]> OkObjectResultCases
        {
            get
            {
                StringValues preferHeaderValue = new("return=minimal");
                string[] methods = [Get, Post, Put, Patch];
                foreach (string method in methods)
                {
                    yield return
                    [
                        method,
                        new HeaderDictionary(new Dictionary<string, StringValues>
                        {
                            [PreferMinimalActionFilterAttribute.PreferHeaderName] = preferHeaderValue
                        }),
                        new FooWithMinimalProps(),
                        (Expression<Func<ExpandoObject, bool>>)(expando => expando != null && expando.Exactly(2)
                                                                           && expando.Once(kv => kv.Key == nameof(FooWithMinimalProps.Prop1))
                                                                           && expando.Once(kv => kv.Key == nameof(FooWithMinimalProps.Baz))
                                                               ),
                        $"The filter is configured to support HTTP verb '{method}' is supported and '{PreferMinimalActionFilterAttribute.PreferHeaderName}' header is set to {preferHeaderValue}"
                    ];
                }
            }
        }

        [Theory]
        [MemberData(nameof(OkObjectResultCases))]
        public void Given_request_with_Prefer_header_When_header_value_is_return_eq_minimal_and_controller_returns_OkObjectResult_Then_attribute_should_behave_as_expected(string method,
                                                                                                                                                                           IHeaderDictionary headers,
                                                                                                                                                                           object actual,
                                                                                                                                                                           Expression<Func<ExpandoObject, bool>> expectedResult,
                                                                                                                                                                           string reason)
        {
            // Arrange
            DefaultHttpContext httpContext = new()
            {
                Request = { Method = method }
            };
            headers.ForEach(header => httpContext.Request.Headers.TryAdd(header.Key, header.Value));

            ActionContext actionContext = new(
               httpContext,
               Substitute.For<RouteData>(),
               Substitute.For<ActionDescriptor>(),
               new ModelStateDictionary());

            ActionExecutedContext actionExecutedContext = new(actionContext,
                                                              new List<IFilterMetadata>(),
                                                              Substitute.For<object>())
            {
                Result = new OkObjectResult(actual)
            };

            // Act
            _sut.OnActionExecuted(actionExecutedContext);

            // Assert
            IActionResult result = actionExecutedContext.Result;

            result.Should()
                  .BeAssignableTo<ObjectResult>().Which.Value
                  .Should().Match(expectedResult, reason);
        }

        public static TheoryData<string, IHeaderDictionary, object, object, string> MissingOrIncorrectHeaderCases
        {
            get
            {
                (StringValues preferHeaderValues, string reason)[] preferHeaderValuesAndReason =  [
                    (new StringValues(),  "The header as no value set"),
                    (new StringValues("return=representation"), "The header's value is `representation` which should not activate the filter."),
                    (new StringValues(["return=representation", "return=minimal"]), "The header has both minimal and representation values")
                ];
                string[] methods = [Get, Post, Put, Patch];

                TheoryData<string, IHeaderDictionary, object, object, string> cases = new();

                foreach ((StringValues preferHeaderValue, string reason) in preferHeaderValuesAndReason)
                {
                    foreach (string method in methods)
                    {
                        cases.Add(
                            method,
                            new HeaderDictionary(new Dictionary<string, StringValues>
                            {
                                [PreferMinimalActionFilterAttribute.PreferHeaderName] = preferHeaderValue
                            }),
                            new FooWithMinimalProps(),
                            new FooWithMinimalProps(),
                            reason
                        );
                    }
                }

                return cases;
            }
        }

        [Theory]
        [MemberData(nameof(MissingOrIncorrectHeaderCases))]
        public void Given_request_with_Prefer_header_When_Prefer_header_is_not_present_or_has_incorrect_value__Then_attribute_not_do_anything(string method,
                                                                                                                                              IHeaderDictionary headers,
                                                                                                                                              object actual,
                                                                                                                                              object expected,
                                                                                                                                              string reason)
        {
            // Arrange
            DefaultHttpContext httpContext = new()
            {
                Request = { Method = method }
            };
            headers.ForEach(header => httpContext.Request.Headers.TryAdd(header.Key, header.Value));

            ActionContext actionContext = new(
               httpContext,
               Substitute.For<RouteData>(),
               Substitute.For<ActionDescriptor>(),
               new ModelStateDictionary());

            ActionExecutedContext actionExecutedContext = new(actionContext, filters: [], controller: Substitute.For<object>())
            {
                Result = new OkObjectResult(actual)
            };

            // Act
            _sut.OnActionExecuted(actionExecutedContext);

            // Assert
            IActionResult result = actionExecutedContext.Result;

            result.Should()
                  .BeAssignableTo<ObjectResult>().Which.Value
                  .Should().BeEquivalentTo(expected);
        }

        private record FooWithMinimalProps
        {
            [Minimal]
            public string Prop1 { get; set; } = nameof(Prop1);

            public Baz Baz { get; set; } = new();
        }

        private record Baz
        {
            public string Prop1 { get; set; } = nameof(Prop1);

            [Minimal]
            public string Prop2 { get; set; } = nameof(Prop2);

        }
    }
}
