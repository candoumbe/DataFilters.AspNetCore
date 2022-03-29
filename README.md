[![GitHub Workflow Status (main)](https://img.shields.io/github/workflow/status/candoumbe/datafilters.aspnetcore/deployment/main?label=main)](https://github.com/candoumbe/DataFilters.AspNetCore/actions/workflows/deployment.yml)
[![GitHub Workflow Status (develop)](https://img.shields.io/github/workflow/status/candoumbe/datafilters.aspnetcore/integration/develop?label=develop)](https://github.com/candoumbe/DataFilters.AspNetCore/actions/workflows/continous.yml)
[![codecov](https://codecov.io/gh/candoumbe/DataFilters.aspnetcore/branch/develop/graph/badge.svg?token=FHSC41A4X3)](https://codecov.io/gh/candoumbe/DataFilters.aspnetcore)
[![GitHub raw issues](https://img.shields.io/github/issues-raw/candoumbe/datafilters.apsnetcore)](https://github.com/candoumbe/datafilters.aspnetcore/issues)
[![Nuget](https://img.shields.io/nuget/vpre/datafilters.aspnetcore)](https://nuget.org/packages/datafilters)
#  DataFilters.AspNetCore <!-- omit in toc -->

A small library that ease usage of [DataFilters][datafilters-nupkg] with ASP.NET Core APIs.





## <a href="#" id="lnk-why">Why</a>

### Make it easier to build `IFilter` instances
[DataFilters][datafilters-nupkg] allows to build complex queries in a "restfull" way so 
However, it comes with some drawbacks.

In order to build a filter, you have to :

1. parse the incoming string
2. map it manually to an underlying model type.
3. converts it into an IFilter instance using the `ToFilter<T>` extension method.

This can be a tedious task and this library can help to ease that process.

### Limit the bandwith usage
The library adds support for two custom HTTP headers : `x-datafilters-fields-include` and `x-datafilters-fields-exclude`.

#### `x-datafilters-fields-include`
`x-datafilters-fields-include` custom HTTP header allows to specified which properties that will be kept in the body response.

#### `x-datafilters-fields-exclude` 
`x-datafilters-fields-exclude` custom HTTP header allows to specify which properties that will be 
dropped from the body response.

These custom headers can be handy for mobile clients that query a REST API by reducing the volume 
of data transfered from backend. This can also allow to design one API that can serve multiple clients :
each client could "select" the properties it want to display.

## Improve performances
The library caches `IFilter` instances created by the service.

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