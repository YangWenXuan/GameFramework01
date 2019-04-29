
#define UPDATER_LOG //open log

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;

namespace XPlugin.Update
{
	[ExecuteInEditMode]
	public class ResManager : MonoBehaviour
	{
		private static ResManager _ins;
		public static ResManager Ins {
			private set { _ins = value; }
			get {
				if (_ins == null) {
					var go = new GameObject("ResManager(AUTO)");
					go.hideFlags = HideFlags.DontSave;
					if (Application.isPlaying) {
						DontDestroyOnLoad(go);
					}
					_ins = go.AddComponent<ResManager>();
				}
				return _ins;
			}
		}

		/// <summary>
		/// 下载目录子目录
		/// </summary>
		internal const string DOWNLOAD_DIR_NAME = "/Update/";
		/// <summary>
		/// 完整的下载目录
		/// </summary>
		public static string FullDownloadDir {
			get { return Application.persistentDataPath + DOWNLOAD_DIR_NAME; }
		}

		/// <summary>
		/// 路径中的目录分隔符
		/// </summary>
		public const string DIR_SPLIT = "_!_";

		/// <summary>
		/// 已下载的文件
		/// key:文件名
		/// value:文件FileInfo
		/// </summary>
		internal Dictionary<string, FileInfo> DownloadedFiles = new Dictionary<string, FileInfo>();
		/// <summary>
		/// 缓存的unity objects
		/// 请求过的物体会在这里缓存起来
		/// </summary>
		internal Dictionary<string, Object> CachedObjects = new Dictionary<string, Object>();
		internal Dictionary<string, AssetBundle> CachedSceneBundles = new Dictionary<string, AssetBundle>();

		internal Dictionary<string, Queue<Action<Object>>> AsyncLoadingQueue = new Dictionary<string, Queue<Action<Object>>>();
		internal Dictionary<string, Queue<Action<bool>>> AsyncLoadingSceneQueue = new Dictionary<string, Queue<Action<bool>>>();


		private void Awake()
		{
			Ins = this;
			ReadDownloadDir();
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		void OnDestroy()
		{
			Ins = null;
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}

		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			UpdaterLog.Log("Clean LevelCached Start");
			//在切换关卡的时候将保留的Bundle释放
			foreach (var bundle in CachedSceneBundles.Values) {
				bundle.Unload(false);
			}
			StartCoroutine(ClearDelay());
		}

		IEnumerator ClearDelay()
		{
			yield return 0;
			//释放资源物体的引用
			CachedObjects.Clear();
			//释放场景bundle的引用  
			CachedSceneBundles.Clear();
			UpdaterLog.Log("Clean LevelCached End");
		}

		/// <summary>
		/// 读取下载目录，将下载目录中的文件缓存起来
		/// </summary>
		internal void ReadDownloadDir()
		{
			DownloadedFiles.Clear();
			CachedObjects.Clear();
			CachedSceneBundles.Clear();
			AsyncLoadingQueue.Clear();
			AsyncLoadingSceneQueue.Clear();

			if (Directory.Exists(FullDownloadDir)) {
				// 找到所有的已下载文件
				var files = Directory.GetFiles(FullDownloadDir);
				foreach (var file in files) {
					FileInfo info = new FileInfo(file);
					UpdaterLog.Log("find downloaded file:" + info);
					var fileName = Path.GetFileName(file);
					DownloadedFiles.Add(fileName, info);
				}
			} else {
				UpdaterLog.Log("create directory:" + FullDownloadDir);
				Directory.CreateDirectory(FullDownloadDir);
			}
		}

		#region Load
		internal Object Load(string path, Type type)
		{
			Object obj = null;
			if (CachedObjects.TryGetValue(path, out obj)) {// 尝试在缓存中查找
				return obj;
			} else {// 尝试从外部文件中加载asset bundle
				var fileName = path.Replace("/", DIR_SPLIT);//替换目录分隔符
				if (DownloadedFiles.ContainsKey(fileName)) {
					FileInfo file = DownloadedFiles[fileName];
					var bytes = File.ReadAllBytes(file.FullName);
					var ab = AssetBundle.LoadFromMemory(bytes);
					string objectName = Path.GetFileName(path);
					obj = ab.LoadAsset(objectName, type);
					ab.Unload(false);//一旦加载完毕，立即释放assetbundle，但不释放bundle中的物体,物体会在场景切换时释放
					if (obj == null) {
						UpdaterLog.LogError(string.Format("the resource {0} load from ab is null", path));
						return null;
					}
					CachedObjects.Add(path, obj);
				} else {
					obj = Resources.Load(path, type);
				}
			}
			return obj;
		}

		#endregion Load

