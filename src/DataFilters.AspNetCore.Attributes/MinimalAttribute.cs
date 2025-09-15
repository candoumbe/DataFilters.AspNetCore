// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

using System;

namespace DataFilters.AspNetCore.Attributes;

/// <summary>
/// This attribute can be used to mark a property to include in HTTP reponses to requests that include the HTTP header <c>Prefer:return=minimal</c>.
/// </summary>
[AttributeUsage(validOn: AttributeTargets.Property)]
public sealed class MinimalAttribute : Attribute;