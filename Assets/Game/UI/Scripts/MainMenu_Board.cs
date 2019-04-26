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

    }
}
