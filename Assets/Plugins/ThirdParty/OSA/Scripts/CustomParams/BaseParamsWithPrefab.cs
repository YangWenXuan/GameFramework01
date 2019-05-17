using UnityEngine;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.CustomParams
{
	/// <summary>
	/// Custom params containing a single prefab. <see cref="ItemPrefabSize"/> is calculated on first accessing and invalidated each time <see cref="InitIfNeeded(IOSA)"/> is called.
	/// </summary>
	[System.Serializable]
	public class BaseParamsWithPrefab : BaseParams
	{
		public RectTransform itemPrefab;

		public float ItemPrefabSize
		{
			get
			{
				if (!itemPrefab)
					throw new System.InvalidOperationException("OSA: " + typeof(BaseParamsWithPrefab) + ": the prefab was not set. Please set it through inspector or in code");

				if (_PrefabSize == -1f)
					_PrefabSize = IsHorizontal ? itemPrefab.rect.width : itemPrefab.rect.height;

				return _PrefabSize;
			}
		}

		float _PrefabSize = -1f;

		/// <inheritdoc/>
		public override void InitIfNeeded(IOSA iAdapter)
		{
			base.InitIfNeeded(iAdapter);

			_PrefabSize = -1f; // so the prefab's size will be recalculated

			DefaultItemSize = ItemPrefabSize;
		}
	}
}
