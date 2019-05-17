using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using frame8.Logic.Misc.Visual.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.Util.ItemDragging;
using Com.TheFallenGames.OSA.DataHelpers;

namespace Com.TheFallenGames.OSA.Demos.ItemDragging
{
	/// <summary>
	/// </summary>
	public class ItemDraggingExample : OSA<MyParams, MyViewsHolder>, DraggableItem.IDragDropListener, ICancelHandler
	{
		public SimpleDataHelper<MyModel> Data { get; private set; }

		DragStateManager _DragManager = new DragStateManager();
		Canvas _Canvas;
		RectTransform _CanvasRT;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<MyModel>(this);

			base.Start();

			_Canvas = GetComponentInParent<Canvas>();
			_CanvasRT = _Canvas.transform as RectTransform;
		}

		/// <inheritdoc/>
		protected override void Update()
		{
			base.Update();

			if (_DragManager.State == DragState.DRAGGING)
			{
				// Can't scroll
				if (this.GetContentSizeToViewportRatio() < 1f)
					return;

				Vector2 localPointInViewport;
				if (!GetLocalPointInViewportIfWithinBounds(
						_DragManager.Dragged.draggableComponent.CurrentOnDragEventWorldPosition,
						_DragManager.Dragged.draggableComponent.CurrentPressEventCamera,
						out localPointInViewport
					)
				)
					return;

				// 0=start(top, in our case), 1=end(bottom)
				float abstrPoint01 = ConvertViewportLocalPointToViewportLongitudinalPointStart0End1(localPointInViewport);

				//Debug.Log(localPointInViewport + ", " + abstrPoint01.ToString("####.####"));

				abstrPoint01 = Mathf.Clamp01(abstrPoint01);
				float scrollAbstrDeltaInCTSpace = _Params.maxScrollSpeedOnBoundary * Time.deltaTime;
				float startEdgeLimit01 = _Params.minDistFromEdgeToBeginScroll01;
				float endEdgeLimit01 = 1f - _Params.minDistFromEdgeToBeginScroll01;

				if (abstrPoint01 < startEdgeLimit01)
					ScrollByAbstractDelta(scrollAbstrDeltaInCTSpace * (startEdgeLimit01 - abstrPoint01) / _Params.minDistFromEdgeToBeginScroll01); // scroll towards start
				else if (abstrPoint01 > endEdgeLimit01)
					ScrollByAbstractDelta(-scrollAbstrDeltaInCTSpace * (abstrPoint01 - endEdgeLimit01) / _Params.minDistFromEdgeToBeginScroll01); // towards end
			}
		}

		protected override void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			base.CollectItemsSizes(changeMode, count, indexIfInsertingOrRemoving, itemsDesc);

			if (changeMode != ItemCountChangeMode.RESET)
				return;

			if (count == 0)
				return;

			// Randomize sizes
			int indexOfFirstItemThatWillChangeSize = 0;
			int end = indexOfFirstItemThatWillChangeSize + count;

