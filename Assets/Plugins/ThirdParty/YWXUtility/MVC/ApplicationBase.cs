using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using YWX.MVC;

namespace YWX.MVC
{
    public abstract class ApplicationBase<T> : SingletonMVC<T>
    where T : MonoBehaviour
{
    //注册控制器
    protected void RegisterController(string eventName,Type controllerType)
    {
        MVC.RegisterController(eventName, controllerType);
    }

    protected void SendEvent(string eventName, object data = null)
    {
        MVC.SendEvent(eventName, data);
    }
}
}