using System;
using BenchmarkDotNet.Attributes;

namespace DataFilters.AspNetCore.PerfomanceTests;

[MemoryDiagnoser]
public class RawFilterVsDataFilters
{
    private IDataFilterService _service;

    [Params("Nickname=Bat*")]
    public string Input { get; set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _service = new DefaultDataFilterService(new DataFilterOptions { MaxCacheSize = 10 });
    }

    [Benchmark]
    public IFilter WithoutCache() => Input.ToFilter<SuperHero>();

    [Benchmark]
    public IFilter WithCache() => _service.Compute<SuperHero>(Input);   
}