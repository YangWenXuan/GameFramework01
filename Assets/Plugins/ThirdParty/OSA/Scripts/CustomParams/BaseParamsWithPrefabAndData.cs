using System;
using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.CustomParams
{
	/// <summary>
	/// If you don't need full control over the list data by storing it manually, and none of the DataHelpers are appropriate for your use case, 
	/// you may derive your Params class from this. We encourage the use of the aforementioned methods, though, especially the DataHelpers, which
	/// were introduced in v4.1
	/// </summary>
	/// <typeparam name="TData">The model type to be used</typeparam>
	[System.Serializable]
	public class BaseParamsWithPrefabAndData<TData> : BaseParamsWithPrefab
	{
		[NonSerialized]
		public List<TData> Data = new List<TData>();
	}
}
