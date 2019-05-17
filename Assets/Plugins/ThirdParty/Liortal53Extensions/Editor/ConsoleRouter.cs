using System.IO;
using System.Text;
using UnityEngine;

public class ConsoleWriter : TextWriter {
	public override void Write(char value) {
	}

	public override void Write(string value) {
		Debug.Log(value);
	}

	public override Encoding Encoding
	{
		get { return Encoding.UTF8; }
	}
}

public class ErrorWriter : TextWriter {
	public override void Write(char value) {
	}

	public override void Write(string value) {
		Debug.LogError(value);
	}

	public override Encoding Encoding
	{
		get { return Encoding.UTF8; }
	}
}

//这个功能会输出一些额外的信息，根据自己的情况开启

//[InitializeOnLoad]
//public class ConsoleRouter {
//	static ConsoleRouter() {
//		System.Console.SetOut(new ConsoleWriter());
//		System.Console.SetError(new ErrorWriter());
//	}
//}