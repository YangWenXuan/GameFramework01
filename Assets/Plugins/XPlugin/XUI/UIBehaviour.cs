using UnityEngine;

namespace XUI {

	public class SingleUiBehaviour<T> : UIBehaviour where T: SingleUiBehaviour<T> {
		public static T Ins { get; private set; }

		public override void OnUIShow(params  object[] args) {
			Ins = (T) this;
			base.OnUIShow(args);
		}

		public override void OnUIClose() {
			base.OnUIClose();
			if (Ins == this) {
				Ins = null;
			}
		}
	}

	public class UIBehaviour :MonoBehaviour,IUIPoolBehaviour,IUIStackBehaviour{
		
		// public void OnUIShowArgs(xxx){}
		public virtual void OnUISpawned() {
		}

		public virtual void OnUIDespawn() {
		}

		public virtual void OnUIShow(params object[] args) {
		}

		public virtual void OnUIClose() {
		}

		public virtual  void OnUIPause(bool cover) {
		}

		public virtual void OnUIResume(bool fromCover) {
		}
	}
}