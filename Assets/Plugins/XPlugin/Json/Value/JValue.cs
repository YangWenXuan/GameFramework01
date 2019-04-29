using System;

namespace XPlugin.Data.Json
{
	public abstract class JValue : JToken
	{
		protected JValue(JType type)
			:base(type)
		{
		}

		public abstract object Value { get; }


		public override string ToString()
		{
			return Value.ToString();
		}

		public override object AsValue()
		{
			return Value;
		}

		public override object GetValue()
		{
			return Value;
		}

		public override object OptValue(object def)
		{
			return Value;
		}

		public static implicit operator JValue(bool value) { return new JBool(value); }
		public static implicit operator JValue(int value) { return new JInt(value); }
		public static implicit operator JValue(long value) { return new JLong(value); }
		public static implicit operator JValue(float value) { return new JFloat(value); }
		public static implicit operator JValue(double value) { return new JDouble(value); }
		public static implicit operator JValue(string value) { return new JString(value); }
	}
}
