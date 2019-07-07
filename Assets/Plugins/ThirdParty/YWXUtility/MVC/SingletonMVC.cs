using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace YWX.MVC
{
    public abstract class SingletonMVC<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T m_instance = null;

    public static T Instance
    {
        get { return m_instance; }
    }

    protected virtual void Awake()
    {
        m_instance = this as T;
    }
}
}