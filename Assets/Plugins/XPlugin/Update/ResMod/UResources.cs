using System;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace XPlugin.Update
{
	public static class UResources
	{

		/// <summary>
		/// 请求一个文件
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static FileInfo ReqFile(string path)
		{
			if (ResManager.Ins.DownloadedFiles.ContainsKey(path)) {
				return ResManager.Ins.DownloadedFiles[path];
			}
			return null;
		}

		/// <summary>
		/// 请求场景
		/// </summary>
		/// <param name="path"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static bool ReqScene(string path)
		{
			return ResManager.Ins.ReqScene(path);
		}

		/// <summary>
		/// 异步请求场景
		/// </summary>
		/// <param name="path"></param>
		/// <param name="mode"></param>
		/// <param name="onDone"></param>
		public static void ReqSceneAsync(string path, Action<bool> onDone)
		{
			ResManager.Ins.ReqSceneAsync(path, onDone);
		}

		/// <summary>
		/// 加载物体
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Object Load(string path)
		{
			return UResources.Load(path, typeof(Object));
		}
		public static T Load<T>(string path) where T : Object
		{
			return (T)((object)UResources.Load(path, typeof(T)));
		}
		public static Object Load(string path, Type type)
		{
			return ResManager.Ins.Load(path, type);
		}


		/// <summary>
		/// 异步加载物体
		/// </summary>
		/// <param name="path"></param>
		/// <param name="onDone"></param>
		public static void LoadAsync(string path, Action<Object> onDone)
		{
			LoadAsync(path, typeof(Object), onDone);
		}
		public static void LoadAsync<T>(string path, Action<Object> onDone) where T : Object
		{
			LoadAsync(path, typeof(T), onDone);
		}
		public static void LoadAsync(string path, Type type, Action<Object> onDone)
		{
			ResManager.Ins.LoadAsync(path, type, onDone);
		}


		/// <summary>
		/// 加载StreamingAssets物体（可更新）
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static WWW LoadStreamingAsset(string path)
		{
			return ResManager.Ins.LoadStreamingAsset(path);
		}

		/// <summary>
		/// 异步加载StreamingAssets物体（可更新）
		/// </summary>
		/// <param name="path"></param>
		/// <param name="onDone"></param>
		public static void LoadStreamingAssetAsync(string path, Action<WWW> onDone)
		{
			ResManager.Ins.LoadStreamingAssetAsync(path, onDone);
		}

		/// <summary>
		/// 加载资源
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="dir">Dir.</param>
		/// <param name="url">URL.</param>
		public static WWW LoadAsset(string dir, string url)
		{
			return ResManager.Ins.LoadAsset(dir, url);
		}

		/// <summary>
		/// 异步加载资源
		/// </summary>
		/// <returns>The asset.</returns>
		/// <param name="dir">Dir.</param>
		/// <param name="url">URL.</param>
		/// <param name="onDone"></param>
		public static void LoadAssetAsync(string dir, string url, Action<WWW> onDone)
		{
			ResManager.Ins.LoadAssetAsync(dir, url, onDone);
		}

		/// <summary>
		/// 保存资源
		/// </summary>
		/// <returns><c>true</c>, if image asset was saved, <c>false</c> otherwise.</returns>
		/// <param name="dir">Dir.</param>
		/// <param name="url">URL.</param>
		/// <param name="img">Image.</param>
		public static bool SaveAsset(string dir, string url, WWW www)
		{
			return ResManager.Ins.SaveAsset(dir, url, www);
		}

		/// <summary>
		/// 移除资源
		/// </summary>
		/// <returns><c>true</c>, if asset was removed, <c>false</c> otherwise.</returns>
		/// <param name="dir">Dir.</param>
		/// <param name="url">URL.</param>
		public static bool RemoveAsset(string dir, string url)
		{
			return ResManager.Ins.RemoveAsset(dir, url);
		}
	}
}
