using XPlugin.Data.Json;

namespace XPlugin.Update {

	[System.Serializable]
	internal class MainPakIndex {
		public int Version = 10000;//01.00.00
		public string URL = "http://192.168.0.1/MainDownload/110.apk";
		public string Info = "默认整包更新公告";

		public MainPakIndex() { }

		public MainPakIndex(int versionCode, JObject jobj) {
			Version = jobj["Version"].OptInt(versionCode);
			Info = jobj["Info"].OptString();
			URL = jobj["URL"].OptString();
		}
	}
}