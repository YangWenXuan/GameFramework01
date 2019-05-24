//
// DeviceLevel.cs
//
// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

#if PERFORMANCE_CONTROL
using UnityEngine;

public class DeviceLevel {
	private static int _score;

	public static int Score {
		get { return _score; }
		set {
			_score = value;
			if (_score < 50) {
				Shader.globalMaximumLOD = 500;
				QualitySettings.maximumLODLevel = 1;
			} else {
				Shader.globalMaximumLOD = 100000;
				QualitySettings.maximumLODLevel = 0;
			}
			Debug.Log("Set Device Performance Score: " + value);
		}
	}

	[RuntimeInitializeOnLoadMethod]
	public static void CalcDeviceScore() {
		int score = 100;
#if UNITY_EDITOR || UNITY_STANDALONE
		score = 100;
#elif UNITY_IPHONE
		var gen = UnityEngine.iOS.Device.generation;
		if (gen <= UnityEngine.iOS.DeviceGeneration.iPhone5C) {
			score = 20;
		} else if (gen <= UnityEngine.iOS.DeviceGeneration.iPadMini2Gen) {
			score = 60;
		} else {
			score = 90;
		}
#elif UNITY_ANDROID
		int processorCount = SystemInfo.processorCount;
		if (processorCount <= 4) {
			score = 0;
		} else {
			score = 80;
		}
#endif
		Score = score;
	}
}

#endif
