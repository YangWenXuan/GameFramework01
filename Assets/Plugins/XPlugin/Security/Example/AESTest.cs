using UnityEngine;
using System.Collections;
using System.Diagnostics;
using XPlugin.Data.Json;
using XPlugin.Security;
using Debug = UnityEngine.Debug;

public class AESTest : MonoBehaviour {

	public string toEncry;

	public string toDecry;


	[ContextMenu("Encry")]
	void Encry() {
		Stopwatch sw = new Stopwatch();
		sw.Start();
		var text = AESUtil.Encrypt(toEncry, AESKey.DefaultKey, AESKey.DefaultIV);
		sw.Stop();
		Debug.Log(sw.ElapsedMilliseconds);
		Debug.Log(text);
		toDecry = text;
	}

	[ContextMenu("Decry")]
	void Decry() {

		Stopwatch sw = new Stopwatch();
		sw.Start();
		var text = AESUtil.Decrypt(toDecry, AESKey.DefaultKey, AESKey.DefaultIV);
		sw.Stop();
		Debug.Log(sw.ElapsedMilliseconds);

		Debug.Log(text);

		JObject.Parse(text);

	}
}
