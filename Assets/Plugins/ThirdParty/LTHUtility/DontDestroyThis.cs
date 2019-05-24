//
// DontDestroyThis.cs
//
// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;

namespace Game
{
		public class DontDestroyThis : MonoBehaviour
		{

				void Awake ()
				{
						GameObject.DontDestroyOnLoad (gameObject);
				}

		}
}
