using System;
using UnityEngine;

namespace XUI {
	public class UIPoolTest : MonoBehaviour {

		public UIPool pool;

		public GameObject prefab1;

		public GameObject ins;


		void OnGUI() {
			LayoutButton("ins", () => {
				this.ins = this.pool.Spawn(this.prefab1);
			});
			LayoutButton("despawn", () => {
				this.pool.Despawn(ins, false);
			});
			LayoutButton("destroy", () => {
				this.pool.Despawn(ins, true);
			});
		}


		public void LayoutButton(string text, Action onClick) {
			if (GUILayout.Button(text)) {
				onClick();
			}
		}


	}
}