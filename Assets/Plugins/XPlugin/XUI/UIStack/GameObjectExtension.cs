using UnityEngine;

public static class GameObjectExtension {

	public static T GetOrAddComponent<T>(this GameObject g) where T : Component {
		T ret = g.GetComponent<T>();
		if (ret == null) {
			ret = g.AddComponent<T>();
		}
		return ret;
	}
}
