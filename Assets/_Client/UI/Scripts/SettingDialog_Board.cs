using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XUI;
using XPlugin.UI;
using UI;

public class SettingDialog_Board : UIBehaviour
{
    public static void show()
    {
        ModMenu.Ins.Overlay(new[] { "UIPrefab/AlphaBG_Board", "UIPrefab/SettingDialog_Board" });
    }
}
