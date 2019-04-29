using System;

namespace XPlugin.Data.Json
{
	public class JString : JValue
	{
		protected string value;

		public JString(string value)
			:base(JType.String)
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
			JString other = obj as JString;

			if (other == null) {
				return false;
			}

			return value == other.value;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		#region As & Get

		public override string AsString()
		{
			return value;
		}

		public override string GetString()
		{
			return value;
		}

		#endregion


		#region Opt

		public override bool OptBool(bool def = default(bool))
		{
			bool v;
			if (value != null && bool.TryParse(value, out v)) {
				return v;
			}
			return def;
		}

		public override int OptInt(int def = default(int))
		{
			int v;
			if (value != null && int.TryParse(value, out v)) {
				return v;
			}
			return def;
		}

		public override long OptLong(long def = default(long))
		{
			long v;
			if (value != null && long.TryParse(value, out v)) {
				return v;
			}
			return def;
		}

		public override float OptFloat(float def = default(float))
		{
			float v;
			if (value != null && float.TryParse(value, out v)) {
				return v;
			}
			return def;
		}

		public override double OptDouble(double def = default(double))
		{
			double v;
			if (value != null && double.TryParse(value, out v)) {
				return v;
			}
			return def;
		}

		public override string OptString(string def)
		{
			return value;
		}

		#endregion
	}
}
