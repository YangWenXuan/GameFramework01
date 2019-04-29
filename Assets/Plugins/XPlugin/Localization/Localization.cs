// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


using XPlugin.Data.Json;
using XPlugin.Update;

namespace XPlugin.Localization {
    public delegate void LanguageChange(LanguageEnum old, LanguageEnum newer);

    public static class Localization {
        public static Dictionary<LanguageEnum, string> LanguagePath;

        static Localization() {
            //初始化语言路径
            LanguagePath = new Dictionary<LanguageEnum, string>();
            foreach (LanguageEnum name in Enum.GetValues(typeof(LanguageEnum))) {
                LanguagePath.Add(name,name+"/");
            }
            LanguagePath[LanguageEnum.unkonwn] = "";
        }

        public static LanguageChange OnLanguageChanged;

        private static LanguageEnum _language = LanguageEnum.unkonwn;

        /// <summary>
        /// 当前语言
        /// </summary>
        public static LanguageEnum Language {
            get { return _language; }
            set {
                Debug.Log("设置语言:" + value);
                var oldLanguage = _language;
                _language = value;
                OnLanguageChanged?.Invoke(oldLanguage, value);
                //if (OnLanguageChanged!=null)
                //{
                //    OnLanguageChanged(oldLanguage, value);
                //}
            }
        }

        /// <summary>
        /// 当前语言资源路径
        /// </summary>
        public static string CurrentPath => LanguagePath[Language];

        public static LanguageEnum SystemLang {
            get {
                LanguageEnum lang=LanguageEnum.unkonwn;
                //预置语言
                var www = UResources.LoadStreamingAsset("language.txt"); 
                if (string.IsNullOrEmpty(www.error)) {
                    var txt = www.text;
                    lang = (LanguageEnum) Enum.Parse(typeof(LanguageEnum), txt);
                    Debug.Log("使用StreamingAsset中的语言:" + www.text + " => " + lang);
                } else {
                    var systemLanguage = Application.systemLanguage;
                    switch (systemLanguage) {
                        case SystemLanguage.Chinese:
                        case SystemLanguage.ChineseSimplified:
                            lang = LanguageEnum.zh;
                            break;
                        case SystemLanguage.ChineseTraditional:
                            lang = LanguageEnum.zht;
                            break;
                        case SystemLanguage.English:
                            lang = LanguageEnum.en;
                            break;
                        case SystemLanguage.French:
                            lang = LanguageEnum.fr;
                            break;
                        case SystemLanguage.Vietnamese:
                            lang = LanguageEnum.vi;
                            break;
                    }
                    Debug.Log("没有找到StreamingAsset中的语言,使用系统语言:" + systemLanguage + " => " + lang);
                }
                return lang;
            }
        }

        public static void Init(LanguageEnum lang) {
            Language = lang;
        }

        /// <summary>
        /// 将资源路径转换为本地化资源路径
        /// 例如简体中文下asset变为zh-CN/asset
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ConvertAssetPath(string path) {
            return CurrentPath + path;
        }
    }
}