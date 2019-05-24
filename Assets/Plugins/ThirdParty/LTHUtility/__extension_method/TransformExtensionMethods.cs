using System;
using UnityEngine;
using Object = UnityEngine.Object;

public static class TransformExtensionMethods {

	/// <summary>
	/// 递归重置Scale为1
	/// </summary>
	/// <param name="transform"></param>
	/// <returns></returns>
	public static Vector3 CounteractLocalScale(this Transform transform)
	{
		transform.localScale = CounteractLocalScaleRecursion(transform, Vector3.one);
		return transform.localScale;
	}

	private static Vector3 CounteractLocalScaleRecursion(Transform transform, Vector3 scale)
	{
		if (transform.parent != null)
		{
			scale = new Vector3(scale.x / transform.parent.localScale.x, scale.y / transform.parent.localScale.y, scale.z / transform.parent.localScale.z);
			return CounteractLocalScaleRecursion(transform.parent, scale);
		}
		return scale; 
	}

	/// <summary>
	/// 递归查找指定名称的物体
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	public static Transform FindInAllChild(this Transform transform, string name) {
		foreach (var v in transform.GetComponentsInChildren<Transform>()) {
			if (v.gameObject.name == name) {
				return v;
			}
		}
		return null;
	}

	/// <summary>
	/// 递归查找包含名称的物体
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	public static Transform FindInAllChildFuzzy(this Transform transform, string name) {
		foreach (var v in transform.GetComponentsInChildren<Transform>())
		{
			if (v.gameObject.name.IndexOf(name, StringComparison.Ordinal) >= 0)
			{
				return v;
			}
		}
		return null;
	}

	/// <summary>
	/// 查找物体，不存在则创建一个新的
	/// </summary>
	/// <param name="transform"></param>
	/// <param name="name"></param>
	/// <returns></returns>
	public static Transform FindOrCreate(this Transform transform, string name)
	{
		var trans = transform.Find(name);
		if (trans == null)
		{
			trans = new GameObject(name).transform;
			trans.SetParent(transform);
			trans.ResetLocal();
		}
		return trans;
	}

	public static void SetPositionX(this Transform transform, float x) {
		var position = transform.position;
		position = new Vector3(x, position.y, position.z);
		transform.position = position;
	}

	public static void SetPositionY(this Transform transform, float y) {
		var position = transform.position;
		position = new Vector3(position.x, y, position.z);
		transform.position = position;
	}

	public static void SetPositionZ(this Transform transform, float z) {
		var position = transform.position;
		position = new Vector3(position.x, position.y, z);
		transform.position = position;
	}

	public static void SetLocalPositionX(this Transform transform, float x) {
		var localPosition = transform.localPosition;
		localPosition = new Vector3(x, localPosition.y, localPosition.z);
		transform.localPosition = localPosition;
	}

	public static void SetLocalPositionY(this Transform transform, float y) {
		var localPosition = transform.localPosition;
		localPosition = new Vector3(localPosition.x, y, localPosition.z);
		transform.localPosition = localPosition;
	}

	public static void SetLocalPositionZ(this Transform transform, float z) {
		var localPosition = transform.localPosition;
		localPosition = new Vector3(localPosition.x, localPosition.y, z);
		transform.localPosition = localPosition;
	}

	public static void SetLocalScaleX(this Transform transform, float x) {
		var localScale = transform.localScale;
		localScale = new Vector3(x, localScale.y, localScale.z);
		transform.localScale = localScale;
	}

	public static void SetLocalScaleY(this Transform transform, float y) {
		var localScale = transform.localScale;
		localScale = new Vector3(localScale.x, y, localScale.z);
		transform.localScale = localScale;
	}

	public static void SetLocalScaleZ(this Transform transform, float z) {
		var localScale = transform.localScale;
		localScale = new Vector3(localScale.x, localScale.y, z);
		transform.localScale = localScale;
	}

	public static void SetLocalEulerAngleX(this Transform transform, float x) {
		var localEulerAngles = transform.localEulerAngles;
		localEulerAngles = new Vector3(x, localEulerAngles.y, localEulerAngles.z);
		transform.localEulerAngles = localEulerAngles;
	}

	public static void SetLocalEulerAngleY(this Transform transform, float y) {
		var localEulerAngles = transform.localEulerAngles;
		localEulerAngles = new Vector3(localEulerAngles.x, y, localEulerAngles.z);
		transform.localEulerAngles = localEulerAngles;
	}

	public static void SetLocalEulerAngleZ(this Transform transform, float z) {
		var localEulerAngles = transform.localEulerAngles;
		localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y, z);
		transform.localEulerAngles = localEulerAngles;
	}

	public static void SetEulerAngleX(this Transform transform, float x) {
		var eulerAngles = transform.eulerAngles;
		eulerAngles = new Vector3(x, eulerAngles.y, eulerAngles.z);
		transform.eulerAngles = eulerAngles;
	}

	public static void SetEulerAngleY(this Transform transform, float y) {
		var eulerAngles = transform.eulerAngles;
		eulerAngles = new Vector3(eulerAngles.x, y, eulerAngles.z);
		transform.eulerAngles = eulerAngles;
	}

	public static void SetEulerAngleZ(this Transform transform, float z) {
		var eulerAngles = transform.eulerAngles;
		eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, z);
		transform.eulerAngles = eulerAngles;
	}

	public static void SetUniformLocalScale(this Transform transform, float uniformScale) {
		transform.localScale = Vector3.one * uniformScale;
	}


	public static void ResetLocal(this Transform transform) {
		transform.localPosition=Vector3.zero;
		transform.localRotation=Quaternion.identity;
		transform.localScale=Vector3.one;
	}

	public static Vector2 ScreenPos(this Transform transform) {
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		return (new Vector2(screenPos.x, screenPos.y));
	}

	public static Vector2 ScreenPosRate(this Transform transform) {
		Vector2 screenPos = transform.ScreenPos();
		Vector2 rate = new Vector2(screenPos.x / (float)Screen.width, screenPos.y / (float)Screen.height);
		return rate;
	}

	public static void DestroyAllChildren(this Transform transform) {
		foreach (Transform t in transform) {
			Object.Destroy(t.gameObject);
		}
	}
}
