// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.Attributes;

using System;


/// <summary>
/// This attribute can be used to mark a property so that 
/// when it will be rendered when <c>Prefer:render=minimal</c>.
/// </summary>
[AttributeUsage(validOn: AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class MinimalAttribute : Attribute
{
}