			itemsDesc.BeginChangingItemsSizes(indexOfFirstItemThatWillChangeSize);
			for (int i = indexOfFirstItemThatWillChangeSize; i < end; ++i)
				itemsDesc[i] = UnityEngine.Random.Range(_Params.DefaultItemSize / 3, _Params.DefaultItemSize * 3);
			itemsDesc.EndChangingItemsSizes();
		}

		/// <inheritdoc/>
		protected override MyViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new MyViewsHolder();
			instance.Init(_Params.itemPrefab, itemIndex);
			instance.draggableComponent.dragDropListener = this;

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(MyViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			MyModel model = Data[newOrRecycled.ItemIndex];

			newOrRecycled.titleText.text = "[id:" + model.id + "] " + model.title;
			newOrRecycled.background.color = model.color;
		}
		#endregion

		#region DraggableItem.IDragDropListener
		bool DraggableItem.IDragDropListener.OnPrepareToDragItem(DraggableItem item)
		{
			var dragged = GetItemViewsHolderIfVisible(item.RT);
			if (dragged == null)
				return false;

			var modelOfDragged = Data[dragged.ItemIndex];
			Debug.Log("Dragging with id " + modelOfDragged.id + ", ItemIndex " + dragged.ItemIndex);

			// Modifying the list manually, because RemoveItemWithViewsHolder will do a Remove itself
			Data.List.RemoveAt(dragged.ItemIndex);

			RemoveItemWithViewsHolder(dragged, true, false);
			UpdateScaleOfVisibleItems(dragged);

			_DragManager.EnterState_PreparingForDrag(dragged, modelOfDragged);

			return true;
		}

		void DraggableItem.IDragDropListener.OnBeginDragItem(PointerEventData eventData)
		{
			_DragManager.EnterState_Dragging(eventData);
		}
		
		void DraggableItem.IDragDropListener.OnDraggedItem(PointerEventData eventData)
		{
			bool isInViewport;
			var closestVH = GetClosestVHAtScreenPoint(eventData, out isInViewport);
			if (closestVH == null)
			{
				// TODO if needed
			}
			
			_DragManager.Dragged.background.color = isInViewport ? _DragManager.ModelOfDragged.color : _DragManager.ModelOfDragged.color * _Params.outsideColorTint;

			UpdateScaleOfVisibleItems(closestVH);
		}

		DraggableItem.OrphanedItemBundle DraggableItem.IDragDropListener.OnDroppedItem(PointerEventData eventData)
		{
			var orphaned = DropDraggedVHAndEnterNoneState(eventData);
			return orphaned;
		}

		bool DraggableItem.IDragDropListener.OnDroppedExternalItem(PointerEventData eventData, DraggableItem orphanedItemWithBundle)
		{
			bool grabbed = TryGrabOrphanedItemVH(eventData, orphanedItemWithBundle);
			return grabbed;
		}
		#endregion

		void ICancelHandler.OnCancel(BaseEventData eventData)
		{
			if (_DragManager.State == DragState.NONE)
				return;

			_DragManager.Dragged.draggableComponent.CancelDragSilently();

			DropDraggedVHAndEnterNoneState(null);
		}

		void UpdateScaleOfVisibleItems(MyViewsHolder vhToRotate)
		{
			foreach (var vh in _VisibleItems)
				vh.scalableViews.localScale = Vector3.one * (vh == vhToRotate ? .98f : 1f);
		}

		bool TryGrabOrphanedItemVH(PointerEventData eventData, DraggableItem orphanedItemWithBundle)
		{
			bool isPointInViewport;
			var closestVH = GetClosestVHAtScreenPoint(eventData, out isPointInViewport);
			if (!isPointInViewport)
				return false;

			orphanedItemWithBundle.dragDropListener = this;

			int atIndex = 0;
			if (closestVH == null) // no items present, but the point is in viewport => add it to the list
				atIndex = 0;
			else
				atIndex = closestVH.ItemIndex;

			var vh = orphanedItemWithBundle.OrphanedBundle.views as MyViewsHolder;
			var model = orphanedItemWithBundle.OrphanedBundle.model as MyModel;
			Debug.Log("Dropped with id " + model.id + ", ItemIndex " + vh.ItemIndex + ", droppedAtItemIndex " + atIndex);

			// Modifying the list manually, because InsertItemWithViewsHolder will do an Insert itself
			Data.List.Insert(atIndex, model);

			float itemSizeToUse = vh.root.rect.height; // for vertical ScrollViews, "size" = height
			InsertItemWithViewsHolder(vh, atIndex, false);

			// The adapter will use _Params.DefaultItemSize to set the item's size, but we want it to keep the same size.
			// The alternative way of doing it (a bit faster, but negligeable) is to store <itemSizeToUse> somewhere and 
			// pass it in CollectItemsSizes for <atIndex>. CollectItemsSizes() is always called during count changes, including during InsertItemWithViewsHolder()
			RequestChangeItemSizeAndUpdateLayout(atIndex, itemSizeToUse, false, true);

			//// Restore the scale of all items;
			//UpdateScaleOfVisibleItems(null);

			return true;
		}

		DraggableItem.OrphanedItemBundle DropDraggedVHAndEnterNoneState(PointerEventData eventData)
		{
			var dragged = _DragManager.Dragged;
			var modelOfDragged = _DragManager.ModelOfDragged;
			dragged.background.color = modelOfDragged.color;

			int atIndex;
			if (eventData == null)
				// No event means something was canceled => put it back at the same index;
				atIndex = dragged.ItemIndex;
			else
			{
				bool isPointInViewport;
				var closestVH = GetClosestVHAtScreenPoint(eventData, out isPointInViewport);

				if (!isPointInViewport)
				{
					Debug.Log("Orphaned item (dropped outside) with id " + modelOfDragged.id + ", ItemIndex " + dragged.ItemIndex);
					var orphaned = new DraggableItem.OrphanedItemBundle
					{
						model = modelOfDragged,
						views = dragged,
						previousOwner = this
					};
					dragged.draggableComponent.dragDropListener = null;
					_DragManager.EnterState_None();

					// Restore the scale of all items;
					UpdateScaleOfVisibleItems(null);

					return orphaned;
				}

				if (closestVH == null) // no items present, but the point is in viewport => add it to the list
					atIndex = 0;
				else
					atIndex = closestVH.ItemIndex;
			}

			Debug.Log("Dropped with id " + modelOfDragged.id + ", ItemIndex " + dragged.ItemIndex + ", droppedAtItemIndex " + atIndex);

			// Modifying the list manually, because InsertItemWithViewsHolder will do an Insert itself
			Data.List.Insert(atIndex, modelOfDragged);

			// Before important changes in the view state, enter a clean state to prepare for other drags
			// and store the dragged in a local var (since it'll be removed from the manager)
			_DragManager.EnterState_None();

			float itemSizeToUse = dragged.root.rect.height; // for vertical ScrollViews, "size" = height
			InsertItemWithViewsHolder(dragged, atIndex, false);

			// The adapter will use _Params.DefaultItemSize to set the item's size, but we want it to keep the same size.
			// The alternative way of doing it (a bit faster, but negligeable) is to store <itemSizeToUse> somewhere and 
			// pass it in CollectItemsSizes for <atIndex>. CollectItemsSizes() is always called during count changes, including during InsertItemWithViewsHolder()
			RequestChangeItemSizeAndUpdateLayout(atIndex, itemSizeToUse, false, true);

			// Restore the scale of all items;
			UpdateScaleOfVisibleItems(null);

			return null;
		}

		MyViewsHolder GetClosestVHAtScreenPoint(PointerEventData eventData, out bool isPointInViewport)
		{
			Vector2 localPoint;
			isPointInViewport = false;
			//isPointInViewport = RectTransformUtility.RectangleContainsScreenPoint(
			//	_Params.Viewport, 
			//	eventData.position, 
			//	_Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : eventData.pressEventCamera
			//);
			if (!GetLocalPointInViewportIfWithinBounds(eventData.position, eventData.pressEventCamera, out localPoint))
				//localPoint = Vector2.zero; // fail-save: return the local position of the viewport's pivot
				return null;

			isPointInViewport = true;

			// Passing 0f so that the item with its start (i.e. top in our case) edge will be returned, and so the item following the dragged one will be shifted downwards
			float _;
			var vh = GetViewsHolderClosestToViewportPoint(_Canvas, _CanvasRT, localPoint, 0f, out _);
			if (vh == null)
				return null;

			return vh;
		}

		bool GetLocalPointInViewport(Vector2 screenPoint, Camera camera, out Vector2 localPoint)
		{ return RectTransformUtility.ScreenPointToLocalPointInRectangle(_Params.Viewport, screenPoint, camera, out localPoint); }

		bool GetLocalPointInViewportIfWithinBounds(Vector2 screenPoint, Camera camera, out Vector2 localPoint)
		{ return GetLocalPointInViewport(screenPoint, camera, out localPoint) && _Params.Viewport.IsLocalPointInRect(localPoint); }
	}


	[Serializable]
	public class MyModel
	{
		public int id;
		public string title;
		public Color color;
	}


	// This in almost all cases will contain the prefab and your list of models
	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		[Range(0f, 1f)]
		public float minDistFromEdgeToBeginScroll01 = .2f;
		public float maxScrollSpeedOnBoundary = 3000f;
		//public bool destroyOnOutsideDrop;
		public Color outsideColorTint = new Color(1f, 1f, 1f, .8f);
	}


	public class MyViewsHolder : BaseItemViewsHolder
	{
		public Text titleText;

		// Using a child that helps us scale the item's views, because the scale of the item itself is managed exclusively by the adapter
		public DraggableItem draggableComponent;
		public RectTransform scalableViews;
		public Image background;


		public override void CollectViews()
		{
			base.CollectViews();

			draggableComponent = root.GetComponent<DraggableItem>();
			root.GetComponentAtPath("ScalableViews", out scalableViews);
			scalableViews.GetComponentAtPath("TitlePanel/TitleText", out titleText);
			scalableViews.GetComponentAtPath("Background", out background);
		}
	}


	class DragStateManager
	{
		public MyViewsHolder Dragged { get; private set; }
		public MyModel ModelOfDragged { get; private set; }
		public DragState State { get; private set; }

		//Canvas _Canvas;
		//RectTransform _CanvasRT;


		public void EnterState_None()
		{
			Dragged = null;
			ModelOfDragged = null;
			State = DragState.NONE;
		}

		public void EnterState_PreparingForDrag(MyViewsHolder dragged, MyModel modelOfDragged)
		{
			Dragged = dragged;
			ModelOfDragged = modelOfDragged;

			State = DragState.PREPARING_FOR_DRAG;
		}

		public void EnterState_Dragging(PointerEventData eventData)
		{
			State = DragState.DRAGGING;
		}
	}


	enum DragState
	{
		NONE,
		PREPARING_FOR_DRAG,
		DRAGGING,
	}
}
