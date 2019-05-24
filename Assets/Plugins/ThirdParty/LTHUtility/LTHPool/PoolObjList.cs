using System.Collections.Generic;
using UnityEngine;

namespace LTHUtility {


	public class PoolObjList {
		public HashSet<GameObject> Spawned;
		public HashSet<GameObject> Despawned;

		public PoolObjList() {
			this.Spawned = new HashSet<GameObject>();
			this.Despawned = new HashSet<GameObject>();
		}

		public void Clean(bool destroy) {
			if (destroy) {
				foreach (var gameObject in this.Spawned) {
					Object.Destroy(gameObject);
				}
				foreach (var gameObject in this.Despawned) {
					Object.Destroy(gameObject);
				}
			}
			this.Spawned.Clear();
			this.Despawned.Clear();
		}

		public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot) {
			GameObject toSpawn = null;
			while (true) {
				if (this.Despawned.Count > 0) {
					var ite = this.Despawned.GetEnumerator();
					toSpawn = ite.Current;
					ite.Dispose();
					this.Despawned.Remove(toSpawn);
					if (toSpawn == null) {
						Debug.LogError(string.Format("生成物体:{0}时出错，回收池中的一个实例为空，你是否手动的销毁了该物体", prefab.name));
						continue;
					}
					toSpawn.transform.position = pos;
					toSpawn.transform.rotation = rot;
					this.Spawned.Add(toSpawn);
					break;
				} else {
					toSpawn = Object.Instantiate(prefab, pos, rot) as GameObject;
					this.Spawned.Add(toSpawn);
					break;
				}
			}
			return toSpawn;
		}

		/// <summary>
		/// 回收物体
		/// </summary>
		/// <param name="ins">回收物体的实例</param>
		public void Despawn(GameObject ins) {
			this.Spawned.Remove(ins);
			this.Despawned.Add(ins);
			ins.SetActive(false);
		}

	}
}
