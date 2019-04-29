using System;
using System.Collections.Generic;

namespace XPlugin.Data.Json
{
	public class JInvalidTypeException : JsonException
	{
		public JInvalidTypeException(JToken token, string method)
			: this(string.Format("Try to do {0}() on {1} token! (name=\"{2}\")", method, token.Type, token.Name))
		{
		}

		public JInvalidTypeException(object value, Type targetType)
			: this(string.Format("Can not cast from {0} to {1}!", (value != null ? value.GetType().ToString() : "null"), targetType))
		{
		}

		public JInvalidTypeException(string message)
			: base(message)
		{
		}
	}

	public class JIndexOutOfRangeException : JsonException
	{
		public JIndexOutOfRangeException(JArray array, int index)
			: this(string.Format("Index {0} out of array range! (count={1},name=\"{2}\")", index, array.Count, array.Name))
		{
		}

		public JIndexOutOfRangeException(string message)
			: base(message)
		{
		}
	}
}
