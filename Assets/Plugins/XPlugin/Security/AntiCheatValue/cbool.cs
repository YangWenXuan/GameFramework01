//
// cbool.cs
//
// Author:
// [ChenJiasheng]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

namespace XPlugin.Security.AntiCheatValue {

	public struct cbool {
		/// <summary>
		/// 获取值（也可以直接用bool转换符）
		/// </summary>
		public bool Value {
			get {
				return this;
			}
		}

		#region 加解密

		private int value;
		private bool inited;

		public static cbool Encode (bool value) {
			return new cbool {
				value = value ? AntiCheat.Factor : ~AntiCheat.Factor,
					inited = true
			};
		}

		public static bool Decode (cbool value) {
			if (!value.inited) {
				return false;
			} else if (value.value == AntiCheat.Factor) {
				return true;
			} else if (value.value == ~AntiCheat.Factor) {
				return false;
			} else {
				AntiCheat.InvokeOnCheat ();
				return false;
			}
		}

		#endregion

		#region 类型转换符及一些运算符

		public static implicit operator cbool (bool value) {
			return Encode (value);
		}

		public static implicit operator bool (cbool obj) {
			return Decode (obj);
		}

		public static bool operator == (cbool lhs, cbool rhs) {
			return lhs.value == rhs.value;
		}

		public static bool operator != (cbool lhs, cbool rhs) {
			return lhs.value != rhs.value;
		}

		#endregion

		#region 重写一些从object派生的方法

		public bool Equals (bool obj) {
			return Decode (this) == obj;
		}

		public bool Equals (cbool obj) {
			return this == obj;
		}

		public override bool Equals (object obj) {
			if (!(obj is cbool))
				return false;

			return this == (cbool) obj;
		}

		public override string ToString () {
			return Decode (this).ToString ();
		}

		public override int GetHashCode () {
			return (bool) this ? 1 : 0;
		}

		#endregion
	}
}