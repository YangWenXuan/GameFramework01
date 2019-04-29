using System.IO;
using UnityEngine;
using UnityEditor;

namespace XPlugin.Update {

	public class UpdateUtilityMenu {

		[MenuItem("XPlugin/Update/打开下载文件目录", false, 2)]
		static void OpenDownloadedFilesDir() {
			if (!Directory.Exists(ResManager.FullDownloadDir)) {
				Directory.CreateDirectory(ResManager.FullDownloadDir);
			}
			EditorUtility.RevealInFinder(ResManager.FullDownloadDir);
		}

		[MenuItem("XPlugin/Update/打开Bundle构建目录", false, 3)]
		static void OpenBuildABDir() {
			if (!Directory.Exists(BuildAssetBundle.FullAbBuildOutPutDir)) {
				Directory.CreateDirectory(BuildAssetBundle.FullAbBuildOutPutDir);
			}
			EditorUtility.RevealInFinder(BuildAssetBundle.FullAbBuildOutPutDir);
		}

	}
}