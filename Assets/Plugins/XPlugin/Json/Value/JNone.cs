using System;

namespace XPlugin.Data.Json
{
	public class JNone : JToken
	{
		public JNone()
			: base(JType.None)
		{
		}

		public JNone(string name)
			: this()
		{
			Name = name;
		}

		public override void Write(JsonWriter writer)
		{
			writer.Write(null);
		}

		public override bool Equals(object obj)
		{
			JNone other = obj as JNone;

			if (other == null) {
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override string ToString()
		{
			return "null";
		}
	}
}
