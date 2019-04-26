using System.Linq;
using System.Reflection;
using UnityEngine;

namespace XUI {
    public class UIItem {
        public GameObject Prefab;
        public GameObject Instance;
        public Canvas InstanceCanvas;
        public IUIStackBehaviour[] UiStackBehaviours;

        public UIItem(GameObject prefab, GameObject instance, Canvas instanceCanvas) {
            this.Prefab = prefab;
            this.Instance = instance;
//            this.InstanceCanvas = instanceCanvas;
            UiStackBehaviours = this.Instance.GetComponents<IUIStackBehaviour>();
        }

        public void SendDeOverlay() {
            foreach (var behaviour in this.UiStackBehaviours) {
                behaviour.OnUIResume(false);
            }
        }

        public void SendBeenOverlay() {
            foreach (var behaviour in this.UiStackBehaviours) {
                behaviour.OnUIPause(false);
            }
        }

        public void SendLeaveStack() {
            foreach (var behaviour in this.UiStackBehaviours) {
                behaviour.OnUIClose();
            }
        }

        public void SendEnterStack(params object[] args) {
            foreach (var behaviour in this.UiStackBehaviours) {
                behaviour.OnUIShow(args);
                var ms = behaviour.GetType().GetMethod("OnUIShowArgs",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                ms?.Invoke(behaviour, args);
            }
        }

        public void SendDeCover() {
            foreach (var behaviour in this.UiStackBehaviours) {
                behaviour.OnUIResume(true);
            }
        }

        public void SendBeenCover() {
            foreach (var behaviour in this.UiStackBehaviours) {
                behaviour.OnUIPause(true);
            }
        }
    }
}