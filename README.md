#  DataFilters.AspNetCore <!-- omit in toc -->


A small library that allow to convert a string to a generic `IFilter`object.
Highly inspired by the elastic query syntax, it offers a powerful way to build and query data with a syntax that's not bound to a peculiar datasource.

**Table of contents**

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
This will add [`IDataFilterService`](/src/DataFilters.AspNetCore/IDataFilterSercice.cs) as a singleton to the dependency injection container


3. You can also opt in to use a custom HTTP header `X-DataFilters-Selection`

# <a href='#' id='how-to-use'>How to use</a>



## <a href='#' id='how-to-use-client'>On the client</a>

The client will have the responsability of building search criteria.
Go to [filtering](#filtering) and [sorting](#sorting) sections to see example on how to get started.

## <a href='#' id='how-to-use-backend'>On the backend</a>

One way to start could be by having a dedicated resource which properties match the resource's properties search will
be performed onto.

Continuing with our `vigilante` API, we could have

```csharp
// Wraps the search criteria for Vigilante resources.
public class SearchVigilanteQuery
{
    public string Firstname {get; set;}

    public string Lastname {get; set;}

    public string Nickname {get; set;}

    public int? Age {get; set;}
}
```

and the following endpoint

```csharp
using DataFilters.AspNetCore;

public class VigilantesController
{
    private readonly IDataFilterService _filterService;

    public class VigilantesController(IDataFilterService filterService)
    {
        _filterService = filterService;
    }

    // code omitted for brievity

    [HttpGet("search")]
    [HttpHead("search")]
    public ActionResult Search([FromQuery] SearchVigilanteQuery query)
    {
        IList<IFilter> filters = new List<IFilter>();

        if(!string.IsNullOrWhitespace(query.Firstname))
        {
            filters.Add($"{nameof(Vigilante.Firstname)}={query.Firstname}".ToFilter<Vigilante>());
        }

        if(!string.IsNullOrWhitespace(query.Lastname))
        {
            filters.Add($"{nameof(Vigilante.Lastname)}={query.Lastname}".ToFilter<Vigilante>());
        }

        if(!string.IsNullOrWhitespace(query.Nickname))
        {
            filters.Add($"{nameof(Vigilante.Nickname)}={query.Nickname}".ToFilter<Vigilante>());
        }

        if(query.Age.HasValue)
        {
            filters.Add($"{nameof(Vigilante.Age)}={query.Age.Value}".ToFilter<Vigilante>());
        }


        IFilter  filter = filters.Count() == 1
            ? filters.Single()
            : new MultiFilter{ Logic = And, Filters = filters };

        // filter now contains how search criteria and is ready to be used 😊

    }
}

```

Some explanation on the controller's code above  :

1. The endpoint is bound to incoming HTTP `GET` and `HEAD` requests on `/vigilante/search`
2. The framework will parse incoming querystring and feeds the `query` parameter accordingly.
3. From this point we test each criterion to see if it's acceptable to turn it into a [IFilter][class-ifilter] instance.
   For that purpose, the handy `.ToFilter<T>()` string extension method is available. It turns a query-string key-value pair into a
   full [IFilter][class-ifilter].
4. we can then either :
   - use the filter directly is there was only one filter
   - or combine them using [composite filter][class-multi-filter] if there were more than one criterion.

You may have noticed that `SearchVigilanteQuery.Age` property is nullable whereas `Vigilante.Age` property is not.
This is to distinguish if the `Age` criterion was provided or not when calling the `vigilantes/search` endpoint.

|                           | Package                                                                                                                                         | Description                                                                                                                                                                         |
| ------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `DataFilters`             | [![Nuget](https://img.shields.io/nuget/v/Datafilters?style=for-the-badge)](https://www.nuget.org/packages/DataFilters)                          | provides core functionalities of parsing strings and converting to [IFilter][class-ifilter] instances.                                                                              |
| `DataFilters.Expressions` | [![Nuget](https://img.shields.io/nuget/v/DataFilters.Expressions?&style=for-the-badge)](https://www.nuget.org/packages/DataFilters.Expressions) | adds `ToExpression<T>()` extension method on top of [IFilter][class-ifilter] instance to convert it to an equivalent `System.Linq.Expressions.Expression<Func<T, bool>>` instance.  |
| `DataFilters Queries`     | [![Nuget](https://img.shields.io/nuget/v/Datafilters.Queries?style=for-the-badge)](https://www.nuget.org/packages/DataFilters.Queries)          | adds `ToWhere<T>()` extension method on top of [IFilter][class-ifilter] instance to convert it to an equivalent [`IWhereClause`](https://dev.azure.com/candoumbe/Queries) instance. |


[class-multi-filter]: /src/DataFilters/MultiFilter.cs
[class-ifilter]: /src/DataFilters/IFilter.cs
[class-filter]: /src/DataFilters/Filter.cs
[datafilters-expressions]: https://www.nuget.org/packages/DataFilters.Expressions
[datafilters-queries]: https://www.nuget.org/packages/DataFilters.Queries