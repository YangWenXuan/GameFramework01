using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XUI {
	public class UIPage : LinkedList<UIGroup> {

		public void SendBeenCover() {
			foreach (var g in this) {
				foreach (var item in g) {
					item.SendBeenCover();
				}
			}
		}

		public void SendDeCover() {
			foreach (var g in this) {
				foreach (var item in g) {
					item.SendDeCover();
				}
			}
		}
	}


	public class UIGroup : List<UIItem> {

		public GameObject gameObject;
		private UIStack Stack;

		public UIGroup(UIStack stack, IEnumerable<GameObject> prefabs) {
			this.Stack = stack;
			if (stack.CreateGroupGameObject) {
				gameObject = new GameObject("Group",typeof(RectTransform));
				RectTransform transform = (RectTransform) gameObject.transform;
				transform.anchorMin=Vector2.zero;
				transform.anchorMax=Vector2.one;
				transform.pivot = Vector2.one * 0.5f;
				transform.offsetMin=Vector2.zero;
				transform.offsetMax=Vector2.zero;
				gameObject.transform.SetParent(stack.transform,false);
			}

			foreach (var prefab in prefabs) {
				Add(stack.PrefabToUIItem(prefab, gameObject));
			}

			stack.OnUIGroupCreated(this);
		}

		public void Spawn() {
			foreach (var item in this) {
				var ins = Stack.Spawn(item.Prefab);
				item.Instance = ins;
//				item.InstanceCanvas = ins.GetOrAddComponent<Canvas>();
//				ins.GetOrAddComponent<GraphicRaycaster>();
				if (gameObject != null) {
					ins.transform.SetParent(gameObject.transform);
				}
			}
		}

		public void Despawn(bool destroy) {
			foreach (var item in this) {
				Stack.Despawn(item.Instance, destroy);
			}
			if (gameObject != null) {
				Object.Destroy(gameObject);//TODO 考虑对象池
			}
		}

		public void SendEnterStack(params  object[] args) {
			foreach (var item in this) {
				if (item.Instance == null) {
					continue;
				}
				item.SendEnterStack(args);
			}
		}

		public void SendLeaveStack() {
			foreach (var item in this) {
				if (item.Instance == null) {
					continue;
				}
				item.SendLeaveStack();
			}
		}

		public void SendBeenOverlay() {
			foreach (var item in this) {
				if (item.Instance == null) {
					continue;
				}
				item.SendBeenOverlay();//不覆盖.
			}
		}

		public void SendDeOverlay() {
			foreach (var item in this) {
				if (item.Instance == null) {
					continue;
				}
				item.SendDeOverlay();
			}
		}

	}

}