// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.Attributes;

using System;

/// <summary>
/// This attribute can be used to mark a property so that 
/// when it will be rendered when the HTTP header <c>Prefer:return=minimal</c>.
/// </summary>
[AttributeUsage(validOn: AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class MinimalAttribute : Attribute
{
}
