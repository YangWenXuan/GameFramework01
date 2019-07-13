using System;
using XPlugin.Security;

namespace GameClient {
    public class StaticData
    {
        public static string LastSavedTime {
            get => PlayerPrefsAES.GetString("LastSavedTime", DateTime.MinValue.Date.ToString("yyyy-M-d"));
            set => PlayerPrefsAES.SetString("LastSavedTime", value);
        }
    }
}
