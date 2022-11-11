using UnityEngine;

namespace CustomFilters;

internal static class SharedGameObjects
{
    internal static Transform ContainerTransform { get; }
    static SharedGameObjects()
    {
        if (ContainerTransform == null)
        {
            var containerGo = new GameObject(nameof(CustomFilters));
            containerGo.SetActive(false);
            Object.DontDestroyOnLoad(containerGo);
            ContainerTransform = containerGo.transform;
        }
    }
}