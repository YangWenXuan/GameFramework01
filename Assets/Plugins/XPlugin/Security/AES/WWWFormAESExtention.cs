using UnityEngine;
using System.Collections;

namespace XPlugin.Security {

	public static class WWWFormAESExtention {

		/// <summary>
		/// 向表单中添加一条字段，字段的value将通过AES默认密钥加密
		/// </summary>
		/// <param name="wwwForm"></param>
		/// <param name="field"></param>
		/// <param name="value"></param>
		public static void AddFieldAES(this WWWForm wwwForm, string field, string value) {
			wwwForm.AddField(field, AESUtil.Encrypt(value, AESKey.DefaultKey, AESKey.DefaultIV));
		}

		/// <summary>
		/// 向表单中添加一条字段，字段的key和value将通过AES默认密钥加密
		/// </summary>
		/// <param name="wwwForm"></param>
		/// <param name="field"></param>
		/// <param name="value"></param>
		public static void AddFieldAESAll(this WWWForm wwwForm, string field, string value) {
			wwwForm.AddField(AESUtil.Encrypt(field, AESKey.DefaultKey, AESKey.DefaultIV), AESUtil.Encrypt(value, AESKey.DefaultKey, AESKey.DefaultIV));
		}

	}
}
