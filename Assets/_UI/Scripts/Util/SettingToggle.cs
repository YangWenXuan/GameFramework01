using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
    public class SettingToggle : MonoBehaviour {
        public GameObject onNode;
        public GameObject offNode;

        private Toggle _toggle;

        private void Awake() {
            this._toggle = GetComponent<Toggle>();
            this._toggle.onValueChanged.AddListener(OnValueChange);
        }

        private void OnValueChange(bool value) {
            Refresh();
        }

        public void Refresh() {
            this.onNode.SetActive(this._toggle.isOn);
            this.offNode.SetActive(!this._toggle.isOn);
        }
        
    }
}