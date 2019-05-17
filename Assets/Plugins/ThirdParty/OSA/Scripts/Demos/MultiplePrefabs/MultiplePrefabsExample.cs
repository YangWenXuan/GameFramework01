using System;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.MultiplePrefabs.ViewsHolders;
using Com.TheFallenGames.OSA.Demos.MultiplePrefabs.Models;

namespace Com.TheFallenGames.OSA.Demos.MultiplePrefabs
{
	/// <summary>
	/// <para>Example implementation demonstrating the use of 2 different views holders, representing 2 different models into their own prefab, with a common Title property, displayed in a Text found on both prefabs. </para>
	/// <para>The only constrain is for the models to have a common ancestor class and for the views holders to also have a common ancestor class</para>
	/// <para>Also, the <see cref="BidirectionalModel"/> is used to demonstrate how the data can flow from the model to the views, but also from the views to the model (i.e. this model updates when the user changes the value of its corresponding slider)</para>
	/// <para>At the core, everything's the same as in other example implementations, so if something's not clear, check them (SimpleExample is a good start)</para>
	/// </summary>
	public class MultiplePrefabsExample : OSA<MyParams, BaseVH>, ExpandCollapseOnClick.ISizeChangesHandler	
	{
		public SimpleDataHelper<BaseModel> Data { get; private set; }

		/// <summary> Used to only allow one item to be expanded at once </summary>
		int _IndexOfCurrentlyExpandedItem;


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			Data = new SimpleDataHelper<BaseModel>(this);

			base.Start();
		}

		/// <inheritdoc/>
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			_IndexOfCurrentlyExpandedItem = -1; // at initialization, and each time the item count changes, this should be invalidated

			base.ChangeItemsCount(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}

