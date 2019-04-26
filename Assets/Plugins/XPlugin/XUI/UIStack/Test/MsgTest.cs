using UnityEngine;

namespace XUI {
	public class MsgTest :MonoBehaviour, IUIStackBehaviour,IUIPoolBehaviour{

		public void OnUISpawned() {
			Debug.Log(gameObject + " OnUISpawned");
		}

		public void OnUIDespawn() {
			Debug.Log(gameObject + " OnUIDespawn");
		}


		public void OnUIShow(params object[] args) {
			string arg = "";
			foreach (var o in args) {
				arg += o.ToString();
			}

			Debug.Log(gameObject + " OnUIShow : "+arg);
		}
		
		public void OnUIClose() {
			Debug.Log(gameObject + " OnUIClose");
		}

		public void OnUIPause(bool cover) {
			Debug.Log(gameObject + " OnUIPause "+cover);
		}

		public void OnUIResume(bool fromCover) {
			Debug.Log(gameObject + " OnUIResume" + fromCover);
		}
	}
}