using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace XPlugin.Localization  {
    public class ShowByLang : MonoBehaviour {
        [ReorderableList]
        public LanguageEnum[] langs;
        public bool show;

        [ReorderableList]
        public GameObject[] Targets;

        
        private void Awake() {
            var enable = this.langs.Any(lang =>  lang==Localization.Language);
            if (!show) {
                enable = !enable;
            }
            if (Targets.Length == 0) {
                gameObject.SetActive(enable);
            } else {
                foreach (var o in Targets) {
                    o.SetActive(enable);
                }
            }
        }

    }
}