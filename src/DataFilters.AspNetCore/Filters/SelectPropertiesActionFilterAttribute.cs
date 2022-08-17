// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

using static Microsoft.AspNetCore.Http.HttpMethods;

/// <summary>
/// An <see cref="ActionFilterAttribute"/> implementation that allows to specify which properties an object
/// should output when the action completed successfully.
/// <para>
/// By default, this filter will only apply to responses that follow "GET" requests
/// </para>
/// </summary>
/// <remarks>
/// This action filter will only apply <strong>AFTER</strong> the code inside the controller action has runned :
/// <list type="number">
///     <item> the custom HTTP header with the specified name <see cref="IncludeFieldSelectorHeaderName"/> and or <see cref="ExcludeFieldSelectorHeaderName"/> was present on the incoming request </item>
///     <item>the action returned an <see cref="OkObjectResult"/>. </item>
///     <item> at least one of the following conditions are met :
///         <list type="bullet">
///             <item><see cref="OnGet"/> is <see langword="true"/> and <see cref="IsGet(string)"/> returns <see langword="true"/> for the HTTP method of the incoming request.</item>
///             <item><see cref="OnPost"/> is <see langword="true"/> and <see cref="IsPost(string)"/> returns <see langword="true"/> for the HTTP method of the incoming request</item>
///             <item><see cref="OnPatch"/> is <see langword="true"/> and <see cref="IsPatch(string)"/> returns <see langword="true"/> for the HTTP method of the incoming request</item>
///             <item><see cref="OnPut"/> is <see langword="true"/> and <see cref="IsPut(string)"/> returns <see langword="true"/> for the HTTP method of the incoming request</item>
///         </list>
///     </item>
/// </list>
/// <para>
/// Specifying both <see cref="IncludeFieldSelectorHeaderName"/> and <see cref="ExcludeFieldSelectorHeaderName"/> is a undefined behaviour which will result in a <see cref="BadRequestObjectResult"/>.
/// </para>
/// </remarks>
public class SelectPropertiesActionFilterAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Name of the header that defines which properties that will appear in the result
    /// </summary>
    public const string IncludeFieldSelectorHeaderName = "x-datafilters-fields-include";

    /// <summary>
    /// Name of the http header that defines properties that <strong>will not</strong> appear in the result.
    /// </summary>
    public const string ExcludeFieldSelectorHeaderName = "x-datafilters-fields-exclude";


    /// <summary>
    /// Indicates if the selector is applied to reponses triggered by HTTP "GET" requests
    /// </summary>
    public bool OnGet { get; }

    /// <summary>
    /// Indicates if the selector is applied to reponses triggered by HTTP "POST" requests
    /// </summary>
    public bool OnPost { get; }

    /// <summary>
    /// Indicates if the selector is applied to reponses triggered by HTTP "PATCH" requests
    /// </summary>
    public bool OnPatch { get; }

    /// <summary>
    /// Indicates if the selector is applied to reponses triggered by HTTP "PUT" requests
    /// </summary>
    public bool OnPut { get; }

    /// <summary>
    /// Builds a new <see cref="SelectPropertiesActionFilterAttribute"/> instance.
    /// </summary>
    /// <remarks>
    /// By default, only responses following "GET" requests will be handled.
    /// </remarks>
    /// <param name="onGet">Enable/disable selecting/excluding properties on "GET" body's responses</param>
    /// <param name="onPost">Enable/disable selecting/excluding properties on "POST" body's responses</param>
    /// <param name="onPatch">Enable/disable selecting/excluding properties on "PATCH" body's responses</param>
    /// <param name="onPut">Enable/disable selecting properties on "PUT"'s body responses</param>
    public SelectPropertiesActionFilterAttribute(bool onGet = true,
                                                 bool onPost = false,
                                                 bool onPatch = false,
                                                 bool onPut = false)
    {
        OnGet = onGet;
        OnPost = onPost;
        OnPatch = onPatch;
        OnPut = onPut;
    }

    /// <inheritdoc/>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (ShouldActivate(context.HttpContext.Request.Method) && context.HttpContext.Request.Headers.ContainsKey(IncludeFieldSelectorHeaderName)
                                                               && context.HttpContext.Request.Headers.ContainsKey(ExcludeFieldSelectorHeaderName)
            )
        {
            ModelStateDictionary modelState = context.ModelState ?? new ModelStateDictionary();
            modelState.TryAddModelError("x-datafilters-fields", $"Cannot specify both '{IncludeFieldSelectorHeaderName}' and '{ExcludeFieldSelectorHeaderName}'");
            context.Result = new BadRequestObjectResult(modelState);
        }
    }

    /// <inheritdoc/>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        bool mustIncludeFields = context.HttpContext.Request.Headers.TryGetValue(IncludeFieldSelectorHeaderName, out StringValues fieldsToInclude) && fieldsToInclude.AtLeastOnce(field => !string.IsNullOrWhiteSpace(field));
        bool mustExcludeFields = (context.HttpContext.Request.Headers.TryGetValue(ExcludeFieldSelectorHeaderName, out StringValues fieldsToExclude) && fieldsToExclude.AtLeastOnce(field => !string.IsNullOrWhiteSpace(field)));

        if (mustIncludeFields && mustExcludeFields)
        {
            throw new InvalidOperationException($@"Only ""{IncludeFieldSelectorHeaderName}"" or ""{ExcludeFieldSelectorHeaderName}"" HTTP header can be set");
        }

        if ((mustIncludeFields || mustExcludeFields) && context.Result is OkObjectResult objectResult && (fieldsToInclude.AtLeastOnce() || fieldsToExclude.AtLeastOnce())
            )
        {
            string method = context.HttpContext.Request.Method;

            if (ShouldActivate(method))
            {
                object obj = objectResult.Value;

                IEnumerable<PropertyInfo> propertyToIncludeInfos = fieldsToInclude.AtLeastOnce()
                                                                    ? obj.GetType()
                                                                        .GetProperties()
                                                                        .Where(pi => fieldsToInclude.Any(field => field.Equals(pi.Name, StringComparison.OrdinalIgnoreCase)))
                                                                    : obj.GetType()
                                                                         .GetProperties()
                                                                         .Where(pi => !fieldsToExclude.Any(field => field.Equals(pi.Name, StringComparison.OrdinalIgnoreCase)));
                ExpandoObject after = new();

                propertyToIncludeInfos.ForEach(prop => after.TryAdd(prop.Name, prop.GetValue(obj)));

                context.Result = new OkObjectResult(after);
            }
        }
    }

    /// <summary>
    /// Checks if the current filter should activate itself
    /// </summary>
    /// <param name="method">HTTP method</param>
    /// <returns><see langword="true"/> if the attribute should run and <see langword="false"/> otherwise.</returns>
    private bool ShouldActivate(string method) => (OnGet && IsGet(method))
                                                    || (OnPost && IsPost(method))
                                                    || (OnPatch && IsPatch(method))
                                                    || (OnPut && IsPut(method));
}
