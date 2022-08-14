[![GitHub Workflow Status (main)](https://img.shields.io/github/workflow/status/candoumbe/datafilters.aspnetcore/deployment/main?label=main)](https://github.com/candoumbe/DataFilters.AspNetCore/actions/workflows/deployment.yml)
[![GitHub Workflow Status (develop)](https://img.shields.io/github/workflow/status/candoumbe/datafilters.aspnetcore/continuous/develop?label=develop)](https://github.com/candoumbe/DataFilters.AspNetCore/actions/workflows/continuous.yml)
[![codecov](https://codecov.io/gh/candoumbe/DataFilters.aspnetcore/branch/develop/graph/badge.svg?token=FHSC41A4X3)](https://codecov.io/gh/candoumbe/DataFilters.aspnetcore)
[![GitHub raw issues](https://img.shields.io/github/issues-raw/candoumbe/datafilters.aspnetcore)](https://github.com/candoumbe/datafilters.aspnetcore/issues)
[![Nuget](https://img.shields.io/nuget/vpre/datafilters.aspnetcore)](https://nuget.org/packages/datafilters)
#  DataFilters.AspNetCore <!-- omit in toc -->

**Table of contents**
- <a href='#why'>Why</a>
  - <a href='#build-ifilters'>Make it easier to build `IFilter` instances</a>
  - <a href='#reduce-bandwith-usage'>Reduce bandwith usage</a>
    - <a href='#custom-http-headers'>Custom HTTP headers</a>
    - <a href='#prefer-http-header-support'>`Prefer` HTTP header support</a>
- <a href='#how-to-install'>How to install</a>


A small library that ease usage of [DataFilters][datafilters-nupkg] with ASP.NET Core APIs.

## <a href="#" id="why">Why</a>

### <a href="#" id="build-ifilters">Make it easier to build `IFilter` instances</a>
[DataFilters][datafilters-nupkg] allows to build complex queries in a "restfull" way so 
However, it comes with some drawbacks.

In order to build a filter, you have to :

1. parse the incoming string
2. map it manually to an underlying model type.
3. converts it into an IFilter instance using the `ToFilter<T>` extension method.

This can be a tedious task and this library can help to ease that process.

### <a href="#" id="reduce-bandwith-usage">Reduce the bandwith usage</a>
The library can help reduce bandwith usage. This can be done in two  differnet ways : 
- using `x-datafilters-fields-include` / `x-datafilters-fields-exclude` custom HTTP headers
- using [`Prefer`] HTTP header,  .

#### <a href="#" id="custom-http-headers"> Custom HTTP headers</a>

##### `x-datafilters-fields-include`
`x-datafilters-fields-include` custom HTTP header allows to specified which properties that will be kept in the body response.

##### `x-datafilters-fields-exclude` 
`x-datafilters-fields-exclude` custom HTTP header allows to specify which properties that will be 
dropped from the body response.

These custom headers can be handy for mobile clients that query a REST API by reducing the volume 
of data transfered from backend. This can also allow to design one API that can serve multiple clients :
each client could "select" the properties it want to display.

#### <a href="#" id="prefer-http-header-support">[`Prefer`](https://httpwg.org/specs/rfc7240.html) HTTP header support</a>

This library offers a limited support of the [Prefer](https://httpwg.org/specs/rfc7240.html) HTTP header. 
Specifically, a client can request a "minimal" representation of the resource by setting the `Prefer: return=minimal` HTTP header.

Given the following request
```http
GET /api/users HTTP/1.1

Prefer: return=minimal
```

and the following C# class where the [`MinimalAttribute`](/src/DataFilters.AspNetCore/Attributes/MinimalAttribute.cs)
is applied to both `Name` and `Id` properties :


```csharp
public class Person
{
    [Minimal]
    public Guid Id { get; set; }
    [Minimal]
    public string Name { get; set; }
    public string Email { get; set; }
}
```

the server can respond with a "minimal" representation of the resource.

```http
HTTP/1.1 200 OK

<headers omitted for brevity>

[
   {
       "id": "83c39be2-5123-47bf-a1d1-9df15d146e6a",
       "name": "John Doe"
   },
   {
       "id": 2,
       "name": "Jane Doe"
   }
]
```

Only properties marked with the [`MinimalAttribute`](/src/DataFilters.AspNetCore/Attributes/MinimalAttribute.cs) are returned.

💡 <strong> Usage of the `MinimalAttribute` automatically triggers support of the `Prefer` HTTP header.</strong>


### Improve performances
The library comes with a [`IDataFilterService`](/src/DataFilters.AspNetCore/IDataFilterService.cs)  that can be used to build caches `IFilter` instances created by the service.


## <a href='#' id='how-to-install'>How to install</a>

1. run `dotnet install DataFilters.AspNetCore` to add the package to your solution
2. add the following line to your Startup.cs file

```csharp
public void ConfigureServices(IServiceCollection services)
{
   services.AddDataFilterService(options => 
   {
         // configure DataFiltersOptions
   });
}
```
This will add [`IDataFilterService`](/src/DataFilters.AspNetCore/IDataFilterService.cs) as a singleton to the dependency injection container.

3. You can also opt in to use the custom HTTP headers by adding and instance of `SelectPropertiesActionFilterAttribute`
```csharp
services.Filters.Add(new SelectPropertyFilterAttribute());
```


This will then enable usage of `x-datafilters-fields-include` and `x-datafilters-fields-exclude` HTTP headers


[datafilters-nupkg]: https://nuget.org/packages/DataFilters