using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

namespace GameClient
{
    public class Client : MonoBehaviour
    {
        public static Client Ins { get; private set; }

        private void Start()
        {
            Init();
        }

        static UIModule uiModule;
        public static UIModule UIModule { get { return uiModule; } }

        void Init()
        {
            uiModule = new UIModule();


            UIModule.Init();
            //LString.Load_UIString();//加载本地化文件.
        }
    }
}
