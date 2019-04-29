using System;

namespace XPlugin.Data.Json
{
	public class JNull : JToken
	{
		public JNull()
			:base(JType.Null)
		{
		}

		public JNull(string name)
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
			JNull other = obj as JNull;

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
