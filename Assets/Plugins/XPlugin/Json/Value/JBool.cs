using System;

namespace XPlugin.Data.Json
{
	public class JBool : JValue
	{
		protected bool value;

		public JBool(bool value)
			: base(JType.Bool)
		{
			this.value = value;
		}

		public override object Value
		{
			get
			{
				return value;
			}
		}

		public override void Write(JsonWriter writer)
		{
			writer.Write(value);
		}

		public override bool Equals(object obj)
		{
			JBool other = obj as JBool;

			if (other == null) {
				return false;
			}

			return value == other.value;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		public override bool AsBool()
		{
			return value;
		}

		public override bool? GetBool()
		{
			return value;
		}

		public override bool OptBool(bool def = default(bool))
		{
			return value;
		}

		public override string OptString(string def)
		{
			return value.ToString();
		}
	}
}
