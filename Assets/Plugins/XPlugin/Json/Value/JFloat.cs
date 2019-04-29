using System;

namespace XPlugin.Data.Json
{
	public class JFloat : JValue
	{
		protected float value;

		public JFloat(float value)
			:base(JType.Number)
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
			JFloat other = obj as JFloat;

			if (other == null) {
				return false;
			}

			return value == other.value;
		}

		public override int GetHashCode()
		{
			return value.GetHashCode();
		}

		#region As

		public override int AsInt()
		{
			return (int) value;
		}

		public override long AsLong()
		{
			return (long) value;
		}

		public override float AsFloat()
		{
			return (float) value;
		}

		public override double AsDouble()
		{
			return (double) value;
		}

		#endregion


		#region Get

		public override int? GetInt()
		{
			return (int) value;
		}

		public override long? GetLong()
		{
			return (long) value;
		}

		public override float? GetFloat()
		{
			return (float) value;
		}

		public override double? GetDouble()
		{
			return (double) value;
		}

		#endregion


		#region Opt

		public override int OptInt(int def = default(int))
		{
			return (int) value;
		}

		public override long OptLong(long def = default(long))
		{
			return (long) value;
		}

		public override float OptFloat(float def = default(float))
		{
			return (float) value;
		}

		public override double OptDouble(double def = default(double))
		{
			return (double) value;
		}

		public override string OptString(string def)
		{
			return ToString();
		}

		#endregion
	}
}
