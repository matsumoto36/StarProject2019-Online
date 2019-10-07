using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FindInterface
{
    /// <summary>
    /// 単数
    /// </summary>
    /// <typeparam name="T">型</typeparam>
    /// <returns></returns>
    public static T FindObjectOfInterfaces<T>() where T : class
    {
        foreach (var n in GameObject.FindObjectsOfType<Component>())
        {
            var component = n as T;

            if (component != null)
            {
                return component;
            }
        }
        return null;
    }

    /// <summary>
    /// 複数
    /// </summary>
    /// <typeparam name="T">型</typeparam>
    /// <returns></returns>
    public static List<T> FindObjectsOfInterfaces<T>() where T : class
    {
        List<T> list = new List<T>();

        foreach (var n in GameObject.FindObjectsOfType<Component>())
        {
            var component = n as T;

            if (component != null)
            {
                list.Add(component);
            }
        }
        return list;
    }

    public static T FindGameObjectInterface<T>(this GameObject target) where T : class
    {

        var components = target.GetComponents<Component>();

        foreach (var n in components)
        {

            var component = n as T;

            if (component != null) return component;

        }
        return null;
    }

}
