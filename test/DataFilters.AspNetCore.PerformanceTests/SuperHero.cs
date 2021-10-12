namespace DataFilters.AspNetCore.PerfomanceTests
{
    using System.Collections.Generic;

    internal class SuperHero
    {
        public string Nickname { get; init; }

        public string[] Powers { get; init; }

        public IEnumerable<SuperHero> Acolytes { get; init; }

    }
}
