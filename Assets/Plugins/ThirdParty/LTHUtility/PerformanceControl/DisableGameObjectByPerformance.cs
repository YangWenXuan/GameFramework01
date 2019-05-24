#if PERFORMANCE_CONTROL

using UnityEngine;


public class DisableGameObjectByPerformance : MonoBehaviour {

	[Range(0, 100)]
	public int disableScore = 50;

	/// <summary>
	/// 当设备分数小于指定分数时关闭物体，否则，当设备分数大于指定分数时关闭物体
	/// </summary>
	public bool disableWhenSmaller = true;

	void OnEnable() {
		if (disableWhenSmaller) {
			if (DeviceLevel.Score < disableScore) {
				gameObject.SetActive(false);
			}
		} else {
			if (DeviceLevel.Score > disableScore) {
				gameObject.SetActive(false);
			}
		}
	}
}
#endif