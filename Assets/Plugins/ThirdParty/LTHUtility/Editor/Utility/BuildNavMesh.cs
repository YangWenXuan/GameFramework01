using UnityEngine;
using System.Collections;
using UnityEditor;

public class BuildNavMesh : MonoBehaviour {

//	[MenuItem("LTH/Utility/Build Nav Mesh")]
//	static void Build(){
//		GameObject g=new GameObject("navMesh");
//		MeshFilter mf=g.AddComponent<MeshFilter>();
//		NavMeshTriangulation triangles = NavMesh.CalculateTriangulation();
//		Mesh mesh = new Mesh();
//		mesh.vertices = triangles.vertices;
//		mesh.triangles = triangles.indices;
//		Vector2[] uvs= new Vector2[mesh.vertices.Length];
//		for(int i=0;i<uvs.Length;i++){
//			uvs[i]=new Vector2(0f,0f);
//		}
//		mesh.uv=uvs;
//		mesh.RecalculateNormals();
//
//		mf.mesh=mesh;
////		MeshRenderer mr= g.AddComponent<MeshRenderer>();
//
//	}
}
