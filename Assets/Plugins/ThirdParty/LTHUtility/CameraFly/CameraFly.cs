// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CameraFly : MonoBehaviour {


	public float speed = 10f;
	public float boostSpeed = 20f;

	void Update() {
		float useSpeed = Input.GetKey(KeyCode.LeftShift) ? boostSpeed : speed;
		useSpeed *= Time.deltaTime;
		float v = Input.GetAxis("Vertical") * useSpeed;
		float h = Input.GetAxis("Horizontal") * useSpeed;

		transform.Translate(h, 0f, v, Space.Self);

	}
}
