//
// AntiCheat.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2018 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using System;

namespace XPlugin.Security.AntiCheatValue {

	public class AntiCheat {
		public static event Action OnCheat = delegate { };

		internal static int Factor;

		static AntiCheat () {
			Factor = new Random ().Next (1, 255);
		}

		internal static long CalcCheck (int value) {
			int i = 0;
			uint c = 0;
			uint p1 = 0;
			uint p2 = 0;
			while (value > 0) {
				i++;
				uint a = (uint) (value % 2);
				if (a > 0) {
					c++;
				}
				if (i % 2 == 1) {
					p1 += a << (i / 2);
				} else {
					p2 += a << (i / 2 - 1);
				}
				value >>= 1;
			}
			return ((long) c << 31) + (p1 << 15) + p2;
		}

		internal static long CalcCheck (long value) {
			int i = 0;
			uint p1 = 0;
			uint p2 = 0;
			while (value > 0) {
				i++;
				uint a = (uint) (value % 2);
				if (i % 2 == 1) {
					p1 += a << (i / 2);
				} else {
					p2 += a << (i / 2 - 1);
				}
				value >>= 1;
			}
			if (p1 == 0) {
				p1 = (uint) 1 << 31;
			}
			return (((long) p1 << 31) + p2);
		}

		public static void InvokeOnCheat () {
			OnCheat ();
		}
	}

}