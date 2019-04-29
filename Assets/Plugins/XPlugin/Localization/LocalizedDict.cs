// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections.Generic;
using XPlugin.Data.Json;
using XPlugin.Localization;

public class LocalizedDict {
    private static LocalizedDict _ins;

    public static LocalizedDict Ins {
        get {
            if (_ins == null) {
                _ins = new LocalizedDict();
            }
            return _ins;
        }
    }

    /// <summary>
    /// 字典(只读)
    /// </summary>
    public readonly Dictionary<string, string> Dict = new Dictionary<string, string>();


    /// <summary>
    /// 加载字典
    /// </summary>
    /// <param name="dictionaryPath"></param>
    public static void Load(string dictionaryPath) {
        var text = LResources.Load<TextAsset>(dictionaryPath);
        if (text == null) {
            Debug.LogError("没有找到当前语言对应的资源" + dictionaryPath);
            return;
        }
        //从JSON解析字典--(理解注释：解析JSON文件，并将解析的结果以键值对的形式存储到本类的Dict里面)
        var jDic = JObject.Parse(text.text);
        foreach (JToken kv in jDic) {
            string key = kv.Name;
            if (Ins.Dict.ContainsKey(key)) {
                Debug.LogError(string.Format("发现重复的key{0},将替换该key对应的值", key));
                Ins.Dict[key] = kv.AsString();
            } else {
                Ins.Dict.Add(key, kv.AsString());
            }
        }
    }

    /// <summary>
    /// 卸载
    /// </summary>
    /// <param name="language"></param>
    public static void UnLoad() {
        Ins.Dict.Clear();
    }

    /// <summary>
    /// 将字符串本地化，可以Format
    /// </summary>
    /// <param name="key"></param>
    /// <param name="args"></param>
    /// PS:拿着一个 key 在与本地化列表解析出来的Dictionary里面找对应的 value
    /// <returns></returns>
    public static string Localized(string key, params object[] args) {
        string result;
        if (Ins.Dict.TryGetValue(key, out result)) {
            if (args != null && args.Length != 0) {
                result = string.Format(result, args);
            }//参数用来填补占位符.
        }
        if (string.IsNullOrEmpty(result)) {
            Debug.LogError("没有找到相应的本地化字符串:" + key);
            return key;//如果没有找到就返回给的参数值.
        } else {
            return LocalizedUtil.RecoverNewLine(result);
        }
    }
}