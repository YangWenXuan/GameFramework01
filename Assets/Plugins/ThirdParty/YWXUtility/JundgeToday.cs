using System;
using UnityEngine;
using XPlugin.Security;

namespace YWX {
    public class JundgeToday : MonoBehaviour
    {

        private static string LastSavedTime {
            get => PlayerPrefsAES.GetString("LastSaveTime", DateTime.MinValue.Date.ToString("yyyy-M-d"));
            set => PlayerPrefsAES.SetString("LastSaveTime", value);
        }
        
        public void JudgeTodayFun1() {
            if (DateTime.TryParse(LastSavedTime, out DateTime lastSavedTime)) {
                if (DateTime.Compare(DateTime.Today, lastSavedTime)<=0) {
                    
                    //这里可以做一些初始的操作,即未存初始日期之前的事情.
                    return;
                }
                LastSavedTime = DateTime.Today.ToString("yyyy-M-d");
                //TODO--每天只进行一次的活动.....
            }
        }

        /// <summary>
        /// 每日签到的新写法.
        /// </summary>
        public void JudgeTodayFun2() {
            if (DateTime.TryParse(LastSavedTime, out DateTime lastSavedTime)) {
                if (DateTime.Compare(DateTime.Today, lastSavedTime) < 0) {
                    //未存数据之前的操作:赋值初始值.
                    return;
                }else if (DateTime.Compare(DateTime.Today, lastSavedTime) == 0) {
                    //还在今天需要进行的一些操作.
                } else {
                    //到了第二天(或者两天后)的操作.
                }
            }
        }
    }
}
