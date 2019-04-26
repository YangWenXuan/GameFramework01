using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XUI;
using GameClient;

namespace UI
{
    public class ModMenu:UIMod<ModMenu>
    {
        private void Start()
        {
            Client.Create();
            MainMenu_Board.Show();
        }
    }
}
