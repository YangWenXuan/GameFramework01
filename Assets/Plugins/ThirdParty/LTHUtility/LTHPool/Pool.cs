using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LTHUtility {
    public class Pool<T> : Pool where T : Pool<T> {
        protected static T _instance;

        public static T Ins {
            get { return _instance; }
        }

        protected override void Awake() {
            _instance = (T) this;
            base.Awake();
        }

        protected virtual void OnDestroy() {
            _instance = null;
        }
    }

    public class Pool : MonoBehaviour {
        [SerializeField] [Tooltip("生成时是否作为生成物体的父物体")]
        private bool _asParentWhenSpawn = true;

        [SerializeField] [Tooltip("销毁后是否作为生成物体的父物体")]
        private bool _asParentAfterDespawn = true;

        private Dictionary<GameObject, PoolObjList> _prefabToList;
        private Dictionary<GameObject, PoolObjList> _insToList;

        protected virtual void Awake() {
            _prefabToList = new Dictionary<GameObject, PoolObjList>();
            _insToList = new Dictionary<GameObject, PoolObjList>();
        }

        /// <summary>
        /// 清理对象池
        /// </summary>
        /// <param name="destroy">是否销毁对象池中的物体</param>
        public void Clean(bool destroy) {
            foreach (var list in _prefabToList.Values) {
                list.Clean(destroy);
            }
            _prefabToList.Clear();
            _insToList.Clear();
        }

        /// <summary>
        /// 产生物体
        /// 试图从对象池中获取一个物体，如果没有则实例化一个物体
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab) {
            return Spawn(prefab, Vector3.zero);
        }

        /// <summary>
        /// 产生物体
        /// 试图从对象池中获取一个物体，如果没有则实例化一个物体
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab, Vector3 pos) {
            return Spawn(prefab, pos, Quaternion.identity);
        }

        /// <summary>
        /// 产生物体
        /// 试图从对象池中获取一个物体，如果没有则实例化一个物体
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot) {
            PoolObjList list;
            if (!_prefabToList.TryGetValue(prefab, out list)) {
                list = new PoolObjList();
                _prefabToList.Add(prefab, list);
            }
            var ins = list.Spawn(prefab, pos, rot);
            _insToList.Add(ins, list);
            if (_asParentWhenSpawn) {
                ins.transform.parent = transform;
            }
            ins.SetActive(true);
            return ins;
        }

        /// <summary>
        /// 回收物体
        /// </summary>
        /// <param name="ins">回收物体的实例</param>
        public void Despawn(GameObject ins) {
            if (ins == null) {
                throw new ArgumentNullException("ins" + "回收实例不能为空");
            }
            PoolObjList list;
            if (!_insToList.TryGetValue(ins, out list)) {
                Debug.LogError(ins + " 没有找到回收实例对应的列表,将会直接销毁该物体");
                Destroy(ins);
                return;
            }
            list.Despawn(ins);
            _insToList.Remove(ins);
            if (_asParentAfterDespawn) {
                ins.transform.parent = transform;
            }
        }

        /// <summary>
        /// 回收通过一个prefab创建的所有实例
        /// </summary>
        /// <param name="prefab"></param>
        public void DespawnAll(GameObject prefab) {
            PoolObjList list;
            if (!this._prefabToList.TryGetValue(prefab, out list)) {
                throw new ArgumentNullException("没有找到prefab对应的实例池" + prefab);
            }
            var copy= list.Spawned.ToArray();
            foreach (var ins in copy) {
                Despawn(ins);
            }
        }
    }
}