		/// <summary>
		/// Creates either a <see cref="BidirectionalVH"/> or a <see cref="ExpandableVH"/>, depending on the type of the model at index <paramref name="itemIndex"/>. Calls <see cref="AbstractViewsHolder.Init(RectTransform, int, bool, bool)"/> on it, which instantiates the prefab etc.
		/// </summary>
		/// <seealso cref="OSA{TParams, TItemViewsHolder}.CreateViewsHolder(int)"/>
		protected override BaseVH CreateViewsHolder(int itemIndex)
		{
			var modelType = Data[itemIndex].CachedType;// _ModelTypes[itemIndex];
			if (modelType == typeof(BidirectionalModel)) // very efficient type comparison, since typeof() is evaluated at compile-time
			{
				var instance = new BidirectionalVH();
				instance.Init(_Params.bidirectionalPrefab, itemIndex);

				return instance;
			}
			else if (modelType == typeof(ExpandableModel))
			{
				var instance = new ExpandableVH();
				instance.Init(_Params.expandablePrefab, itemIndex);

				instance.expandCollapseOnClickBehaviour.sizeChangesHandler = this;
				instance.expandCollapseOnClickBehaviour.expandFactor = _Params.expandableItemExpandFactor;

				return instance;
			}
			else
				throw new InvalidOperationException("Unrecognized model type: " + modelType.Name);
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(BaseVH newOrRecycled)
		{
			// Initialize/update the views from the associated model
			BaseModel model = Data[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(model);
		}

		/// <summary>
		/// <para>This is overidden only so that the items' title will be updated to reflect its new index in case of Insert/Remove, because the index is not stored in the model</para>
		/// <para>If you don't store/care about the index of each item, you can omit this</para>
		/// <para>For more info, see <see cref="OSA{TParams, TItemViewsHolder}.OnItemIndexChangedDueInsertOrRemove(TItemViewsHolder, int, bool, int)"/> </para>
		/// </summary>
		protected override void OnItemIndexChangedDueInsertOrRemove(BaseVH shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{
			base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

			shiftedViewsHolder.UpdateTitleOnly(Data[shiftedViewsHolder.ItemIndex]);
		}

		/// <summary>Overriding the base implementation, which always returns true. In this case, a views holder is recyclable only if its <see cref="BaseVH.CanPresentModelType(Type)"/> returns true for the model at index <paramref name="indexOfItemThatWillBecomeVisible"/></summary>
		/// <seealso cref="OSA{TParams, TItemViewsHolder}.IsRecyclable(TItemViewsHolder, int, double)"/>
		protected override bool IsRecyclable(BaseVH potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double heightOfItemThatWillBecomeVisible)
		{ return potentiallyRecyclable.CanPresentModelType(Data[indexOfItemThatWillBecomeVisible].CachedType); }

		#region ExpandCollapseOnClick.ISizeChangesHandler implementation
		bool ExpandCollapseOnClick.ISizeChangesHandler.HandleSizeChangeRequest(RectTransform rt, float newRequestedSize)
		{
			var vh = GetItemViewsHolderIfVisible(rt);

			// If the vh is visible and the request is accepted, we update our list of sizes
			if (vh != null)
			{
				var modelOfExpandingItem = Data[vh.ItemIndex] as ExpandableModel;
				RequestChangeItemSizeAndUpdateLayout(vh, newRequestedSize, _Params.freezeItemEndEdgeWhenResizing);

				if (_IndexOfCurrentlyExpandedItem != -1)
				{
					var vhAsExpandable = vh as ExpandableVH;
					if (!vhAsExpandable.expandCollapseOnClickBehaviour.expanded) // the item is currently expanding => simultaneously collapse the previously expanded one with the same percentage 
					{
						float expandingItem_ExpandedAmount01 = newRequestedSize / (modelOfExpandingItem.nonExpandedSize * _Params.expandableItemExpandFactor);
						var modelOfExpandedItem = Data[_IndexOfCurrentlyExpandedItem] as ExpandableModel;
						var expandedItemNewRequestedSize =
							Mathf.Lerp(
								modelOfExpandedItem.nonExpandedSize,
								modelOfExpandedItem.nonExpandedSize * _Params.expandableItemExpandFactor,
								1f - expandingItem_ExpandedAmount01 // the previously expanded item grows inversely than the newly expanding one
							);
						RequestChangeItemSizeAndUpdateLayout(_IndexOfCurrentlyExpandedItem, expandedItemNewRequestedSize, _Params.freezeItemEndEdgeWhenResizing);
					}
				}

				return true;
			}

			return false;
		}

		public void OnExpandedStateChanged(RectTransform rt, bool expanded)
		{
			var vh = GetItemViewsHolderIfVisible(rt);

			// If the vh is visible and the request is accepted, we update the model's "expanded" field
			if (vh != null)
			{
				var asExpandableModel = Data[vh.ItemIndex] as ExpandableModel;
				if (asExpandableModel == null)
					throw new UnityException(
						"MultiplePrefabsExample.MyScrollRectAdapter.OnExpandedStateChanged: item model at index " + vh.ItemIndex
						+ " is not of type " + typeof(ExpandableModel).Name + ", as expected by the views holder having this itemIndex. Happy debugging :)");
				asExpandableModel.expanded = expanded;

				if (expanded)
				{
					// Mark previous as non-expanded, if any
					if (_IndexOfCurrentlyExpandedItem != -1)
					{
						var lastExpandedModel = Data[_IndexOfCurrentlyExpandedItem] as ExpandableModel;
						lastExpandedModel.expanded = false;

						// If it's also visible, update its views too
						var collapsedVHIfVisible = GetItemViewsHolderIfVisible(_IndexOfCurrentlyExpandedItem);
						if (collapsedVHIfVisible != null)
							collapsedVHIfVisible.UpdateViews(lastExpandedModel);
					}

					_IndexOfCurrentlyExpandedItem = vh.ItemIndex;
				}
				else if (vh.ItemIndex == _IndexOfCurrentlyExpandedItem) // the currently expanded item was collapsed => invalidate indexOfCurrentlyExpandedItem 
					_IndexOfCurrentlyExpandedItem = -1;
			}
		}
		#endregion

		#endregion
	}

	/// <summary>
	/// Contains the 2 prefabs associated with the 2 views holders and the data list containing models of the 2 type, stored as <see cref="BaseModel"/>
	/// </summary>
	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParams
	{
		public RectTransform bidirectionalPrefab, expandablePrefab;
		public float expandableItemExpandFactor = 2f;

		[NonSerialized]
		public bool freezeItemEndEdgeWhenResizing;
	}
}
