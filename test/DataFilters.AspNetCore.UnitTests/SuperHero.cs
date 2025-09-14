using System.Collections.Generic;

namespace DataFilters.AspNetCore.UnitTests;

internal class SuperHero
{
    public string Nickname { get; set; }

    public string[] Powers { get; set; }

    public IEnumerable<SuperHero> Acolytes { get; set; }

}