using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class kgBase : MonoBehaviour 
{
    public delegate void Task();

    #region Extensions
    public void Invoke(Task task, float time)
    {
        Invoke(task.Method.Name, time);
    }

    public I GetInterfaceComponent<I>() where I : class
    {
        return GetComponent(typeof(I)) as I;
    }

    public static List<I> FindObjectsOfInterface<I>() where I : class
    {
        MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
        List<I> list = new List<I>();

        foreach (MonoBehaviour behaviour in monoBehaviours)
        {
            I component = behaviour.GetComponent(typeof(I)) as I;

            if (component != null)
            {
                list.Add(component);
            }
        }

        return list;
    }

    #endregion

    #region Boilerplate

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion

}
