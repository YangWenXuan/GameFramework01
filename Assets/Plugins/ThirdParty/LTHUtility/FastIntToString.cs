using System.Collections.Generic;

public static class FastIntToString {

	private static Dictionary<int, string> _dict;

	static FastIntToString() {
		_dict = new Dictionary<int, string>();
		int min = -100;
		int max = 100;
		for (int i = min; i <= max; i++) {
			_dict.Add(i, i.ToString());
		}
	}

	public static string ToStringFast(this int i) {
		string ret;
		if (_dict.TryGetValue(i, out ret)) {
			return ret;
		}
		return i.ToString();
	}
}
