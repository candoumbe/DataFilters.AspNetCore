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

    using Moq;

    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq.Expressions;

    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Categories;

    using static Microsoft.AspNetCore.Http.HttpMethods;

    [UnitTest]
    public class PreferMinimalActionFilterAttributeTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public PreferMinimalActionFilterAttributeTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
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
                StringValues preferHeaderValue = new ("return=minimal");
                string[] methods = { Get, Post, Put, Patch };
                foreach (string method in methods)
                {
                    yield return new object[]
                    {
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
                    };
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
            DefaultHttpContext httpContext = new();
            httpContext.Request.Method = method;
            headers.ForEach(header => httpContext.Request.Headers.TryAdd(header.Key, header.Value));

            ActionContext actionContext = new(
               httpContext,
               new Mock<RouteData>().Object,
               new Mock<ActionDescriptor>().Object,
               new ModelStateDictionary());

            ActionExecutedContext actionExecutedContext = new(actionContext,
                                                              new List<IFilterMetadata>(),
                                                              new Mock<object>())
            {
                Result = new OkObjectResult(actual)
            };

            PreferMinimalActionFilterAttribute sut = new();

            // Act
            sut.OnActionExecuted(actionExecutedContext);

            // Assert
            IActionResult result = actionExecutedContext.Result;

            result.Should()
                  .BeAssignableTo<ObjectResult>().Which.Value
                  .Should().Match(expectedResult, reason);
        }

        internal record FooWithMinimalProps
        {
            [Minimal]
            public string Prop1 { get; set; } = nameof(Prop1);

            public Baz Baz { get; set; } = new();
        }

        internal record Baz
        {
            public string Prop1 { get; set; } = nameof(Prop1);

            [Minimal]
            public string Prop2 { get; set; } = nameof(Prop2);

        }
    }
}
