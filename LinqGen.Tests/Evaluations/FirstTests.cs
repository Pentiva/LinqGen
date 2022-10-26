// LinqGen.Tests, Maxwell Keonwoo Kang <code.athei@gmail.com>, 2022

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Cathei.LinqGen;

namespace Cathei.LinqGen.Tests;

[TestFixture]
public class FirstTests
{
    [TestCase(0, 10)]
    [TestCase(-5, 10)]
    public void TestResult_SameAsLinq(int start, int count)
    {
        var expected = Enumerable.Range(start, count)
            .First();

        var actual = GenEnumerable.Range(start, count)
            .First();

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void First_ThrowsException_IfEmpty()
    {
        IEnumerable<int> x = Array.Empty<int>();

        x.Specialize();
        //.First();

        Array.Empty<int>().Specialize();

    }

    //
    // [TestCase(0, 0)]
    // [TestCase(0, 10)]
    // [TestCase(-5, 10)]
    // public void TestResultDelegate_SameAsLinq(int start, int count)
    // {
    //     var expected = Enumerable.Range(start, count)
    //         .First(x => x * 10);
    //
    //     var actual = GenEnumerable.Range(start, count)
    //         .First(x => x * 10);
    //
    //     Assert.AreEqual(expected, actual);
    // }
    //
    // [TestCase(0, 0)]
    // [TestCase(0, 10)]
    // [TestCase(-5, 10)]
    // public void TestResultStruct_SameAsLinq(int start, int count)
    // {
    //     var expected = Enumerable.Range(start, count)
    //         .First(x => x * 0.1m);
    //
    //     var actual = GenEnumerable.Range(start, count)
    //         .First(new Selector());
    //
    //     Assert.AreEqual(expected, actual);
    // }
    //
    // private struct Selector : IStructFunction<int, decimal>
    // {
    //     public decimal Invoke(int arg)
    //     {
    //         return arg * 0.1m;
    //     }
    // }
}
