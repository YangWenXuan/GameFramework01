using System;
using UnityEngine;
using System.Collections;
using System.Text;

namespace XPlugin.Security {

	public class AndroidSignature {

		/// <summary>
		/// 获取安卓包的签名信息
		/// </summary>
		/// <returns></returns>
		public String GetSignatureMd5() {
			try {
				// 获取Android的PackageManager    
				AndroidJavaClass Player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject Activity = Player.GetStatic<AndroidJavaObject>("currentActivity");
				AndroidJavaObject PackageManager = Activity.Call<AndroidJavaObject>("getPackageManager");

				// 获取当前Android应用的包名
				string packageName = Activity.Call<string>("getPackageName");

				// 调用PackageManager的getPackageInfo方法来获取签名信息数组    
				int GET_SIGNATURES = PackageManager.GetStatic<int>("GET_SIGNATURES");
				AndroidJavaObject PackageInfo = PackageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, GET_SIGNATURES);
				AndroidJavaObject[] Signatures = PackageInfo.Get<AndroidJavaObject[]>("signatures");

				// 获取当前的签名的哈希值，判断其与我们签名的哈希值是否一致
				if (Signatures != null && Signatures.Length > 0) {
					string ret = Signatures[0].Call<string>("toCharsString");
					ret = MD5Util.GetMD5(ret);
					return ret;
				}
			} catch (Exception e) {
				Debug.LogException(e);
			}
			return null;
		}

	}
}

