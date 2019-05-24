using System;
using UnityEngine;
using System.Collections;

public class AnimatorCheckEnd : MonoBehaviour {

	public Animator animator;

	public Action<AnimatorStateInfo, AnimationClip> OnAnimatorStateEnd = delegate { };

	private float _lastNormalizedTime = 0f;

	// Use this for initialization
	void Start() {
		if (animator == null) {
			animator = GetComponent<Animator>();
		}
	}

	// Update is called once per frame
	bool thisAnimAlreadyEnd;
	void Update() {
		var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		float normalizedTime = stateInfo.normalizedTime;
		if (normalizedTime % 1f >= 0.90f || ((int)normalizedTime - (int)_lastNormalizedTime) >= 1f) {
			//use normalizedTime to check if animtion is over
			if (!thisAnimAlreadyEnd) {
				//animation is not over,check if it is over
				thisAnimAlreadyEnd = true;
				OnAnimatorStateEnd(stateInfo, animator.GetCurrentAnimatorClipInfo(0)[0].clip);
			}
		} else {
			thisAnimAlreadyEnd = false;
		}
		_lastNormalizedTime = normalizedTime;
	}


}
