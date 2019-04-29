using System.IO;

namespace XPlugin.UI {
    public static class UIUpdateUtil {
        /// <summary>
        /// 获取物体相对于Resource的路径
        /// </summary>
        public static string GetResourcePath(string path, bool withExt = false) {
            string dir = path;
            for (int i = 0; i < 100; i++) { //max search depth is 100
                dir = Path.GetDirectoryName(dir);
                if (dir == null) {
                    return null;
                }
                if (dir.EndsWith("Resources")) {
                    break;
                } else if (dir == Path.GetPathRoot(path)) {
                    return null;
                }
            }
            dir = dir.Replace("\\", "/");
            var ret = path.Replace(dir + "/", "");
            if (!withExt) {
                ret = ret.Remove(ret.LastIndexOf("."));
            }
            return ret;
        }
    }
}