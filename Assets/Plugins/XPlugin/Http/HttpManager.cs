// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XPlugin.Http {

	public class HttpManager : MonoBehaviour {
		/// <summary>
		/// 超时错误
		/// </summary>
		public const string TIMEOUT_ERROR = "TimeOut";
		/// <summary>
		/// 默认超时时间
		/// </summary>
		public const float DEFAUT_TIMEOUT = 20f;

		private static HttpManager _ins;
		private static HttpManager ins {
			set { _ins = value; }
			get {
				if (_ins == null) {
					var go = new GameObject("HttpManager(Auto)");
					go.hideFlags = HideFlags.DontSave;
					DontDestroyOnLoad(go);
					_ins = go.AddComponent<HttpManager>();
				}
				return _ins;
			}
		}

		internal List<HttpRequest> _reqests = new List<HttpRequest>();

		void Awake() {
			ins = this;
		}

		void OnDestroy() {
			ins = null;
		}

		void Update() {
			float now = Time.realtimeSinceStartup;
			for (int i = 0; i < _reqests.Count;) {
				var req = _reqests[i];
				if (req.www.isDone) {
					_reqests.Remove(req);
					if (!string.IsNullOrEmpty(req.www.error)) {
						req.CallOnDone(req.www.error);
					} else {
						req.CallOnDone(null);
					}
				} else if (req.TimeOut > 0 && now - req.AddTime >= req.TimeOut) {
					req.CallOnDone(TIMEOUT_ERROR);
					_reqests.Remove(req);
				} else {
					i++;
				}
				req.CallOnProgress();
			}
		}

		/// <summary>
		/// 简易Post,(自动使用默认的超时时间)
		/// </summary>
		/// <param name="url"></param>
		/// <param name="form"></param>
		/// <param name="onResponse"></param>
		/// <param name="onProgress"></param>
		public static void SimplePost(string url, WWWForm form, OnHttpResponse onResponse, OnProgress onProgress = null) {
			Post(url, form, onResponse, onProgress, DEFAUT_TIMEOUT);
		}

		/// <summary>
		/// Post
		/// </summary>
		/// <param name="url"></param>
		/// <param name="form"></param>
		/// <param name="onResponse"></param>
		/// <param name="onProgress"></param>
		/// <param name="timeOut"></param>
		public static void Post(string url, WWWForm form, OnHttpResponse onResponse, OnProgress onProgress = null, float timeOut = -1) {
			ins._reqests.Add(new HttpRequest() {
				www = new WWW(url, form),
				onDone = onResponse,
				onProgress = onProgress,
				AddTime = Time.realtimeSinceStartup,
				TimeOut = timeOut,
			});
		}

		/// <summary>
		/// 简易GET，自动使用默认超时时间
		/// </summary>
		/// <param name="url"></param>
		/// <param name="onResponse"></param>
		/// <param name="onProgress"></param>
		public static void SimpleGet(string url, OnHttpResponse onResponse, OnProgress onProgress = null) {
			Get(url, onResponse, onProgress, DEFAUT_TIMEOUT);
		}

		/// <summary>
		/// GET
		/// </summary>
		/// <param name="url"></param>
		/// <param name="onResponse"></param>
		/// <param name="onProgress"></param>
		/// <param name="TimeOut"></param>
		public static void Get(string url, OnHttpResponse onResponse, OnProgress onProgress = null, float TimeOut = -1) {
			ins._reqests.Add(new HttpRequest() {
				www = new WWW(url),
				onDone = onResponse,
				onProgress = onProgress,
				AddTime = Time.realtimeSinceStartup,
				TimeOut = TimeOut
			});
		}

	}


}
