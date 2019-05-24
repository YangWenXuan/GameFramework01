using UnityEngine;
using System.Collections;

public class CheckResourceAttr : PropertyAttribute {
	public const string skillPath = "update/skill/";
	public const string effectPath = "update/effect/";
	public const string hudPath = "update/hud/";
	public const string heroPath = "update/hero/";

	public string path;
	public CheckResourceAttr(string path=""){
		this.path = path;
	}

}
