// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.Filters
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Primitives;

    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;

    using static Microsoft.AspNetCore.Http.HttpMethods;

    /// <summary>
    /// An <see cref="ActionFilterAttribute"/> implementation that allows to select which properties an object
    /// should output when the action completed successfully.
    ///
    /// <para>
    /// By default, this filter will only apply to responses that follow "GET" requests
    /// </para>
    /// </summary>
    /// <remarks>
    /// This action filter will only apply <strong>AFTER</strong> the code of inside the controller action has was runned controller :
    /// <list type="number">
    ///     <item> the custom HTTP header with the specified name <see cref="FieldSelectorHeaderName"/> was present on the incoming request </item>
    ///     <item>the action returned an <see cref="OkObjectResult"/>. </item>
    ///     <item> at least one of the following conditions are met :
    ///         <list type="bullet">
    ///             <item><see cref="OnGet"/> is <c>true</c> and the <see cref="IsGet(string)"/> returns <c>true</c> for the HTTP method of the incoming request.</item>
    ///             <item><see cref="OnPost"/> is <c>true</c> and the <see cref="IsPost(string)"/> returns <c>true</c> for the HTTP method of the incoming request</item>
    ///             <item><see cref="OnPatch"/> is <c>true</c> and the <see cref="IsPatch(string)"/> returns <c>true</c> for the HTTP method of the incoming request</item>
    ///             <item><see cref="OnPut"/> is <c>true</c> and the incoming request's method is 'P'</item>
    ///         </list>
    ///     </item>
    /// </list>
    /// </remarks>
    public class SelectPropertiesActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Name of the header that defines which properties to let go through
        /// </summary>
        public const string FieldSelectorHeaderName = "x-fields-selection";

        /// <summary>
        /// Enables/disables the current filter for applied the filter on actions triggered by HTTP "GET" requests
        /// </summary>
        public bool OnGet { get; }

        /// <summary>
        /// Enables/disables the current filter for applied the filter on actions triggered by HTTP "POST" requests
        /// </summary>
        public bool OnPost { get; }

        /// <summary>
        /// Enables/disables the current filter for applied the filter on actions triggered by HTTP "PUT" requests
        /// </summary>
        public bool OnPatch { get; }

        /// <summary>
        /// Enables/disables the current filter for applied the filter on actions triggered by HTTP "PUT" requests
        /// </summary>
        public bool OnPut { get; }

        /// <summary>
        /// Builds a new <see cref="SelectPropertiesActionFilterAttribute"/> instance.
        /// </summary>
        /// <remarks>
        /// By default, only responses following "GET" requests will be handled.
        /// </remarks>
        /// <param name="onGet">Enable/disable selecting properties on "GET" body's responses</param>
        /// <param name="onPost">Enable/disable selecting properties on "POST" body's responses</param>
        /// <param name="onPatch">Enable/disable selecting properties on "PATCH" body's responses</param>
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
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Request.Headers.TryGetValue(FieldSelectorHeaderName, out StringValues fields)
                && fields.AtLeastOnce(field => !string.IsNullOrWhiteSpace(field))
                && context.Result is OkObjectResult objectResult)
            {
                string method = context.HttpContext.Request.Method;
                if ((OnGet && IsGet(method))
                    || (OnPost && IsPost(method))
                    || (OnPatch && IsPatch(method))
                    || (OnPut && IsPut(method))
                    )
                {
                    object obj = objectResult.Value;
                    IEnumerable<PropertyInfo> propertyInfos = obj.GetType()
                                                                 .GetProperties()
                                                                 .Where(pi => fields.Any(field => field.Equals(pi.Name, StringComparison.OrdinalIgnoreCase)));

                    ExpandoObject after = new();

                    propertyInfos.ForEach(prop => after.TryAdd(prop.Name, prop.GetValue(obj)));

                    context.Result = new OkObjectResult(after);
                }
            }
        }
    }
}
