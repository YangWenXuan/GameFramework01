

namespace LTHUtility {
	[System.Serializable]
	public class RangeFloat {
		//>=(!!!!!!)
		public float min = 0f;
		//<
		public float max = 1f;

		public RangeFloat(float min, float max) {
			this.max = max;
			this.min = min;
		}


		public bool InsideRange(float value) {
			return (value >= min) && (value < max);
		}

		public float Random {
			get {
				return UnityEngine.Random.Range(min, max);
			}
		}

	}
}
