using UnityEngine;
using System.Collections;
using System.Diagnostics;
using XPlugin.Security;
using Debug = UnityEngine.Debug;

public class DESTest : MonoBehaviour {
	public string key;
	public string iv;

	public string toEncry;

	public string toDecry;


	[ContextMenu("Encry")]
	void Encry() {
		Stopwatch sw = new Stopwatch();
		sw.Start();
        var text = DESUtil.Encrypt(toEncry, key, iv);
		sw.Stop();
		Debug.Log(sw.ElapsedMilliseconds);
		Debug.Log(text);
	}

	[ContextMenu("Decry")]
	void Decry() {
		Stopwatch sw = new Stopwatch();
		sw.Start();
        var text = DESUtil.Decrypt(toDecry, key, iv);
		sw.Stop();
		Debug.Log(sw.ElapsedMilliseconds);
		Debug.Log(text);
	}

}
