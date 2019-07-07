using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XUI;
using GameClient;
using XPlugin.Localization;

using System;//--Enum
using XPlugin.Security;//--PlayerPrefsAES.GetString()

namespace UI
{
    public class ModMenu:UIMod<ModMenu>
    {
        private void Start()
        {
            LanguageEnum lang = LanguageEnum.unkonwn;
            Enum.TryParse(PlayerPrefsAES.GetString("lang"), true, out lang);//?????
            if (lang == LanguageEnum.unkonwn)
            {
                lang = Localization.SystemLang;
            }
            if (lang == LanguageEnum.unkonwn)
            {//默认英文
                lang = LanguageEnum.en;
            }
            Localization.Init(lang);
            LString.Load_UIString();
        }
    }
}
