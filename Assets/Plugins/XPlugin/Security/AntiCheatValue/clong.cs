//
// clong.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace XPlugin.Security.AntiCheatValue {

	public struct clong {
		/// <summary>
		/// 获取值（也可以直接用long转换符）
		/// </summary>
		public long Value {
			get {
				return this;
			}
		}

		#region 加解密

		private long value;
		private long check;
		private bool inited;

		public static clong Encode (long value) {
			long v = value;
			v ^= AntiCheat.Factor;
			v += AntiCheat.Factor;
			v = ~v;
			return new clong {
				value = v,
					check = AntiCheat.CalcCheck (value),
					inited = true
			};
		}

		public static long Decode (clong value) {
			if (value.inited) {
				long v = value.value;
				v = ~v;
				v -= AntiCheat.Factor;
				v ^= AntiCheat.Factor;
				if (value.check == AntiCheat.CalcCheck (v)) {
					return v;
				} else {
					AntiCheat.InvokeOnCheat ();
				}
			}
			return 0;
		}

		#endregion

		#region 类型转换符及一些运算符

		public static implicit operator clong (long value) {
			return Encode (value);
		}

		public static implicit operator long (clong obj) {
			return Decode (obj);
		}

		public static clong operator ++ (clong lhs) {
			return Decode (lhs) + 1;
		}

		public static clong operator -- (clong lhs) {
			return Decode (lhs) - 1;
		}

		public static bool operator == (clong lhs, clong rhs) {
			return lhs.value == rhs.value;
		}

		public static bool operator != (clong lhs, clong rhs) {
			return lhs.value != rhs.value;
		}

		#endregion

		#region 重写一些从object派生的方法

		public bool Equals (long obj) {
			return Decode (this) == obj;
		}

		public bool Equals (clong obj) {
			return this == obj;
		}

		public override bool Equals (object obj) {
			if (!(obj is clong))
				return false;

			return this == (clong) obj;
		}

		public override string ToString () {
			return Decode (this).ToString ();
		}

		public override int GetHashCode () {
			return Decode (this).GetHashCode ();
		}

		#endregion
	}
}