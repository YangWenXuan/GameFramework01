using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.Demos.Common
{
	public class OSATitle : MonoBehaviour
	{
		protected void Start() { GetComponent<Text>().text = "Optimized ScrollView Adapter v" + C.OSA_VERSION_STRING; }
	}
}