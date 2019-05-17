using System;
using UnityEngine;
using UnityEngine.EventSystems;
using frame8.Logic.Misc.Visual.UI;

namespace Com.TheFallenGames.OSA.Core
{
	/// <summary>
	/// Contains commonly used members so that an <see cref="OSA{TParams, TItemViewsHolder}"/> instance 
	/// can be referenced abstractly (since instances of derived generic classes cannot be referenced by a variable of base type).
	/// </summary>
	/// <seealso cref="IScrollRectProxy"/>
	public interface IOSA : IScrollRectProxy, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
	{
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.Initialized"/></summary>
		event Action Initialized;

		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.ItemsRefreshed"/></summary>
		event Action<int, int> ItemsRefreshed;

		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.BaseParameters"/></summary>
		BaseParams BaseParameters { get; }
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.AsMonoBehaviour"/></summary>
		MonoBehaviour AsMonoBehaviour { get; }
		[Obsolete("Use IScrollRectProxy.GetContentSizeToViewportRatio() (extension method)", true)]
		double ContentVirtualSizeToViewportRatio { get; }
		double ContentVirtualInsetFromViewportStart { get; }
		double ContentVirtualInsetFromViewportEnd { get; }
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.VisibleItemsCount"/></summary>
		int VisibleItemsCount { get; }
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.RecyclableItemsCount"/></summary>
		int RecyclableItemsCount { get; }
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.IsDragging"/></summary>
		bool IsDragging { get; }
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.InsertAtIndexSupported"/></summary>
		bool InsertAtIndexSupported { get; }
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.RemoveFromIndexSupported"/></summary>
		bool RemoveFromIndexSupported { get; }

		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.ChangeItemsCount(ItemCountChangeMode, int, int, bool, bool)"/></summary>
		void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfAppendingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false);
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.Refresh(bool, bool)"/></summary>
		void Refresh(bool contentPanelEndEdgeStationary = false, bool keepVelocity = false);
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.ResetItems(int, bool, bool)"/></summary>
		void ResetItems(int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false);
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.InsertItems(int, int, bool, bool)"/></summary>
		void InsertItems(int index, int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false);
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.RemoveItems(int, int, bool, bool)"/></summary>
		void RemoveItems(int index, int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false);
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.SetVirtualAbstractNormalizedScrollPosition(double, bool, out bool, bool)"/></summary>
		double SetVirtualAbstractNormalizedScrollPosition(double pos, bool computeVisibilityNow, out bool looped, bool keepVelocity = false);
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.GetItemsCount"/></summary>
		int GetItemsCount();
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.ScrollTo(int, float, float)"/></summary>
		void ScrollTo(int itemIndex, float normalizedOffsetFromViewportStart = 0f, float normalizedPositionOfItemPivotToUse = 0f);
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.SmoothScrollTo(int, float, float, float, Func{float, bool}, bool)"/></summary>
		bool SmoothScrollTo(int itemIndex, float duration, float normalizedOffsetFromViewportStart = 0f, float normalizedPositionOfItemPivotToUse = 0f, Func<float, bool> onProgress = null, bool overrideCurrentScrollingAnimation = false);
		/// <summary>See <see cref="OSA{TParams, TItemViewsHolder}.GetViewsHolderClosestToViewportLongitudinalNormalizedAbstractPoint(Canvas, RectTransform, float, float, out float)"/></summary>
		AbstractViewsHolder GetViewsHolderClosestToViewportLongitudinalNormalizedAbstractPoint(Canvas canvas, RectTransform canvasRectTransform, float viewportPoint01, float itemPoint01, out float distance);
	}
}
