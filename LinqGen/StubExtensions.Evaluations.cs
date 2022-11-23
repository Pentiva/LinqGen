﻿// LinqGen, Maxwell Keonwoo Kang <code.athei@gmail.com>, 2022

using System;
using System.Collections.Generic;
using Cathei.LinqGen.Hidden;

namespace Cathei.LinqGen
{
    /// <summary>
    /// This will return empty stub, and will be replaced by generated method.
    /// Thus it should never be called on runtime.
    /// Unused parameter ensures that the this version never called with overloading resolution.
    /// </summary>
    public static partial class StubExtensions
    {
        public static IEnumerator<T> GetEnumerator<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }

        public static T First<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }

        public static T FirstOrDefault<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }

        public static T Last<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }

        public static T LastOrDefault<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }

        public static T Sum<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }

        public static TOut Sum<T, TUp, TOut>(this IStub<IContent<T>, TUp> enumerable, Func<T, TOut> func)
        {
            throw new NotImplementedException();
        }

        public static TOut Sum<T, TUp, TOut>(this IStub<IContent<T>, TUp> enumerable, IStructFunction<T, TOut> func)
        {
            throw new NotImplementedException();
        }

        public static int Count<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }

        public static int Count<T, TUp>(this IStub<IContent<T>, TUp> enumerable, Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public static int Count<T, TUp>(this IStub<IContent<T>, TUp> enumerable, IStructFunction<T, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public static int Min<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }

        public static int Min<T, TUp, TComparer>(this IStub<IContent<T>, TUp> enumerable, TComparer comparer)
            where TComparer : IComparer<T>
        {
            throw new NotImplementedException();
        }

        // public static int MinBy<T, TUp, TKey>(this IStub<IContent<T>, TUp> enumerable,
        //     Func<T, TKey> selector, IComparer<TKey>? comparer = null)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public static int MinBy<T, TUp, TKey, TComparer>(this IStub<IContent<T>, TUp> enumerable,
        //     IStructFunction<T, TKey> keySelector, TComparer comparer)
        //     where TComparer : IComparer<TKey>
        // {
        //     throw new NotImplementedException();
        // }

        public static int Max<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }

        public static int Max<T, TUp, TComparer>(this IStub<IContent<T>, TUp> enumerable, TComparer comparer)
            where TComparer : IComparer<T>
        {
            throw new NotImplementedException();
        }

        // public static int MaxBy<T, TUp, TKey>(this IStub<IContent<T>, TUp> enumerable,
        //     Func<T, TKey> selector, IComparer<TKey>? comparer = null)
        // {
        //     throw new NotImplementedException();
        // }
        //
        // public static int MaxBy<T, TUp, TKey, TComparer>(this IStub<IContent<T>, TUp> enumerable,
        //     IStructFunction<T, TKey> keySelector, TComparer comparer)
        //     where TComparer : IComparer<TKey>
        // {
        //     throw new NotImplementedException();
        // }

        public static List<T> ToList<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }

        public static T[] ToArray<T, TUp>(this IStub<IContent<T>, TUp> enumerable)
        {
            throw new NotImplementedException();
        }
    }
}