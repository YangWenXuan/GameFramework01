using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SoulStoneClipEffect : BaseMeshEffect {

	private const float half = 2f;

	private const float sort = 1f / 4f;
	private const float longer = 3f / 4f;


	private List<UIVertex> list = new List<UIVertex>();

	public override void ModifyMesh(VertexHelper vh) {
		if (!this.IsActive()) {
			return;
		}
		list.Clear();
		vh.GetUIVertexStream(list);

		if (list.Count != 6) {
			Debug.LogError("这个组件只能用在simple的image上");
			return;
		}

		var color = list[0].color;
		Vector3 xy = list[0].position;
		Vector3 zw = list[2].position;
		Vector3 uvxy = list[0].uv0;
		Vector3 uvzw = list[2].uv0;

		Vector2 lb = new Vector2(xy.x, xy.y);//左下角坐标
		Vector2 wh = new Vector2(zw.x - xy.x, zw.y - xy.y);//宽高

		//		var v = new Vector4(xy.x, xy.y, zw.x, zw.y);
		var u = new Vector4(uvxy.x, uvxy.y, uvzw.x, uvzw.y);

		float halfW = u.z - u.x;
		halfW /= half;
		float halfH = u.w - u.y;
		halfH /= half;
		Vector2 origin = new Vector2(halfW + u.x, halfH + u.y);

		vh.Clear();

		vh.AddVert(new Vector3(lb.x, lb.y + wh.y * sort), color, origin + new Vector2(-halfW, -halfH / half));
		vh.AddVert(new Vector3(lb.x, lb.y + wh.y), color, origin + new Vector2(-halfW, halfH));
		vh.AddVert(new Vector3(lb.x + longer * wh.x, lb.y + wh.y), color, origin + new Vector2(halfW / half, halfH));
		vh.AddVert(new Vector3(lb.x + sort * wh.x, lb.y), color, origin + new Vector2(-halfW / half, -halfH));

		vh.AddVert(new Vector3(lb.x + longer * wh.x, lb.y + wh.y), color, origin + new Vector2(halfW / half, halfH));
		vh.AddVert(new Vector3(lb.x + wh.x, lb.y + longer * wh.y), color, origin + new Vector2(halfW, halfH / half));
		vh.AddVert(new Vector3(lb.x + wh.x, lb.y), color, origin + new Vector2(halfW, -halfH));
		vh.AddVert(new Vector3(lb.x + sort * wh.x, lb.y), color, origin + new Vector2(-halfW / half, -halfH));

		vh.AddTriangle(0, 1, 2);
		vh.AddTriangle(2, 3, 0);
		vh.AddTriangle(4, 5, 6);
		vh.AddTriangle(6, 7, 4);
	}


}
