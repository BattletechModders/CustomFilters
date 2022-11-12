using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using UnityEngine;

namespace CustomFilters.MechBaySorting;

internal static class MechBayDynamicSorting
{
    internal static void Sort<T>(List<T> list) where T : IMechLabDraggableItem
    {
        if (list.Count < 2)
        {
            return;
        }

        var supportsMechDef = list[0].MechDef != null;
        list.Sort((aItem, bItem) =>
        {
            foreach (var sortOrder in SortOrder)
            {
                if (!supportsMechDef && sortOrder.ValueExtractor.UsesMechDef)
                {
                    continue;
                }

                var cmp = CompareObjects(
                    sortOrder.ValueExtractor.Func(aItem),
                    sortOrder.ValueExtractor.Func(bItem)
                );

                if (cmp != 0)
                {
                    return sortOrder.Descending ? -cmp : cmp;
                }
            }
            return 0;
        });
    }
    private static int CompareObjects(object? a, object? b)
    {
        if (a == b)
        {
            return 0;
        }
        if (a == null)
        {
            return -1;
        }
        if (b == null)
        {
            return 1;
        }
        if (a is string aStr)
        {
            return string.CompareOrdinal(aStr, b as string);
        }
        if (a is float aFloat && b is float bFloat)
        {
            if (Mathf.Approximately(aFloat, bFloat))
            {
                return 0;
            }
            return aFloat.CompareTo(bFloat);
        }
        if (a is IComparable aComparable)
        {
            return aComparable.CompareTo(b);
        }
        return 0;
    }

    private static readonly List<Sorter> SortOrder = new();
    internal static void SetSortOrder(params string[] terms)
    {
        SortOrder.Clear();
        foreach (var candidate in terms)
        {
            string term;
            bool descending;
            if (candidate.StartsWith("!"))
            {
                term = candidate.Substring(1);
                descending = true;
            }
            else
            {
                term = candidate;
                descending = false;
            }

            if (ValueExtractors.TryGetValue(term, out var extractor))
            {
                SortOrder.Add(new(descending, extractor));
            }
            else
            {
                Logging.Warning?.Log($"Can't find sorter for term {term}, available terms: " + string.Join(", ", ValueExtractors.Keys));
            }
        }
    }
    private class Sorter
    {
        internal readonly bool Descending;
        internal readonly ValueExtractor ValueExtractor;

        public Sorter(bool descending, ValueExtractor valueExtractor)
        {
            Descending = descending;
            ValueExtractor = valueExtractor;
        }
    }
    private class ValueExtractor
    {
        internal readonly bool UsesMechDef;
        internal readonly Func<IMechLabDraggableItem, object?> Func;

        internal ValueExtractor(bool usesMechDef, Func<IMechLabDraggableItem, object?> func)
        {
            UsesMechDef = usesMechDef;
            Func = func;
        }
    }
    private static readonly Dictionary<string, ValueExtractor> ValueExtractors = new()
    {
        { "MechName", new(true, mech => mech.MechDef.Name) },
        { "MechCBillValue", new(true, mech => CalculateCBillValue(mech.MechDef)) },
        { "ChassisName", new(false, mech => mech.ChassisDef.Description.Name) },
        { "ChassisTonnage", new(false, mech => mech.ChassisDef.Tonnage) },
        { "ChassisVariantName", new(false, mech => mech.ChassisDef.VariantName) },
        { "ChassisCost", new(false, mech => mech.ChassisDef.Description.Cost) },
    };
    private static float CalculateCBillValue(MechDef mechDef)
    {
        var current = 0f;
        var max = 0f;
        MechStatisticsRules.CalculateCBillValue(mechDef, ref current, ref max);
        return current;
    }
}
