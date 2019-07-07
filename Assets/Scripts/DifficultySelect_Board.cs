using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XUI;

namespace UI
{
    public class DifficultySelect_Board : UIBehaviour
    {
        private static UIGroup Group;

        public static void Show()
        {
            Group = ModMenu.Ins.Cover(new[] { "UIPrefab/Bottom_Board", "UIPrefab/DifficultySelect_Board" });
        }
    }
}
