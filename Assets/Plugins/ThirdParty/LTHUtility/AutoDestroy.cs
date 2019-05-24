using System.Collections;
using UnityEngine;

namespace LTHUtility {
    public class AutoDestroy : MonoBehaviour {
        public float delay=5;
        public bool ignoreTimeScale;

        private IEnumerator Start() {
            if (this.ignoreTimeScale) {
                yield return new WaitForSecondsRealtime(this.delay);
            } else {
                yield return new WaitForSeconds(this.delay);
            }
            Destroy(gameObject);
        }
    }
}