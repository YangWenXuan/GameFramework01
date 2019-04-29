using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace XPlugin.Http {

	public delegate void OnHttpResponse(string error, WWW www);

	public delegate void OnProgress(float progress);

	internal class HttpRequest {
		public WWW www;
		public OnHttpResponse onDone;
		public OnProgress onProgress;

		public float AddTime;
		public float TimeOut;

		public void CallOnDone(string error) {
			if (onDone != null) {
				onDone(error, www);
			}
		}


		public void CallOnProgress() {
			if (onProgress != null) {
				onProgress(www.progress);
			}
		}
	}

}