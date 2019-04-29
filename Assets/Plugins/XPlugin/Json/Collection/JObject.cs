using System.Collections.Generic;

namespace XPlugin.Data.Json
{
	public class JObject : JCollection, IToJson
	{
		protected IDictionary<string, JToken> _dict;


		public static new JObject Parse(string json)
		{
			return JToken.Parse(json) as JObject;
		}

		public static JObject OptParse(string json)
		{
			var ret = JToken.OptParse(json) as JObject;
			if (ret == null) {
				ret = new JObject();
			}
			return ret;
		}

		public static JObject OptParse(string json, JObject def)
		{
			var ret = JToken.OptParse(json) as JObject;
			if (ret == null) {
				ret = def;
			}
			return ret;
		}

		public static explicit operator JObject(string json)
		{
			return Parse(json);
		}

		public JObject()
			: base(JType.Object)
		{
			_dict = new Dictionary<string, JToken>();
		}

		public override JToken this[int index]
		{
			get
			{
				return this[index.ToString()];
			}
			set
			{
				this[index.ToString()] = value;
			}
		}

		public override JToken this[string name]
		{
			get
			{
				JToken token;
				_dict.TryGetValue(name, out token);
				if (token == null) {
					token = new JNone(name);
				}
				return token;
			}
			set
			{
				if (value == null) {
					value = new JNull();
				}

				if (_dict.ContainsKey(name)) {
					_dict[name] = value;
				} else {
					_dict.Add(name, value);
				}
				value.Name = name;
			}
		}

		public override int Count
		{
			get
			{
				return _dict.Count;
			}
		}

		public void Add(string name, JToken value)
		{
			this[name] = value;
		}

		public void Remove(string name)
		{
			_dict.Remove(name);
		}

		public void Clear()
		{
			_dict.Clear();
		}

		public override IEnumerator<JToken> GetEnumerator()
		{
			return _dict.Values.GetEnumerator();
		}

		public IEnumerable<string> Keys
		{
			get
			{
				return _dict.Keys;
			}
		}

		public override JObject AsObject()
		{
			return this;
		}

		public override JObject GetObject()
		{
			return this;
		}

		public override JObject OptObject(JObject def)
		{
			return this;
		}

		public override void Write(JsonWriter writer)
		{
			writer.WriteObjectStart();

			foreach (JToken token in this) {
				writer.WritePropertyName(token.Name);
				token.Write(writer);
			}
			writer.WriteObjectEnd();
		}

		public JObject ToJson()
		{
			return this;
		}
	}
}