		#region LoadAsync
		internal void LoadAsync(string path, Type type, Action<Object> onDone)
		{
			Object obj = null;
			if (CachedObjects.TryGetValue(path, out obj)) {// 尝试在缓存中查找
				if (onDone != null) {
					onDone(obj);
				}
			} else {
				Queue<Action<Object>> onDoneQueue;
				if (AsyncLoadingQueue.TryGetValue(path, out onDoneQueue)) {//如果该物体已经在加载中，则将回调直接入列，等待加载结束调用即可
					onDoneQueue.Enqueue(onDone);
					return;
				} else {
					onDoneQueue = new Queue<Action<Object>>();
					AsyncLoadingQueue.Add(path, onDoneQueue);
					onDoneQueue.Enqueue(onDone);
				}
				var fileName = path.Replace("/", DIR_SPLIT);//替换目录分隔符
				if (DownloadedFiles.ContainsKey(fileName)) {// 尝试从外部文件中加载asset bundle
					FileInfo file = DownloadedFiles[fileName];
					var bytes = File.ReadAllBytes(file.FullName);
					StartCoroutine(AsyncCreateABReqObj(path, type, bytes));
				} else {
					StartCoroutine(AsyncResourcesLoad(path, type));
				}
			}
		}

		IEnumerator AsyncResourcesLoad(string path, Type type)
		{
			var req = Resources.LoadAsync(path, type);
			yield return req;
			Object obj = req.asset;
			Queue<Action<Object>> onDoneQueue = AsyncLoadingQueue[path];
			if (onDoneQueue == null) {
				UpdaterLog.LogException(new Exception("internal error , the AsyncLoadingQueue have no queue named :" + path));
				yield break;
			}
			while (onDoneQueue.Count > 0) {
				onDoneQueue.Dequeue()(obj);
			}
			this.AsyncLoadingQueue.Remove(path);
		}

		IEnumerator AsyncCreateABReqObj(string path, Type type, byte[] bytes)
		{
			var abCreateReq = AssetBundle.LoadFromMemoryAsync(bytes);//异步创建ab
			yield return abCreateReq;
			string objectName = Path.GetFileName(path);
			var abReq = abCreateReq.assetBundle.LoadAssetAsync(objectName, type);//异步读取物体
			yield return abReq;
			Object obj = abReq.asset;
			abCreateReq.assetBundle.Unload(false);//一旦加载完毕，立即释放assetbundle，但不释放bundle中的物体,物体会在场景切换时释放
			if (obj == null) {
				UpdaterLog.LogError(string.Format("the resource {0} load async from ab is null", path));
			} else {
				UpdaterLog.Log(string.Format("load async the resource {0} from ab", path));
				if (!CachedObjects.ContainsKey(path)) {
					CachedObjects.Add(path, obj);
				}

			}
			Queue<Action<Object>> onDoneQueue = AsyncLoadingQueue[path];
			if (onDoneQueue == null) {
				UpdaterLog.LogException(new Exception("internal error , the AsyncLoadingQueue have no queue named :" + path));
				yield break;
			}
			while (onDoneQueue.Count > 0) {
				onDoneQueue.Dequeue()(obj);
			}
			this.AsyncLoadingQueue.Remove(path);
		}

		#endregion LoadAsync

		#region ReqScene
		internal bool ReqScene(string path)
		{
			if (!CachedSceneBundles.ContainsKey(path)) {
				if (DownloadedFiles.ContainsKey(path)) {
					FileInfo file = DownloadedFiles[path];
					var bytes = File.ReadAllBytes(file.FullName);
					var ab = AssetBundle.LoadFromMemory(bytes);
					if (ab != null) {  //判断AB中是否有该场景
						var scenePaths = ab.GetAllScenePaths();
						foreach (var scenePath in scenePaths) {
							if (Path.GetFileNameWithoutExtension(scenePath) == path) {
								UpdaterLog.Log("find scene in assetbundle:" + path);
								CachedSceneBundles.Add(path, ab);
								break;
							}
						}
					}
				}
			}
			return Application.CanStreamedLevelBeLoaded(path);
		}
		#endregion ReqScene

		#region ReqSceneAsync
		internal void ReqSceneAsync(string path, Action<bool> onDone)
		{
			if (CachedSceneBundles.ContainsKey(path)) {//已经在缓存列表中，则直接返回
				onDone(Application.CanStreamedLevelBeLoaded(path));
			} else {
				if (DownloadedFiles.ContainsKey(path)) {
					Queue<Action<bool>> onDoneQueue;
					if (AsyncLoadingSceneQueue.TryGetValue(path, out onDoneQueue)) {
						//如果该场景已经在加载中，则将回调直接入列，等待加载结束调用即可
						onDoneQueue.Enqueue(onDone);
						return;
					} else {
						onDoneQueue = new Queue<Action<bool>>();
						AsyncLoadingSceneQueue.Add(path, onDoneQueue);
						onDoneQueue.Enqueue(onDone);
					}

					FileInfo file = DownloadedFiles[path];
					var bytes = File.ReadAllBytes(file.FullName);
					StartCoroutine(AsyncCreateABReqScene(path, bytes));
				} else {
					if (onDone != null) {
						onDone(Application.CanStreamedLevelBeLoaded(path));
					}
				}
			}
		}

