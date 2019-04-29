// Author:
// [LongTianhong]
//
// Copyright (C) 2014 Nanjing Xiaoxi Network Technology Co., Ltd. (http://www.mogoomobile.com)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ClipImageByMoveVert : BaseMeshEffect {

	public Vector2 LT;
	public Vector2 LB;
	public Vector2 RT;
	public Vector2 RB;

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

		list[5] = new UIVertex() {
			position	= list[5].position + new Vector3(LB.x, LB.y),
			color		= list[5].color,
			normal		= list[5].normal,
			tangent		= list[5].tangent,
			uv0			= list[5].uv0,
			uv1			= list[5].uv1,
		};

		list[0] = new UIVertex() {
			position	= list[0].position + new Vector3(LB.x, LB.y),
			color		= list[0].color,
			normal		= list[0].normal,
			tangent		= list[0].tangent,
			uv0			= list[0].uv0,
			uv1			= list[0].uv1,
		};

		list[1] = new UIVertex() {
			position	= list[1].position + new Vector3(LT.x, LT.y),
			color		= list[1].color,
			normal		= list[1].normal,
			tangent		= list[1].tangent,
			uv0			= list[1].uv0,
			uv1			= list[1].uv1,
		};


		list[2] = new UIVertex() {
			position	= list[2].position + new Vector3(RT.x, RT.y),
			color		= list[2].color,
			normal		= list[2].normal,
			tangent		= list[2].tangent,
			uv0			= list[2].uv0,
			uv1			= list[2].uv1,
		};

		list[3] = new UIVertex() {
			position	= list[3].position + new Vector3(RT.x, RT.y),
			color		= list[3].color,
			normal		= list[3].normal,
			tangent		= list[3].tangent,
			uv0			= list[3].uv0,
			uv1			= list[3].uv1,
		};

		list[4] = new UIVertex() {
			position	= list[4].position + new Vector3(RB.x, RB.y),
			color		= list[4].color,
			normal		= list[4].normal,
			tangent		= list[4].tangent,
			uv0			= list[4].uv0,
			uv1			= list[4].uv1,
		};




		vh.Clear();
		vh.AddUIVertexTriangleStream(list);

	}
}