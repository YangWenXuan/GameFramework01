using UnityEngine;
using System.Collections;

public class RenameAttr : PropertyAttribute {
	public readonly string showName;
	public readonly Color color;

	public RenameAttr(string showName) {
		this.showName = showName;
		this.color = Color.white;
	}

	public RenameAttr(string showName,Color color) {
		this.showName = showName;
		this.color = color;
	}
}
