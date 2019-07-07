using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XUI;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu_Board : UIBehaviour
    {
        public static UIGroup Group;

        private Button LocalButton;

        public static void Show()
        {
            Group = ModMenu.Ins.Cover(new[] { "UIPrefab/MainMenu_Board" });
        }

        private void Awake()
        {
            LocalButton = transform.Find("LocalButton").GetComponent<Button>();

            UIEventTriggerListener.Get(LocalButton.gameObject).onClick = OnLocalButtonClick;
        }

        private void OnLocalButtonClick(GameObject go)
        {
            DifficultySelect_Board.Show();
        }

        public override void OnUISpawned()
        {
            //UI面板被实例化出来的时候调用，先于OnUIShow()
            Debug.Log("OnUISpawned........");

        }

        public override void OnUIShow(params object[] args)
        {
            //UI面板被显示出来的时候执行这个方法.
            Debug.Log("OnUIShow..........");
        }

        public override void OnUIClose()
        {
            Debug.Log("OnUIClose..........");
        }

        public override void OnUIDespawn()
        {
            Debug.Log("OnUIDespawn..........");
        }

        public override void OnUIPause(bool cover)
        {
            //当 当前面板切换到别的面板的时候，这个方法执行.
            Debug.Log("OnUIPause..........");
        }

        public override void OnUIResume(bool fromCover)
        {
            //当从别的面板回到当前面板的时候，这个方法执行.
            Debug.Log("OnUIResume..........");
            Debug.Log("FromCover:" + fromCover);
        }
    }
}
