// LinqGen.Benchmarks, Maxwell Keonwoo Kang <code.athei@gmail.com>, 2022

using BenchmarkDotNet.Attributes;
using Cathei.LinqGen;
using StructLinq;
using NetFabric.Hyperlinq;
using StructLinq.Range;

namespace QuickLinq.Benchmarks.Cases;

[MemoryDiagnoser]
public class ListSelectSum
{
    private const int Count = 10_000;

    private static List<int> TestData;

    static ListSelectSum()
    {
        TestData = new List<int>();

        for (int i = 0; i < Count; ++i)
            TestData.Add(i);
    }

    [Benchmark]
    public double ForEachLoop()
    {
        double sum = 0;

        foreach (var i in TestData)
            sum += i * 2.0;

        return sum;
    }

    [Benchmark(Baseline = true)]
    public double Linq()
    {
        return TestData
            .Select(x=> x * 2.0)
            .Sum();
    }

    [Benchmark]
    public double LinqGenDelegate()
    {
        return TestData
            .Specialize()
            .Select(x => x * 2.0)
            .Sum();
    }

    [Benchmark]
    public double LinqGenStruct()
    {
        var selector = new Selector();

        return TestData
            .Specialize()
            .Select(selector)
            .Sum();
    }

    [Benchmark]
    public double StructLinqDelegate()
    {
        return TestData
            .ToStructEnumerable()
            .Select(x => x * 2.0)
            .Sum();
    }

    [Benchmark]
    public double StructLinqStruct()
    {
        var selector = new Selector();

        return TestData
            .ToStructEnumerable()
            .Select(ref selector, x => x, x => x)
            .Sum(x=> x);
    }

    [Benchmark]
    public double HyperLinqDelegate()
    {
        return TestData
            .AsValueEnumerable()
            .Select(x => x * 2.0)
            .Sum();
    }

    [Benchmark]
    public double HyperLinqStruct()
    {
        return TestData
            .AsValueEnumerable()
            .Select<double, Selector>()
            .Sum();
    }

    readonly struct Selector :
        StructLinq.IFunction<int, double>,
        NetFabric.Hyperlinq.IFunction<int, double>,
        IStructFunction<int, double>
    {
        public double Eval(int element)
        {
            return element * 2.0;
        }

        public double Invoke(int arg)
        {
            return arg * 2.0;
        }
    }
}
