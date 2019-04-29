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
        private GameObject SettingsBtn;

        private void Awake()
        {
            BackBtn = transform.Find("BackBtn").gameObject;
            SettingsBtn = transform.Find("SettingsBtn").gameObject;

            UIEventTriggerListener.Get(BackBtn).onClick = OnBackBtnClick;
            UIEventTriggerListener.Get(SettingsBtn).onClick = OnSettingsBtnClick;
        }

        private void OnSettingsBtnClick(GameObject go)
        {
            SettingDialog_Board.show();
        }

        private void OnBackBtnClick(GameObject go)
        {
            ModMenu.Ins.Back(true);//返回上一级.
        }
    }
}
