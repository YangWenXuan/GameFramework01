using System;
using System.Collections;
using System.Collections.Generic;

namespace XPlugin.Data.Json
{
	public abstract class JCollection : JToken, IEnumerable<JToken>
	{
		public JCollection(JType type) : base(type)
		{}

		public abstract IEnumerator<JToken> GetEnumerator();
		public abstract override int Count { get; }

		public override JToken Get(string path)
		{
			string[] split = path.Split('.');
			JToken current = this;
			for (int i = 0; i < split.Length; i++) {
				if (current is JCollection) {
					current = current[split[i]];
				} else {
					return new JNone(split[i]);
				}
			}
			return current;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override JCollection AsCollection()
		{
			return this;
		}

		public override JCollection GetCollection()
		{
			return this;
		}

		public override JCollection OptCollection(JCollection def)
		{
			return this;
		}
	}
}
