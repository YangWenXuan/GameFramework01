using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class Game : MonoBehaviour
{
    private bool inited = false;
    public static Game Instance { get; private set; }
    public bool Inited { get { return inited; } }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        //资源第一级优先初始化，其他初始化放在后面
        yield return null;
    }
}
