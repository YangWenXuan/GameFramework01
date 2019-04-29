// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using System.Collections.Generic;
using System.IO;
using XPlugin.Data.Json;
using XPlugin.Security;


namespace XPlugin.Update {

	[System.Serializable]
	internal class PatchIndex {
		public int Version = -1;
		public int PatchVersion = 0;
		public string PatchBaseURL = "http://192.168.0.1/PatchDownload/{0}-{1}";
		public string PatchInfo = "默认补丁包更新公告";
		public Dictionary<string, Patch> Patches = new Dictionary<string, Patch>();

		public float TotalPatchSize { private set; get; }

		public PatchIndex() { }

		public PatchIndex(int versionCode, JObject jobj) {
			Version = jobj["Version"].OptInt(versionCode);
			PatchVersion = jobj["PatchVersion"].OptInt(0);
			PatchInfo = jobj["Info"].OptString(null);
			PatchBaseURL = jobj["URL"].OptString(null);
			Patches = Patch.CreateDicFromJObj(jobj["Patches"].OptObject());
			CalcTotalSize();
		}

		internal void CalcTotalSize() {
			TotalPatchSize = 0f;
			foreach (var patch in Patches.Values) {
				if (!patch.NeedDelete) {
					TotalPatchSize += patch.Size;
				}
			}
		}

		public void SetNewPatches(Dictionary<string, Patch> newPatch) {
			this.Patches = newPatch;
			this.CalcTotalSize();
		}

		public void WriteToFile() {
			JObject index = new JObject();

			index["Version"] = Version;
			index["PatchVersion"] = PatchVersion;
			index["PatchBaseURL"] = PatchBaseURL;

			index["Patches"] = Patch.DicToJObj(Patches);
			DESFileUtil.WriteAllText(UpdateManager.Full_Index_File_Path, index.ToString(), DESKey.DefaultKey, DESKey.DefaultIV);
		}


		internal void UpdatePatch(string name, Patch patch) {
			if (!Patches.ContainsKey(name)) {
				Patches.Add(name, patch);
			} else {
				Patches[name] = patch;
			}
		}


		/// <summary>
		/// 比较两个IndexJson中的Patch，过滤出需要更新的Patch
		/// </summary>
		/// <param name="local"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public static Dictionary<string, Patch> FilterUpdatePatch(PatchIndex local, PatchIndex server) {
			var ret = new Dictionary<string, Patch>();
			//比较本地和服务器patch，过滤需要更新的patch
			foreach (var serverPatchKV in server.Patches) {
				var serverPatchName = serverPatchKV.Key;
				var serverPatch = serverPatchKV.Value;
				if (!local.Patches.ContainsKey(serverPatchName)) {
					ret.Add(serverPatchName, serverPatch);
				} else {//本地已经有这个补丁
					if (serverPatch.NeedDelete) {//如果是一个删除补丁，则需要更新（删除）
						ret.Add(serverPatchName, serverPatch);
					} else {//不是删除补丁，则比较版本号
						var localPatch = local.Patches[serverPatchName];
						if (localPatch.Version < serverPatch.Version) {//版本较低，则需要更新
							ret.Add(serverPatchName, serverPatch);
						}
					}
				}
			}
			return ret;
		}

		public Dictionary<string, Patch> VerifyPatches() {
			var ret = new Dictionary<string, Patch>();
			//验证本地补丁
			foreach (var patchKV in Patches) {
				var name = patchKV.Key;
				var patch = patchKV.Value;
				FileInfo file = UResources.ReqFile(name);
				if (patch.NeedDelete) {//如果这个补丁是删除补丁，验证他是否真的被删除
					if (file != null) {
						UpdaterLog.LogError(string.Format("{0} 这个文件理应被删除，但验证发现没有被删除，将更新", name));
						ret.Add(name, patch);
					}
				} else {//这是更新补丁，检测是否存在，并验证md5
					if (file == null) {
						UpdaterLog.LogError(string.Format("{0} 这个文件验证时发现应该存在，但实际不存在，将更新", name));
						ret.Add(name, patch);
					} else {
						string fileMd5 = MD5Util.GetMD5ForFile(file.FullName);
						string recordMd5 = patch.MD5;
						if (!fileMd5.EqualIgnoreCase(recordMd5)) {
							UpdaterLog.LogError(string.Format("{0} 这个文件验证的md5和记录的md5不一致{1}//{2}，将更新", name, fileMd5, recordMd5));
							ret.Add(name, patch);
						}
					}

				}
			}
			return ret;
		}
	}
}