using System;
using UnityEngine;
using System.Collections;

public class WatchAttr : PropertyAttribute {

	public string[] ws;

	public WatchAttr(params string[] ws) {
		this.ws = ws;
	}


}
