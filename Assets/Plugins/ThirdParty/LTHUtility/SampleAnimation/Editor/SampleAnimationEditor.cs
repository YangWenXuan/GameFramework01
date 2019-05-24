using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SampleAnimation))]
public class SampleAnimationEditor : Editor {

	private AnimationClip clip;
	private float time;
	private SampleAnimation tar;
	void OnEnable() {
		tar = (SampleAnimation)target;
	}

	public override void OnInspectorGUI() {
		EditorGUI.BeginChangeCheck();
		clip = (AnimationClip)EditorGUILayout.ObjectField(clip, typeof(AnimationClip), false);
		if (clip != null) {
			GUILayout.Label("time:" + time + "/" + clip.length);
			time = GUILayout.HorizontalSlider(time, 0f, clip.length);
		}
		if (EditorGUI.EndChangeCheck()) {
			if (tar.gameObject != null && clip != null) {
				clip.SampleAnimation(tar.gameObject, time);
			}
		}
	}
}