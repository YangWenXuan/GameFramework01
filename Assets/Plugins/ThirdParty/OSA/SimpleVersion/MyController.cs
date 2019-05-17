using System.Collections.Generic;
using UnityEngine;

namespace OSA {
    public class MyController : MonoBehaviour {
        public int count = 151;

        [ContextMenu("reload")]
        private void Start() {
            var data = new List<object>();
            for (int i = 0; i < count; ++i) {
                data.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
            }
            this.GetComponent<SimpleScroller>().SetDatas(data);
        }
    }
}