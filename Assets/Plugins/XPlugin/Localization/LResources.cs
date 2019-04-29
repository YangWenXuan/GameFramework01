// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using System.IO;
using XPlugin.Update;
using Object = UnityEngine.Object;

namespace XPlugin.Localization {

	/// <summary>
	/// 用于加载本地化资源的接口类
	/// </summary>
	public static class LResources {

		/// <summary>
		/// 请求一个文件
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static FileInfo ReqFile(string path) {
			//转化为本地化资源路径
			var localizationPath = Localization.ConvertAssetPath(path);
			FileInfo result = UResources.ReqFile(localizationPath);
			if (result == null) {
				//没有找到本地化资源,寻找非本地化资源
				result = UResources.ReqFile(path);
			}
			return result;
		}

		/// <summary>
		/// 加载物体
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Object Load(string path) {
			return Load(path, typeof(Object));
		}
		public static T Load<T>(string path) where T : Object {
			return (T)((object)Load(path, typeof(T)));
		}
		public static Object Load(string path, Type type) {
			var localizationPath = Localization.ConvertAssetPath(path);
			Object result = UResources.Load(localizationPath, type);
			if (result == null) {
				result = UResources.Load(path, type);
			}
			return result;
		}


		/// <summary>
		/// 异步加载物体
		/// </summary>
		/// <param name="path"></param>
		/// <param name="onDone"></param>
		public static void LoadAsync(string path, Action<Object> onDone) {
			LoadAsync(path, typeof(Object), onDone);
		}
		public static void LoadAsync<T>(string path, Action<Object> onDone) where T : Object {
			LoadAsync(path, typeof(T), onDone);
		}
		public static void LoadAsync(string path, Type type, Action<Object> onDone) {
			var localizationPath = Localization.ConvertAssetPath(path);
			UResources.LoadAsync(localizationPath, type, localizationResult => {
				if (localizationResult != null) {
					onDone(localizationResult);
				} else {
					UResources.LoadAsync(path, type, onDone);
				}
			});
		}


		/// <summary>
		/// 加载StreamingAssets物体（可更新）
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static WWW LoadStreamingAsset(string path) {
			var localizationPath = Localization.ConvertAssetPath(path);
			WWW result = UResources.LoadStreamingAsset(localizationPath);
			if (!string.IsNullOrEmpty(result.error)) {
				result = UResources.LoadStreamingAsset(path);
			}
			return result;
		}

		/// <summary>
		/// 异步加载StreamingAssets物体（可更新）
		/// </summary>
		/// <param name="path"></param>
		/// <param name="onDone"></param>
		public static void LoadStreamingAssetAsync(string path, Action<WWW> onDone) {
			var localizationPath = Localization.ConvertAssetPath(path);
			UResources.LoadStreamingAssetAsync(localizationPath, www => {
				if (string.IsNullOrEmpty(www.error)) {
					onDone(www);
				} else {
					UResources.LoadStreamingAssetAsync(localizationPath, onDone);
				}
			});
		}
	}
}