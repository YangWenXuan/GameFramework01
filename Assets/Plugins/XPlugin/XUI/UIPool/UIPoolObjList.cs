using System.Collections.Generic;
using UnityEngine;

namespace XUI {


	public class UIPoolObjList {
		private List<GameObject> _spawned;
		private List<GameObject> _despawned;

		public UIPoolObjList() {
			_spawned = new List<GameObject>();
			_despawned = new List<GameObject>();
		}

		public void Clean(bool destroy) {
			if (destroy) {
				foreach (var gameObject in _spawned) {
					Object.Destroy(gameObject);
				}
				foreach (var gameObject in _despawned) {
					Object.Destroy(gameObject);
				}
			}
			_spawned.Clear();
			_despawned.Clear();
		}

		public GameObject Spawn(GameObject prefab,Transform parent) {
			GameObject toSpawn = null;
			while (true) {
				if (_despawned.Count > 0) {
					toSpawn = _despawned[0];
					_despawned.RemoveAt(0);
					if (toSpawn == null) {
						Debug.LogError(string.Format("生成物体:{0}时出错，回收池中的一个实例为空，你是否手动的销毁了该物体", prefab.name));
						continue;
					}
					//					toSpawn.transform.position = pos;
					//					toSpawn.transform.rotation = rot;
					_spawned.Add(toSpawn);
					break;
				} else {
					toSpawn = Object.Instantiate(prefab,parent,false);
					_spawned.Add(toSpawn);
					break;
				}
			}
			return toSpawn;
		}

		/// <summary>
		/// 回收物体
		/// </summary>
		/// <param name="ins">回收物体的实例</param>
		public void Despawn(GameObject ins, bool destroy) {
			_spawned.Remove(ins);
			if (!destroy) {
				_despawned.Add(ins);
			}
		}

	}
}