		IEnumerator AsyncCreateABReqScene(string path, byte[] bytes)
		{
			var abCreateReq = AssetBundle.LoadFromMemoryAsync(bytes);
			yield return abCreateReq;

			var ab = abCreateReq.assetBundle;
			if (ab != null) {  //判断AB中是否有该场景
				var scenePaths = ab.GetAllScenePaths();
				foreach (var scenePath in scenePaths) {
					if (Path.GetFileNameWithoutExtension(scenePath) == path) {
						UpdaterLog.Log("find scene in assetbundle:" + path);
						if (!CachedSceneBundles.ContainsKey(path)) {
							CachedSceneBundles.Add(path, ab);
						}
						break;
					}
				}
			}

			Queue<Action<bool>> onDoneQueue = AsyncLoadingSceneQueue[path];
			if (onDoneQueue == null) {
				UpdaterLog.LogException(new Exception("Internal error , the AsyncLoadingQueue have no queue named :" + path));
				yield break;
			}

			bool result = Application.CanStreamedLevelBeLoaded(path);
			while (onDoneQueue.Count > 0) {
				onDoneQueue.Dequeue()(result);
			}
			this.AsyncLoadingSceneQueue.Remove(path);
		}

		#endregion ReqSceneAsnc

		#region LoadStreamingAsset

		internal string GetUpdatedStreamingAssetPath(string path)
		{
			var fileName = Path.GetFileNameWithoutExtension(path);
			if (DownloadedFiles.ContainsKey(fileName)) {
				path = "file://" + DownloadedFiles[fileName].FullName;
				UpdaterLog.Log("find streaming asset at update path : " + path);
			} else {
				path = Path.Combine(Application.streamingAssetsPath, path);
				if (!path.Contains("://")) {
					path = "file://" + path;
				}
			}
			return path;
		}


		internal WWW LoadStreamingAsset(string path)
		{
			WWW www = new WWW(GetUpdatedStreamingAssetPath(path));
			while (!www.isDone) {
				if (!string.IsNullOrEmpty(www.error)) {
					break;
				}
			}
			return www;
		}

		internal void LoadStreamingAssetAsync(string path, Action<WWW> onDone)
		{
			StartCoroutine(AsyncLoadStreamingAsset(GetUpdatedStreamingAssetPath(path), onDone));
		}

		IEnumerator AsyncLoadStreamingAsset(string path, Action<WWW> onDone)
		{
			WWW www = new WWW(path);
			yield return www;
			if (onDone != null) {
				onDone(www);
			}
		}

		#endregion LoadStreamingAsset

		#region LoadAsset

		internal WWW LoadAsset(string dir, string url)
		{
			string path = dir + url.GetHashCode().ToString();
			WWW www = new WWW(File.Exists(path) ? "file:///" + path : url);
			while (!www.isDone) {
				if (!string.IsNullOrEmpty(www.error)) {
					break;
				}
			}
			return www;
		}

		internal void LoadAssetAsync(string dir, string url, Action<WWW> onDone)
		{
			StartCoroutine(AsyncLoadAsset(dir, url, onDone));
		}

		IEnumerator AsyncLoadAsset(string dir, string url, Action<WWW> onDone)
		{
			string path = dir + url.GetHashCode().ToString();
			WWW www = new WWW(File.Exists(path) ? "file:///" + path : url);
			yield return www;
			if (onDone != null) {
				onDone(www);
			}
		}

		internal bool SaveAsset(string dir, string url, WWW www) {
			string path = dir + url.GetHashCode().ToString();
			if(! File.Exists(path)) {
				try {
					FileInfo info = new FileInfo(path);
					DirectoryInfo dirInfo = info.Directory;
					if (!dirInfo.Exists) {
						dirInfo.Create();
					}
					File.WriteAllBytes(info.FullName, www.bytes);
					return true;
				} catch (Exception e) {
					Debug.LogException(e);
					return false;
				}
			}
			return true;
		}

		internal bool RemoveAsset(string dir, string url)
		{
			string path = dir + url.GetHashCode().ToString();
			if (!File.Exists(path)) {
				return true;
			}
			try {
				File.Delete(path);
				return true;
			} catch {
				return false;
			}
		}

		#endregion LoadImage
	}
}
