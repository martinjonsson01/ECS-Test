using System;
using System.Collections.Generic;

using Unity.Entities;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

namespace Game.Extensions
{
public static class DynamicBufferExtensions
{
    internal static bool Any<T>(this DynamicBuffer<T> elements, Func<T, bool> predicate)
        where T : struct, IBufferElementData
    {
        foreach (T element in elements)
        {
            if (predicate(element)) return true;
        }
        return false;
    }

    internal static void RemoveElementsButExcludeSome<T>(this DynamicBuffer<T> elements, Func<T, bool> exclude)
        where T : struct, IBufferElementData
    {
        IEnumerable<T> cooldownsExcludingSome = elements.ExcludeElements(exclude);
        RemoveElements(elements, cooldownsExcludingSome);
    }

    internal static void RemoveElements<T>(this DynamicBuffer<T> elements, IEnumerable<T> toRemoves)
        where T : struct, IBufferElementData
    {
        foreach (T toRemove in toRemoves)
        {
            elements.TryRemoveElement(toRemove);
        }
    }

    internal static bool TryRemoveElement<T>(this DynamicBuffer<T> elements, T toRemove)
        where T : struct, IBufferElementData
    {
        int removeIndex = elements.FindIndexOf(toRemove);
        if (removeIndex == -1) return false;
        elements.RemoveAtSwapBack(removeIndex);
        return true;
    }

    internal static IEnumerable<T> ExcludeElements<T>(this DynamicBuffer<T> elements, Func<T, bool> exclude)
        where T : struct, IBufferElementData
    {
        var ofType = new List<T>();
        foreach (T element in elements)
        {
            if (exclude(element)) continue;
            ofType.Add(element);
        }

        return ofType;
    }

    internal static int FindIndexOf<T>(this DynamicBuffer<T> elements, T toFind) where T : struct, IBufferElementData
    {
        int removeIndex = -1;
        for (var i = 0; i < elements.Length; i++)
        {
            if (!elements.ElementAt(i).Equals(toFind)) continue;
            removeIndex = i;
            break;
        }

        return removeIndex;
    }
}
}
