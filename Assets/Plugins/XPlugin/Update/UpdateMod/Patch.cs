using System.Collections.Generic;
using XPlugin.Data.Json;

namespace XPlugin.Update {

	[System.Serializable]
	public struct Patch {
		public int Version;
		public float Size;//kb
		public string MD5;

		public bool NeedDelete {
			get { return string.IsNullOrEmpty(MD5); }
		}

		public JObject ToJObject() {
			JObject ret = new JObject();
			ret["Version"] = Version;
			ret["Size"] = Size;
			ret["MD5"] = MD5;
			return ret;
		}

		public Patch(int version, float size, string md5) {
			Version = version;
			Size = size;
			MD5 = md5;
		}

		public Patch(JObject jobj) {
			Version = jobj["Version"].OptInt(0);
			Size = jobj["Size"].OptFloat(0);
			MD5 = jobj["MD5"].OptString();
		}

		public static JObject DicToJObj(Dictionary<string, Patch> dic) {
			JObject ret = new JObject();
			foreach (var patchKV in dic) {
				JObject patchJobj = patchKV.Value.ToJObject();
				ret[patchKV.Key] = patchJobj;
			}
			return ret;
		}

		public static Dictionary<string, Patch> CreateDicFromJObj(JObject jobj) {
			var ret = new Dictionary<string, Patch>();
			foreach (var token in jobj) {
				if (token != null) {
					JObject patchJobj = token.OptObject();
					Patch patch = new Patch(patchJobj);
					ret.Add(token.Name, patch);
				}
			}
			return ret;
		}

	}
}