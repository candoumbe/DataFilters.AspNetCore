// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.Filters;

using DataFilters.AspNetCore.Attributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

using static Microsoft.AspNetCore.Http.HttpMethods;


/// <summary>
/// This action filter implementation handles the HTTP <c>Prefer</c> header
/// </summary>
public class PreferMinimalActionFilterAttribute : ActionFilterAttribute
{
    /// <summary>
    /// Name of the <c>Prefer</c> header.
    /// </summary>
    public const string PreferHeaderName = "Prefer";

    ///<inheritdoc/>
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (CanHandleRequest(context.HttpContext.Request.Method) && context.Result is OkObjectResult result)
        {
            ExpandoObject expando = ExtractProperties(result.Value);

            context.Result = new OkObjectResult(expando);
        }

        static bool CanHandleRequest(in string method) => IsGet(method)
                                                       || IsPost(method)
                                                       || IsPatch(method)
                                                       || IsPut(method);

        static ExpandoObject ExtractProperties(object obj)
        {
            ExpandoObject expando = new();
            IEnumerable<PropertyInfo> pis = obj.GetType().GetProperties()
                                               .Where(pi => pi.CanRead);

            pis.ForEach(pi =>
            {
                if(pi.CustomAttributes.Any(attr => attr.AttributeType == typeof(MinimalAttribute)))
                {
                    expando.TryAdd(pi.Name, pi.GetValue(obj));
                }
                else if(!(pi.PropertyType.IsPrimitive || pi.PropertyType == typeof(string)))
                {
                    expando.TryAdd(pi.Name, ExtractProperties(pi.GetValue(obj)));
                }
            });

            return expando;
        }
    }
}
