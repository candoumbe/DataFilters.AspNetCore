using BenchmarkDotNet.Running;

namespace DataFilters.AspNetCore.PerfomanceTests;

public class Program
{
    static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
        .Run(args);
}