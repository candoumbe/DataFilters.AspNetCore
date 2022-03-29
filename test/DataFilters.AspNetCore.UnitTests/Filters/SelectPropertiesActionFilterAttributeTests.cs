namespace DataFilters.AspNetCore.UnitTests.Filters
{
    using Bogus;

    using DataFilters.AspNetCore.Filters;

    using FluentAssertions;

    using FsCheck;
    using FsCheck.Xunit;

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
    using System.Linq;
    using System.Linq.Expressions;

    using Xunit;
    using Xunit.Abstractions;

    using static Microsoft.AspNetCore.Http.HttpMethods;

    public class SelectPropertiesActionFilterAttributeTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public SelectPropertiesActionFilterAttributeTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Property]
        public Property Ctor_should_set_properties_accordingly(bool onGet, bool onPost, bool onPut, bool onPatch)
        {
            // Act
            SelectPropertiesActionFilterAttribute attribute = new(onGet, onPost, onPatch, onPut);

            // Assert
            return (attribute.OnGet == onGet).And(attribute.OnPut == onPut)
                                             .And(attribute.OnPost == onPost)
                                             .And(attribute.OnPatch == onPatch);
        }

        [Fact]
        public void Type_should_be_an_ActionFilterAttribute()
        {
            Type selectPropertiesAttribute = typeof(SelectPropertiesActionFilterAttribute);

            // Assert
            selectPropertiesAttribute.Should()
                                     .NotBeAbstract().And
                                     .NotBeStatic().And
                                     .HaveConstructor(new[] { typeof(bool), typeof(bool), typeof(bool), typeof(bool) }).And
                                     .HaveAccessModifier(FluentAssertions.Common.CSharpAccessModifier.Public);

            selectPropertiesAttribute.Should()
                                     .BeDerivedFrom<ActionFilterAttribute>();
        }

        [Property]
        public void Given_inputs_constructor_should_set_properties_accordingly(bool onGet, bool onPost, bool onPut, bool onPatch)
        {
            _outputHelper.WriteLine($"{nameof(onGet)} : {onGet}");
            _outputHelper.WriteLine($"{nameof(onPost)} : {onPost}");
            _outputHelper.WriteLine($"{nameof(onPut)} : {onPut}");
            _outputHelper.WriteLine($"{nameof(onPatch)} : {onPatch}");

            // Act
            SelectPropertiesActionFilterAttribute attribute = new(onGet: onGet, onPost: onPost, onPatch: onPatch, onPut: onPut);

            // Assert
            attribute.OnGet.Should().Be(onGet);
            attribute.OnPost.Should().Be(onPost);
            attribute.OnPut.Should().Be(onPut);
            attribute.OnPatch.Should().Be(onPatch);
        }

        public static IEnumerable<object[]> OkObjectResultCases
        {
            get
            {
                yield return new object[]
                {
                    Get,
                    new HeaderDictionary(new Dictionary<string, StringValues>
                    {
                        [SelectPropertiesActionFilterAttribute.IncludeFieldSelectorHeaderName] = new StringValues("prop")
                    }),
                    new
                    {
                        prop = "value",
                        prop2 = new
                        {
                            subProp = 1,
                            subProp2 = 2
                        }
                    },
                    (Expression<Func<IEnumerable<string>, bool>>)(props => props.Exactly(1)
                                                                           && props.Once(propName => propName =="prop")
                    ),
                    $"The filter is configured to support HTTP verb '{Get}' is supported and '{SelectPropertiesActionFilterAttribute.IncludeFieldSelectorHeaderName}' header is set to 'prop'"
                };

                yield return new object[]
                {
                    Get,
                    new HeaderDictionary(new Dictionary<string, StringValues>
                    {
                        [SelectPropertiesActionFilterAttribute.ExcludeFieldSelectorHeaderName] = new StringValues("prop")
                    }),
                    new
                    {
                        prop = "value",
                        prop2 = new
                        {
                            subProp = 1,
                            subProp2 = 2
                        },
                    },
                    (Expression<Func<IEnumerable<string>, bool>>)(props => props.Exactly(1)
                                                                           && props.Once(propName => propName =="prop2")
                    ),
                    $"The filter is configured to support HTTP '{Get}' is supported and '{SelectPropertiesActionFilterAttribute.ExcludeFieldSelectorHeaderName}' header is set to 'prop'"
                };
            }
        }

        [Theory]
        [MemberData(nameof(OkObjectResultCases))]
        public void Given_request_with_custom_selection_headers_and_controller_returned_OkObjectResult_filter_should_perform_selection(string method,
                                                                                                                                       IHeaderDictionary headers,
                                                                                                                                       object okResultValue,
                                                                                                                                       Expression<Func<IEnumerable<string>, bool>> expectation,
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
                Result = new OkObjectResult(okResultValue)
            };

            SelectPropertiesActionFilterAttribute sut = new();

            // Act
            sut.OnActionExecuted(actionExecutedContext);

            // Assert
            IActionResult result = actionExecutedContext.Result;

            IEnumerable<string> fieldNames = result.Should()
                                                   .BeAssignableTo<ObjectResult>().Which.Value.Should()
                                                   .BeAssignableTo<ExpandoObject>().Which
                                                   .Select(kv => kv.Key);

            fieldNames.Should()
                      .Match(expectation, reason);
        }

        [Property]
        public void Given_request_without_custom_selection_headers_and_controller_returned_OkObjectResult_filter_should_perform_no_action(NonWhiteSpaceString method)
        {
            // Arrange
            DefaultHttpContext httpContext = new();
            httpContext.Request.Method = method.Item;

            ActionContext actionContext = new(
               httpContext,
               new Mock<RouteData>().Object,
               new Mock<ActionDescriptor>().Object,
               new ModelStateDictionary());

            OkObjectResult okObjectResult = new(new
            {
                prop = "value",
                prop2 = new
                {
                    subProp = 1,
                    subProp2 = 2
                }
            });
            ActionExecutedContext actionExecutedContext = new(actionContext,
                                                              new List<IFilterMetadata>(),
                                                              new Mock<object>())
            {
                Result = okObjectResult
            };

            SelectPropertiesActionFilterAttribute sut = new();

            // Act
            sut.OnActionExecuted(actionExecutedContext);

            // Assert
            IActionResult result = actionExecutedContext.Result;

            result.Should()
                  .Be(okObjectResult);
        }

        [Property]
        public Property Given_both_http_headers_for_including_and_excluding_fields_are_specified_OnActionExecuting_should_return_BadRequestObjectResult_with_the_specified_message(bool onGet,
                                                                                                                                                                                   bool onPost,
                                                                                                                                                                                   bool onPut,
                                                                                                                                                                                   bool onPatch,
                                                                                                                                                                                   NonWhiteSpaceString property)
        {
            // Arrange
            Faker faker = new();
            string method = faker.PickRandom(Get, Post, Put, Patch, Head, Connect, Delete, Options, Trace);
            SelectPropertiesActionFilterAttribute sut = new(onGet, onPost, onPatch, onPut);

            _outputHelper.WriteLine($"{nameof(SelectPropertiesActionFilterAttribute.OnGet)} : {sut.OnGet}");
            _outputHelper.WriteLine($"{nameof(SelectPropertiesActionFilterAttribute.OnPost)} : {sut.OnPost}");
            _outputHelper.WriteLine($"{nameof(SelectPropertiesActionFilterAttribute.OnPut)} : {sut.OnPut}");
            _outputHelper.WriteLine($"{nameof(SelectPropertiesActionFilterAttribute.OnPatch)} : {sut.OnPatch}");

            DefaultHttpContext httpContext = new();
            httpContext.Request.Method = method;
            httpContext.Request.Headers.Add(SelectPropertiesActionFilterAttribute.ExcludeFieldSelectorHeaderName, property.Item);
            httpContext.Request.Headers.Add(SelectPropertiesActionFilterAttribute.IncludeFieldSelectorHeaderName, property.Item);

            ActionContext actionContext = new(httpContext,
                                              new Mock<RouteData>().Object,
                                              new Mock<ActionDescriptor>().Object,
                                              new ModelStateDictionary());

            ActionExecutingContext actionExecutingContext = new(actionContext,
                                                                new List<IFilterMetadata>(),
                                                                new Dictionary<string, object>(),
                                                                new Mock<object>().Object);

            // Act
            sut.OnActionExecuting(actionExecutingContext);

            // Assert
            return (actionExecutingContext.Result is BadRequestObjectResult).Label($"{nameof(SelectPropertiesActionFilterAttribute.OnGet)} : {sut.OnGet}")
                                                                     .When((sut.OnGet && IsGet(method))
                                                                           || (sut.OnPost && IsPost(method))
                                                                           || (sut.OnPut && IsPut(method))
                                                                           || (sut.OnPatch && IsPatch(method)))
                                                                     ;
        }

        [Property]
        public void Given_both_http_headers_for_including_and_excluding_fields_are_specified_OnActionExecuted_should_throw_InvalidOperationException(bool onGet,
                                                                                                                                                     bool onPost,
                                                                                                                                                     bool onPut,
                                                                                                                                                     bool onPatch,
                                                                                                                                                     NonWhiteSpaceString property,
                                                                                                                                                     object okObjectResultInnerValue)
        {
            // Arrange
            Faker faker = new();
            string method = faker.PickRandom(Get, Post, Put, Patch, Head, Connect, Delete, Options, Trace);
            SelectPropertiesActionFilterAttribute sut = new(onGet, onPost, onPatch, onPut);

            DefaultHttpContext httpContext = new();
            httpContext.Request.Method = method;
            httpContext.Request.Headers.Add(SelectPropertiesActionFilterAttribute.ExcludeFieldSelectorHeaderName, property.Item);
            httpContext.Request.Headers.Add(SelectPropertiesActionFilterAttribute.IncludeFieldSelectorHeaderName, property.Item);

            ActionContext actionContext = new(
               httpContext,
               new Mock<RouteData>().Object,
               new Mock<ActionDescriptor>().Object,
               new ModelStateDictionary());

            OkObjectResult okObjectResult = new(okObjectResultInnerValue);
            ActionExecutedContext actionExecutedContext = new(actionContext,
                                                              new List<IFilterMetadata>(),
                                                              new Mock<object>())
            {
                Result = okObjectResult
            };

            // Act
            Action onActionExecuted = () => sut.OnActionExecuted(actionExecutedContext);

            // Assert
            onActionExecuted.Should()
                            .ThrowExactly<InvalidOperationException>();
        }
    }
}
