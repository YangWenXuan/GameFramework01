using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RawImagePot : RawImage {

	public Vector2 OriginSize=new Vector2(100,100);

	public void SetOriginSize() {
		this.rectTransform.anchorMax = this.rectTransform.anchorMin;
		this.rectTransform.sizeDelta = this.OriginSize;
	}
}
