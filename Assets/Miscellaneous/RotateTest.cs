using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTest : MonoBehaviour {
    public Transform axisPoint;
    private float _speed = 60;
    public Vector3 axis;

    private void Update() {
        transform.RotateAround(this.axisPoint.position,this.axis,this._speed*Time.deltaTime);
    }
}
