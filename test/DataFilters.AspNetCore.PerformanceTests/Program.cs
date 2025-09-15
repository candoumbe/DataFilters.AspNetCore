using BenchmarkDotNet.Running;
using DataFilters.AspNetCore.PerfomanceTests;

BenchmarkSwitcher.FromAssembly(typeof(RawFilterVsDataFilters).Assembly).Run(args);