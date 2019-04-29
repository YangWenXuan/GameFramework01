using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UI;
using GameClient;

public sealed partial class Game : MonoBehaviour
{
    //private bool inited = false;
    //public static Game Instance { get; private set; }
    //public bool Inited { get { return inited; } }

    private void Awake()
    {
        //Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        //资源第一级优先初始化，其他初始化放在后面
        //modMenu = new ModMenu();
        //client = new Client();

        //ModMenu.Init();
        //Client.Init();
        yield return null;
    }

    IEnumerator LateInit()
    {
        yield return null;
    }

    #region GameModules
    //static ModMenu modMenu;
    //public static ModMenu ModMenu { get { return modMenu; } }
    //public static Client Client { get { return Client.Ins; } }
    #endregion

}
