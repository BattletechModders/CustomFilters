using System;
using UnityEngine;

namespace CustomFilters.Shared;

internal static class ComponentExtensions
{
    internal static bool IsGameObjectNull(this Component component)
    {
        try
        {
            return component.gameObject == null;
        }
        catch (NullReferenceException)
        {
            return true;
        }
    }
}