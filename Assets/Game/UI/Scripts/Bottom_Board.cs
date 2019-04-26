using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XUI;

namespace UI
{
    public class Bottom_Board : UIBehaviour
    {
        private GameObject BackBtn;

        private void Awake()
        {
            BackBtn = transform.Find("BackBtn").gameObject;

            UIEventTriggerListener.Get(BackBtn).onClick = OnBackBtnClick;
        }

        private void OnBackBtnClick(GameObject go)
        {
            ModMenu.Ins.Back(true);//返回上一级.
        }
    }
}
