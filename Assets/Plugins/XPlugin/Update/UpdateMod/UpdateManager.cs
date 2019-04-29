// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)
#define UPDATER_LOG

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XPlugin.Data.Json;
using XPlugin.Http;
using XPlugin.Security;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XPlugin.Update {

	public class UpdateManager : MonoBehaviour {
		private static UpdateManager _ins;
		public static UpdateManager Ins {
			private set {
				_ins = value;
			}
			get {
				if (_ins == null) {
					var go = new GameObject("UpdateManager(AUTO)");
					go.hideFlags = HideFlags.DontSave;
					DontDestroyOnLoad(go);
					_ins = go.AddComponent<UpdateManager>();
				}
				return _ins;
			}
		}

		internal const string INDEX_FILE_NAME = ".patchIndex";
		internal static string Full_Index_File_Path {
			get { return ResManager.FullDownloadDir + INDEX_FILE_NAME; }
		}

		[System.NonSerialized]
		public int VersionCode;
		[System.NonSerialized]
		public string CheckUpdateUrl;

		public JObject CustomData;

		public IUpdateListener Listener;


		private MainPakIndex _mainPakIndexFromServer;
		private PatchIndex _patchIndexNew;
		private PatchIndex _patchIndexLocal;
		private UpdateType _updateType;


		private void Awake() {
			Ins = this;
		}

		private void OnDestroy() {
			Ins = null;
		}

		public void Init(int versionCode) {
			this.VersionCode = versionCode;
			//检查是否有patchIndex文件,并读取内容
			var indexFile = UResources.ReqFile(INDEX_FILE_NAME);
			if (indexFile != null) {
				string fileJson = DESFileUtil.ReadAllText(indexFile.FullName, DESKey.DefaultKey, DESKey.DefaultIV);
				_patchIndexLocal = new PatchIndex(this.VersionCode, JObject.Parse(fileJson));
				if (_patchIndexLocal.Version != this.VersionCode) {
					UpdaterLog.Log("发现patchIndex中的主版本不等于目前的主版本，说明刚经历过主包更新，清空补丁");
					foreach (var file in ResManager.Ins.DownloadedFiles.Values) {
						File.Delete(file.FullName);
					}
					ResManager.Ins.ReadDownloadDir();
					_patchIndexLocal = new PatchIndex();
				}
			} else {
				_patchIndexLocal = new PatchIndex();
			}
		}

		/// <summary>
		/// 检查更新
		/// </summary>
		/// <param name="url"></param>
		/// <param name="customData"></param>
		/// <param name="listener"></param>
		public void CheckUpdate(string url, JObject customData, IUpdateListener listener) {
			this.CheckUpdateUrl = url;
			this.CustomData = customData;
			this.Listener = listener;
			CheckUpdate();
		}


		private void CheckUpdate() {
			//检查是否有patchIndex文件,并读取内容
			Init(this.VersionCode);

			//构建检查更新需要的表单
			JObject json = new JObject();
			json["version"] = this.VersionCode;
			json["patchVersion"] = _patchIndexLocal.PatchVersion;
			if (CustomData != null) {
				foreach (var kv in CustomData) {
					json[kv.Name] = kv;
				}
			}
			WWWForm form = new WWWForm();
			form.AddField("P", json.ToString());

			HttpManager.Post(CheckUpdateUrl, form, (error, www) => {
				if (!string.IsNullOrEmpty(error)) {
					Listener.OnError(error);
					return;
				}
				UpdaterLog.Log(www.text);

				JObject responseJObj = JObject.Parse(www.text);
				if (responseJObj["E"].GetString() != null) {
					Listener?.OnError(responseJObj["E"].AsString());
					return;
				}
				responseJObj = responseJObj["P"].OptObject();
				_updateType = (UpdateType)(responseJObj["Type"].OptInt(0));

				switch (_updateType) {
					case UpdateType.NoUpdate:
						_patchIndexNew = new PatchIndex(this.VersionCode, responseJObj);
						//没有找到更新，验证文件
						Listener.OnVerifyStart();
						if (VerifyPatches()) {
							Listener.OnVerifySuccess();
							Listener.OnPassUpdate();
						} else {
							Listener.OnVerifyFail();
						}
						break;
					case UpdateType.MainPak:
						_mainPakIndexFromServer = new MainPakIndex(this.VersionCode, responseJObj);
						Listener.OnFindMainUpdate(_mainPakIndexFromServer.Info);
						break;
					case UpdateType.Patch:
						_patchIndexNew = new PatchIndex(this.VersionCode, responseJObj);
						if (Listener != null) {
							Listener.OnFindPatchUpdate(_patchIndexNew.PatchInfo, _patchIndexNew.TotalPatchSize);
						}
						break;
				}
			});
		}

		/// <summary>
		/// 前往整包更新
		/// </summary>
		public void GoToMainUpdate() {
			if (_mainPakIndexFromServer != null) {
				Application.OpenURL(_mainPakIndexFromServer.URL);
			}
		}

		/// <summary>
		/// 开始下载补丁包
		/// </summary>
		public void GotoPatchUpdate() {
			if (_patchIndexNew != null && _patchIndexLocal != null) {
				//比较服务器patchIndex和本地patchIndex得到需要更新的文件
				Dictionary<string, Patch> patchToDownload;
				if (_updateType == UpdateType.NoUpdate) {//如果更新结果为，没有更新，表示文件验证失败，直接使用patches
					patchToDownload = _patchIndexNew.Patches;
				} else {
					patchToDownload = PatchIndex.FilterUpdatePatch(_patchIndexLocal, _patchIndexNew);
				}

				var enumerator = patchToDownload.GetEnumerator();
				DownloadPatch(enumerator, 0, patchToDownload.Count);
			}
		}

		void DownloadPatch(Dictionary<string, Patch>.Enumerator enumerator, int i, int totalToDownload) {
			if (!enumerator.MoveNext()) {
				//下载完毕，将新版本号写入patchIndex,并保存
				_patchIndexLocal.Version = VersionCode;
				_patchIndexLocal.PatchVersion = _patchIndexNew.PatchVersion;
				_patchIndexLocal.WriteToFile();
				ResManager.Ins.ReadDownloadDir();
				if (Listener != null) {
					Listener.OnPatchDownloadFinish();
				}
				CheckUpdate();//补丁更新结束后再次检查更新
				return;
			}
			i++;
			var patchKV = enumerator.Current;
			var name = patchKV.Key;
			var patch = patchKV.Value;
			if (patch.NeedDelete) {//如果这个补丁是删除更新，则删除
				FileInfo file = UResources.ReqFile(name);
				if (file != null) {
					File.Delete(file.FullName);
				}
				if (Listener != null) {
					Listener.OnPatchDownloadProgress(name, 1, i, totalToDownload);
				}
				_patchIndexLocal.UpdatePatch(name, patch);
				_patchIndexLocal.WriteToFile();
				DownloadPatch(enumerator, i, totalToDownload);
				return;
			}
			//下载更新
			string url = String.Format(_patchIndexNew.PatchBaseURL, name, patch.Version);
			HttpManager.Get(url, (error, www) => {
				if (!string.IsNullOrEmpty(error)) {
					UpdaterLog.LogError("下载补丁包出错" + name + "  " + www.error + "  url:" + www.url);
					if (Listener != null) {
						Listener.OnPatchDownloadFail(error);
					}
					return;
				}
				string filePath = ResManager.FullDownloadDir + name;
				File.WriteAllBytes(filePath, www.bytes);
				//下载完一个文件后立刻写入PatchIndex，这样中途断开网络后重新下载可以不下载该文件
				_patchIndexLocal.UpdatePatch(name, patch);
				_patchIndexLocal.WriteToFile();
				DownloadPatch(enumerator, i, totalToDownload);
				return;
			}, progress => {
				if (Listener != null) {
					Listener.OnPatchDownloadProgress(name, progress, i, totalToDownload);
				}
			});
		}

		private bool VerifyPatches() {
			var failPatch = _patchIndexLocal.VerifyPatches();
			if (failPatch == null || failPatch.Count == 0) {//如果没有发现验证失败的文件则退出
				return true;
			} else {
				_patchIndexNew.SetNewPatches(failPatch);
				if (Listener != null) {
					Listener.OnFindPatchUpdate(_patchIndexNew.PatchInfo, _patchIndexNew.TotalPatchSize);
				}
				return false;
			}
		}

		/// <summary>
		/// 获取补丁版本号
		/// </summary>
		public int PatchVersionCode {
			get {
				if (_patchIndexLocal != null) {
					return _patchIndexLocal.PatchVersion;
				} else {
					UpdaterLog.LogError("PatchIndex file not inited");
					return -1;
				}
			}
		}

	}
}