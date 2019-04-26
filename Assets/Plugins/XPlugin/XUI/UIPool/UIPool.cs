using System;
using System.Collections.Generic;
using UnityEngine;

namespace XUI {

	public class UIPool : MonoBehaviour {
		private Dictionary<GameObject, UIPoolObjList> _prefabToList;
		private Dictionary<GameObject, UIPoolObjList> _insToList;

		protected virtual void Awake() {
			_prefabToList = new Dictionary<GameObject, UIPoolObjList>();
			_insToList = new Dictionary<GameObject, UIPoolObjList>();
		}

		/// <summary>
		/// 清理对象池
		/// </summary>
		/// <param name="destroy">是否销毁对象池中的物体</param>
		public void Clean(bool destroy) {
			foreach (var list in _prefabToList.Values) {
				list.Clean(destroy);
			}
			_prefabToList.Clear();
			_insToList.Clear();
		}

		/// <summary>
		/// 产生物体
		/// 试图从对象池中获取一个物体，如果没有则实例化一个物体
		/// </summary>
		public GameObject Spawn(GameObject prefab) {
			UIPoolObjList list;
			if (!_prefabToList.TryGetValue(prefab, out list)) {
				list = new UIPoolObjList();
				_prefabToList.Add(prefab, list);
			}
			var ins = list.Spawn(prefab,transform);
			_insToList.Add(ins, list);
			ins.SetActive(true);
			var behaviours = ins.GetComponents<IUIPoolBehaviour>();
			foreach (var b in behaviours) {
				b.OnUISpawned();
			}
//			if (behaviours.Length == 0) {
//				ins.SendMessage("OnUISpawned", SendMessageOptions.DontRequireReceiver);//以后不发消息了，用接口回调
//			}
			return ins;
		}

		/// <summary>
		/// 回收物体
		/// </summary>
		/// <param name="ins">回收物体的实例</param>
		public void Despawn(GameObject ins, bool destroy) {
			if (ins == null) {
				return;
			}
			UIPoolObjList list;
			if (!_insToList.TryGetValue(ins, out list)) {
				return;
			}
			list.Despawn(ins, destroy);
			_insToList.Remove(ins);
			var behaviours = ins.GetComponents<IUIPoolBehaviour>();
			foreach (var b in behaviours) {
				b.OnUIDespawn();
			}
//			if (behaviours.Length == 0) {
//				ins.SendMessage("OnUIDespawn", SendMessageOptions.DontRequireReceiver);//以后不发消息了，用接口回调
//			}
			if (destroy) {
				Destroy(ins);
			} else {
				ins.SetActive(false);
				ins.transform.SetParent(transform);
			}
		}

	}
}
