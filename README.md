#  DataFilters.AspNetCore <!-- omit in toc -->

A small library that ease usage of [DataFilters][datafilters-nupkg] with ASP.NET Core APIs.

# Why
[DataFilters][datafilters-nupkg] allows to build complex queries in a "restfull" way.
However, it comes with some drawbacks.

In order to build a filter, you have to :

1. parse the incoming string
2. map it manually to the underlying model.
2. converts it into an IFilter instance using the `ToFilter<T>` extension method.

This can be a tedious task and I created this library to ease that process.


# <a href='#' id='how-to-install'>How to install</a>

1. run `dotnet install DataFilters.AspNetCore` to add the package to your solution
2. add the following line to your Startup.cs file

```csharp
public void ConfigureServices(IServiceCollection services)
{
   services.AddDataFilterService( options => 
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