using UnityEngine;
using System.Collections;

public static class GameObjectExtensions
{

    /// <summary>
    /// Log an unfound component
    /// </summary>
    /// <typeparam name="T">Type of component</typeparam>
    /// <param name="obj">Parent object</param>
    /// <returns>Component Found</returns>
    public static T GetSafeComponent<T>(this GameObject obj) where T : MonoBehaviour
    {
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            Debug.LogError("Expected to find component of type " + typeof(T) + " but found none", obj);
        }

        return component;
    }

}
