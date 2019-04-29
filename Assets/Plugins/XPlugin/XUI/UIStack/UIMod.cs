using System.Collections.Generic;
using UnityEngine;
using XPlugin.Localization;

namespace XUI {

	public class UIMod<T> : UIMod where T : UIMod<T> {
		protected static T _instance;

		public static T Ins {
			get { return _instance; }
		}

		protected override void Awake() {
			base.Awake();
			_instance = (T)this;
		}

		protected virtual void OnDestroy() {
			_instance = null;
		}
	}

	public class UIMod : UIStack {
		public string Current { get; private set; }

		public virtual GameObject Spawn(string name) {
			return Spawn(GetPrefab(name));
		}

		public virtual UIGroup Cover(IEnumerable<string> names, bool destroyBefore = false,params object[] args) {
			List<GameObject> group = new List<GameObject>();
			foreach (string t in names) {//names 里面存储着
				@group.Add(GetPrefab(t));//相当于Resources.Load()这里加载Group内的成员.
			}
			var result = Cover(group, destroyBefore,args);
			return result;
		}


		public virtual UIGroup Overlay(IEnumerable<string> names,params object[] args) {
			List<GameObject> group = new List<GameObject>();
			foreach (string t in names) {
				@group.Add(GetPrefab(t));
			}
			var result = Overlay(group,args);
			return result;
		}

        public static GameObject GetPrefab(string name)
        {
            return LResources.Load<GameObject>(name);
        }

        //public static GameObject GetPrefab(string name)
        //{
        //	return Resources.Load<GameObject>(name);
        //}
    }
}