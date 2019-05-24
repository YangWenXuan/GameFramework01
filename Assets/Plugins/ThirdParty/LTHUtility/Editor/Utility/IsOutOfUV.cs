using UnityEngine;
using System.Collections;
using UnityEditor;

public class IsOutOfUV : MonoBehaviour {

		[MenuItem("LTH/Utility/检查UV超界")]
		static void checkOutOfUV(){
				GameObject selected = Selection.GetTransforms (SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable) [0].gameObject;
				bool r=hasOutOfBoundsUVs (selected.GetComponent <MeshFilter>().sharedMesh);
				Debug.Log (selected+" : "+ r);
		}

		public static bool hasOutOfBoundsUVs(Mesh m, int submeshIndex = -1){
				Vector2[] uvs = m.uv;
				if (uvs.Length == 0) return false;
				float minx,miny,maxx,maxy;
				if (submeshIndex >= m.subMeshCount) return false;
				if (submeshIndex >= 0){
						int[] tris = m.GetTriangles(submeshIndex);
						if (tris.Length == 0) return false;
						minx = maxx = uvs[tris[0]].x;
						miny = maxy = uvs[tris[0]].y;
						for (int idx = 0; idx < tris.Length; idx++){
								int i = tris[idx];
								if (uvs[i].x < minx) minx = uvs[i].x;
								if (uvs[i].x > maxx) maxx = uvs[i].x;
								if (uvs[i].y < miny) miny = uvs[i].y;
								if (uvs[i].y > maxy) maxy = uvs[i].y;
						}			
				} else {
						minx = maxx = uvs[0].x;
						miny = maxy = uvs[0].y;
						for (int i = 0; i < uvs.Length; i++){
								if (uvs[i].x < minx) minx = uvs[i].x;
								if (uvs[i].x > maxx) maxx = uvs[i].x;
								if (uvs[i].y < miny) miny = uvs[i].y;
								if (uvs[i].y > maxy) maxy = uvs[i].y;
						}
				} 
//				uvBounds.x = minx;
//				uvBounds.y = miny;
//				uvBounds.width = maxx - minx;
//				uvBounds.height = maxy - miny;
				if (maxx > 1f || minx < 0f || maxy > 1f || miny < 0f){
						return true;
				}
				//all well behaved objs use the same rect so TexSets compare properly
//				uvBounds.x = uvBounds.y = 0f;
//				uvBounds.width = uvBounds.height = 1f;
				return false;
		}
}
