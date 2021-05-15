#  DataFilters.AspNetCore <!-- omit in toc -->


A small library that ease usage of [DataFilters](https://nuget.org/packages/DataFilters) nuget package.


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
services.Filters
```


4.  `x-datafilters-fields-include`

