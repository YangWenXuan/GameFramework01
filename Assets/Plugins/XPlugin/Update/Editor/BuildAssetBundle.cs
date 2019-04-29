using System.IO;
using UnityEngine;
using UnityEditor;

namespace XPlugin.Update {

	public partial class BuildAssetBundle {


		private const string AB_BUILD_OUTPUT_DIR = "/../BuildAB/";

		/// <summary>
		/// 获取构建的AB输出目录,如果没有则会创建
		/// 格式为 dataPath+"../AB_BUILD_OUTPUT_DIR/PLATFORM/"
		/// </summary>
		public static string FullAbBuildOutPutDir {
			get {
				string ret = Application.dataPath + AB_BUILD_OUTPUT_DIR + EditorUserBuildSettings.activeBuildTarget + "/";
				if (!Directory.Exists(ret)) {
					Directory.CreateDirectory(ret);
				}
				return ret;
			}
		}

		private static string GetBundleName(Object obj) {
			var path = AssetDatabase.GetAssetPath(obj);
			string dir = path;
			for (int i = 0; i < 100; i++) {//max search depth is 10
				dir = Path.GetDirectoryName(dir);
				if (dir == null) {
					break;
				}
				if (dir.EndsWith("Resources")) {
					break;
				} else if (dir == Path.GetPathRoot(path)) {
					return Path.GetFileName(path);
				}
			}

			dir += "/";
			var fileName = path.Remove(0, dir.Length);
			fileName = fileName.Replace("/", ResManager.DIR_SPLIT);
			return fileName;
		}

		public static bool SimpleBuild(Object obj, bool compress) {
			AssetBundleBuild b1 = new AssetBundleBuild() {
				assetBundleName = GetBundleName(obj),
				assetNames = new string[] { AssetDatabase.GetAssetPath(obj) },
			};

			BuildAssetBundleOptions opt = compress
				? BuildAssetBundleOptions.None
				: BuildAssetBundleOptions.UncompressedAssetBundle;
			var manifest = BuildPipeline.BuildAssetBundles(FullAbBuildOutPutDir, new AssetBundleBuild[] { b1 }, opt, EditorUserBuildSettings.activeBuildTarget);
			if (manifest != null) {
				var abPath = FullAbBuildOutPutDir + b1.assetBundleName.ToLower();
				if (File.Exists(abPath)) {
					var truePath = FullAbBuildOutPutDir + Path.GetFileNameWithoutExtension(b1.assetBundleName);
					if (File.Exists(truePath)) {
						File.Delete(truePath);
					}
					File.Move(abPath, truePath);//Unity会自动转为小写，这里把它转回来，并且去除扩展名
					return true;
				}
			}
			return false;
		}
	}
}