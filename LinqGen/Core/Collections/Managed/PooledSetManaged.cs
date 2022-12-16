﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cathei.LinqGen.Hidden
{
    /// <summary>
    /// Do not use this struct manually, reserved for generated code
    /// No need to provide Remove operation
    /// </summary>
    public struct PooledSetManaged<T, TComparer> : IDisposable
        where TComparer : IEqualityComparer<T>
    {
        private readonly TComparer _comparer;

        private DynamicArrayNative<int> _buckets;
        private DynamicArrayManaged<PooledSetSlot<T>> _slots;

        private int _size;
        private int _count;

        public PooledSetManaged(int capacity, TComparer comparer) : this()
        {
            _comparer = comparer;

            _size = HashHelpers.GetPrime(capacity);
            _count = 0;

            _buckets = new DynamicArrayNative<int>(_size);
            _slots = new DynamicArrayManaged<PooledSetSlot<T>>(_size);

            _buckets.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetHashCode(T item)
        {
            return item == null ? 0 : _comparer.GetHashCode(item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Reduce(int hashCode, int size)
        {
            return (uint)hashCode % (uint)size;
        }

        private void IncreaseCapacity()
        {
            int newSize = HashHelpers.ExpandPrime(_count);
            if (newSize <= _count)
            {
                throw new InvalidOperationException("Capacity overflow");
            }

            // Able to increase capacity; copy elements to larger array and rehash
            SetCapacity(newSize);
        }

        private void SetCapacity(int newSize)
        {
            DynamicArrayNative<int> newBuckets;
            var localSlots = _slots;
            bool replaceBucket;

            // Because ArrayPool might have given us larger arrays than we asked for, see if we can
            // use the existing capacity without actually resizing.
            if (_buckets.Length >= newSize && _slots.Length >= newSize)
            {
                _buckets.Clear();
                newBuckets = _buckets;
                replaceBucket = false;
            }
            else
            {
                newBuckets = new DynamicArrayNative<int>(newSize);
                newBuckets.Clear();
                replaceBucket = true;

                localSlots.IncreaseCapacity(newSize, _count);
            }

            for (int i = 0; i < _count; i++)
            {
                ref var slot = ref localSlots[i];
                uint bucket = Reduce(slot.HashCode, newSize);
                slot.Next = newBuckets[bucket] - 1;
                newBuckets[bucket] = i + 1;
            }

            if (replaceBucket)
            {
                _buckets.Dispose();
                _buckets = newBuckets;
            }

            _size = newSize;
        }

        public bool Add(T value)
        {
            int hashCode = GetHashCode(value);
            uint bucket = Reduce(hashCode, _size);
            int collisionCount = 0;
            var localSlots = _slots;

            for (int i = _buckets[bucket] - 1; i >= 0; )
            {
                ref var slot = ref localSlots[i];
                if (slot.HashCode == hashCode && _comparer.Equals(slot.Value, value))
                    return false;

                if (collisionCount >= _size)
                {
                    // The chain of entries forms a loop, which means a concurrent update has happened.
                    throw new InvalidOperationException("Concurrent operations are not supported.");
                }
                collisionCount++;
                i = slot.Next;
            }

            if (_count == _size)
            {
                IncreaseCapacity();
                // this will change during resize
                localSlots = _slots;
                bucket = Reduce(hashCode, _size);
            }

            int index = _count;

            ref var lastSlot = ref localSlots[index];
            lastSlot.HashCode = hashCode;
            lastSlot.Value = value;
            lastSlot.Next = _buckets[bucket] - 1;

            _buckets[bucket] = index + 1;
            _count++;
            return true;
        }

        public bool Contains(T value)
        {
            int hashCode = GetHashCode(value);
            uint bucket = Reduce(hashCode, _size);

            var localSlots = _slots;

            for (int i = _buckets[bucket] - 1; i >= 0;)
            {
                ref var slot = ref localSlots[i];
                if (slot.HashCode == hashCode && _comparer.Equals(slot.Value, value))
                    return true;

                i = slot.Next;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            _slots.Dispose();
            _buckets.Dispose();

            _size = 0;
            _count = 0;
        }
    }
}
