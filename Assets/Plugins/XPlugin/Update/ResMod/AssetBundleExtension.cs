// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace XPlugin.Update {

	internal static class AssetBundleExtension {
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9

		public static Object LoadAsset(this AssetBundle assetBundle, string name, Type type) {
			return assetBundle.Load(name, type);
		}

		public static AssetBundleRequest LoadAssetAsync(this AssetBundle assetBundle, string name, Type type) {
			return assetBundle.LoadAsync(name, type);
		}

#endif
	}
}