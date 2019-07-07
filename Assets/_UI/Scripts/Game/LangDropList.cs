using GameClient;
using UI;
using UnityEngine;
using UnityEngine.UI;
using XPlugin.Localization;
using XPlugin.Security;

namespace UI {
    public class LangDropList : MonoBehaviour {
        public Toggle zh;
        public Toggle zht;
        public Toggle en;
        public ToggleGroup group;
        private bool restart = true;

        private void Awake() {

            this.restart = false;
            switch (Localization.Language) {
                case LanguageEnum.en: {
                    this.en.isOn = true;
                    break;
                }
                case LanguageEnum.zh: {
                    this.zh.isOn = true;
                    break;
                }
                case LanguageEnum.zht: {
                    this.zht.isOn = true;
                    break;
                }
            }

            this.restart = true;
        }

        void Start() {
            this.zh.GetComponent<SettingToggle>().Refresh();
            this.en.GetComponent<SettingToggle>().Refresh();
            this.zht.GetComponent<SettingToggle>().Refresh();
        }

        // Update is called once per frame
        void Update() {
            
        }

        public void OnToggleClick() {
            foreach (var activeToggle in this.@group.ActiveToggles()) {
                PlayerPrefsAES.SetString("lang", activeToggle.name);
                break;
            }
            if (this.restart) {
                //Black.FadeOutIn(0.35f, () => Client.Ins.RestartGame());
            }
        }
    }
}