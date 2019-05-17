using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;

namespace Com.TheFallenGames.OSA.Demos.SelectAndDelete
{
    /// <summary>
    /// Implementation demonstrating the usage of a <see cref="GridAdapter{TParams, TCellVH}"/> with support for selecting items on long click and deleting them with a nice collapse animation
    /// </summary>
    public class SelectAndDeleteExample : GridAdapter<MyGridParams, MyCellViewsHolder>, LongClickableItem.IItemLongClickListener, ExpandCollapseOnClick.ISizeChangesHandler//, ILazyListSimpleDataManager<BasicModel>
	{
		public event Action<bool> SelectionModeChanged;

		public LazyDataHelper<BasicModel> LazyData { get; private set; }
		public int CurrentFreeID { get { return _CurrentFreeID; } set { _CurrentFreeID = value; } }
		
		const float SELECTED_SCALE_FACTOR = .8f;
		readonly Vector3 SELECTED_SCALE = new Vector3(SELECTED_SCALE_FACTOR, SELECTED_SCALE_FACTOR, 1f);

		bool _SelectionMode;
		bool _WaitingForItemsToBeDeleted;
		int _CurrentFreeID = 0;
		List<BasicModel> _SelectedModels = new List<BasicModel>();


		#region GridAdapter implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			LazyData = new LazyDataHelper<BasicModel>(this, CreateNewModel);

			base.Start();
			_Params.deleteButton.onClick.AddListener(() => { if (!PlayPreDeleteAnimation()) DeleteSelectedItems(); }); // delete items directly, if selected item is visible to wait for the delete animation
			_Params.cancelButton.onClick.AddListener(() => SetSelectionModeWithChecks(false));
		}

		/// <inheritdoc/>
		protected override void Update()
		{
			base.Update();

			if (Input.GetKeyUp(KeyCode.Escape))
				SetSelectionModeWithChecks(false);
		}

		/// <inheritdoc/>
		public override void ChangeItemsCount(ItemCountChangeMode changeMode, int cellsCount, int indexIfAppendingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{
			// Assure nothing is selected before changing the count
			// Update: not calling RefreshSelectionStateForVisibleCells(), since UpdateCellViewsHolder() will be called for all cells anyway
			if (_SelectionMode)
				SetSelectionMode(false);
			UpdateSelectionActionButtons();

			base.ChangeItemsCount(changeMode, cellsCount, indexIfAppendingOrRemoving, contentPanelEndEdgeStationary, keepVelocity);
		}

		/// <seealso cref="GridAdapter{TParams, TCellVH}.Refresh(bool, bool)"/>
		public override void Refresh(bool contentPanelEndEdgeStationary = false /*ignored*/, bool keepVelocity = false)
		{
			_CellsCount = LazyData.Count;
			base.Refresh(_Params.freezeContentEndEdgeOnCountChange, keepVelocity);
		}

		/// <inheritdoc/>
		protected override CellGroupViewsHolder<MyCellViewsHolder> CreateViewsHolder(int itemIndex)
		{
			var cellsGroupVHInstance = base.CreateViewsHolder(itemIndex);

			// Set listeners for the Toggle in each cell. Will call OnCellToggled() when the toggled state changes
			// Set this adapter as listener for the OnItemLongClicked event
			for (int i = 0; i < cellsGroupVHInstance.ContainingCellViewsHolders.Length; ++i)
			{
				var cellVH = cellsGroupVHInstance.ContainingCellViewsHolders[i];
				cellVH.toggle.onValueChanged.AddListener(_ => OnCellToggled(cellVH));
				cellVH.longClickableComponent.longClickListener = this;
			}

			return cellsGroupVHInstance;
		}

		/// <summary> Called when a cell becomes visible </summary>
		/// <param name="holder"> use viewsHolder.ItemIndexto find your corresponding model and feed data into its views</param>
		/// <see cref="GridAdapter{TParams, TCellVH}.UpdateCellViewsHolder(TCellVH)"/>
		protected override void UpdateCellViewsHolder(MyCellViewsHolder holder)
		{
			var model = LazyData.GetOrCreate(holder.ItemIndex);
			holder.UpdateViews(model);

			UpdateSelectionState(holder, model);
		}

		/// <inheritdoc/>
		protected override void OnBeforeRecycleOrDisableCellViewsHolder(MyCellViewsHolder viewsHolder, int newItemIndex)
		{
			viewsHolder.views.localScale = Vector3.one;
			viewsHolder.expandCollapseComponent.expanded = true;
		}
		#endregion

		#region LongClickableItem.IItemLongClickListener implementation
		public void OnItemLongClicked(LongClickableItem longClickedItem)
		{
			// Enter selection mode
			SetSelectionMode(true);
			RefreshSelectionStateForVisibleCells();
			UpdateSelectionActionButtons();

			if (_Params.autoSelectFirstOnSelectionMode)
			{
				// Find the cell views holder that corresponds to the LongClickableItem parameter & mark it as toggled
				int numVisibleCells = GetNumVisibleCells();
				for (int i = 0; i < numVisibleCells; ++i)
				{
					var cellVH = base.GetCellViewsHolder(i);

					if (cellVH.longClickableComponent == longClickedItem)
					{
						var model = LazyData.GetOrCreate(cellVH.ItemIndex);
						SelectModel(model);
						UpdateSelectionState(cellVH, model);

						break;
					}
				}
			}
		}
		#endregion

		#region ExpandCollapseOnClick.ISizeChangesHandler implementation
		public bool HandleSizeChangeRequest(RectTransform rt, float newSize) { rt.localScale = SELECTED_SCALE * newSize; return true; }
		public void OnExpandedStateChanged(RectTransform rt, bool expanded)
		{
			if (expanded) // only caring about the shrinking case
				return;

			if (_WaitingForItemsToBeDeleted)
			{
				_WaitingForItemsToBeDeleted = false;

				// Prevent resizing of other items after the CurrentDeleteAnimationDone() was called 
				// (since we only call it for the first OnExpandedStateChanged callback and ignore the following ones)
				var numVisibleCells = GetNumVisibleCells();
				for (int i = 0; i < numVisibleCells; ++i)
				{
					var cellVH = GetCellViewsHolder(i);
					if (cellVH != null)
						cellVH.expandCollapseComponent.sizeChangesHandler = null;
				}

				DeleteSelectedItems();
			}
		}
		#endregion

		#region DrawerCommandPanel events
		#endregion

		void UpdateSelectionState(MyCellViewsHolder viewsHolder, BasicModel model)
		{
			viewsHolder.longClickableComponent.gameObject.SetActive(!_SelectionMode); // can be long-clicked only if selection mode is off
			viewsHolder.toggle.gameObject.SetActive(_SelectionMode); // can be selected only if selection mode is on
			viewsHolder.toggle.isOn = model.isSelected;
		}

		void SetSelectionModeWithChecks(bool isSelectionMode)
		{
			if (_SelectionMode != isSelectionMode)
			{
				SetSelectionMode(isSelectionMode);
				RefreshSelectionStateForVisibleCells();
				UpdateSelectionActionButtons();
			}
		}

		/// <summary>Assumes the current state of SelectionMode is different than <paramref name="active"/></summary>
		void SetSelectionMode(bool active)
		{
			_SelectionMode = active;
			UnselectAllModels();
			_WaitingForItemsToBeDeleted = false;

			if (SelectionModeChanged != null)
				SelectionModeChanged(active);
		}

		void UpdateSelectionActionButtons()
		{
			if (!_SelectionMode)
				_Params.deleteButton.interactable = false;

			_Params.cancelButton.interactable = _SelectionMode;
		}

		bool PlayPreDeleteAnimation()
		{
			var numVisibleCells = GetNumVisibleCells();
			for (int i = 0; i < numVisibleCells; ++i)
			{
				var cellVH = GetCellViewsHolder(i);
				var m = LazyData.GetOrCreate(cellVH.ItemIndex);
				if (!m.isSelected) // faster to use the isSelected flag than to search through _SelectedModels
					continue;

				if (cellVH != null)
				{
					_WaitingForItemsToBeDeleted = true;
					cellVH.expandCollapseComponent.sizeChangesHandler = this;
					cellVH.expandCollapseComponent.OnClicked();
				}
			}

			return _WaitingForItemsToBeDeleted;
		}

		void RefreshSelectionStateForVisibleCells()
		{
			// Rather than calling Refresh, we retrieve the already-visible ones and update them manually (less lag)
			int visibleCellCount = GetNumVisibleCells();
			for (int i = 0; i < visibleCellCount; ++i)
			{
				var cellVH = GetCellViewsHolder(i);
				UpdateSelectionState(cellVH, LazyData.GetOrCreate(cellVH.ItemIndex));
			}
		}

		void OnCellToggled(MyCellViewsHolder cellVH)
		{
			// Update the model this cell is representing
			var model = LazyData.GetOrCreate(cellVH.ItemIndex);
			if (cellVH.toggle.isOn)
			{
				SelectModel(model);
				cellVH.views.localScale = SELECTED_SCALE;
			}
			else
			{
				UnselectModel(model);
				cellVH.views.localScale = Vector3.one;
			}
		}

		void SelectModel(BasicModel model)
		{
			if (!model.isSelected)
			{
				_SelectedModels.Add(model);
				model.isSelected = true;
			}
			_Params.deleteButton.interactable = true;
		}

		void UnselectModel(BasicModel model)
		{
			if (model.isSelected)
			{
				_SelectedModels.Remove(model);
				model.isSelected = false;
			}

			// Activate the delete button if at least one item was selected
			_Params.deleteButton.interactable = _SelectedModels.Count > 0;
		}

		// Faster than removing the models one by one using UnselectModel
		void UnselectAllModels()
		{
			for (int i = 0; i < _SelectedModels.Count; ++i)
				_SelectedModels[i].isSelected = false;
			_SelectedModels.Clear();
			_Params.deleteButton.interactable = false;
		}

		/// <summary>Deletes the selected items immediately</summary>
		void DeleteSelectedItems()
		{
			if (_SelectedModels.Count > 0)
			{
				// Remove models from adapter & update views
				// TODO see how necessary is to use an id-based approach to remove the items even faster frm a map instead of iterating through the list
				for (int i = 0; i < _SelectedModels.Count; ++i)
					LazyData.List.Remove(_SelectedModels[i]);

				// Re-enable selection mode
				if (_Params.keepSelectionModeAfterDeletion)
					SetSelectionModeWithChecks(true);

				// "Remove from disk" or similar
				foreach (var item in _SelectedModels)
					HandleItemDeletion(item);

				UnselectAllModels();

				Refresh(_Params.freezeContentEndEdgeOnCountChange);
			}
		}

		BasicModel CreateNewModel(int index)
		{
			return new BasicModel()
			{
				id = _CurrentFreeID++,
				//title = "Item ID: " + id,
			};
		}

		void HandleItemDeletion(BasicModel model)
        { Debug.Log("Deleted with id: " + model.id); }
	}


	[Serializable]
	public class MyGridParams : GridParams
	{
		/// <summary>Will be enabled when in selection mode and there are items selsted. Disabled otherwise</summary>
		public Button deleteButton;

		/// <summary>Will be enabled when in selection mode. Pressing it will exit selection mode. Useful for devices with no back/escape (iOS)</summary>
		public Button cancelButton;

		/// <summary>Select the first item when entering selection mode</summary>
		public bool autoSelectFirstOnSelectionMode = true;

		/// <summary>Wether to remain in selection mode after deletion or not</summary>
		public bool keepSelectionModeAfterDeletion = true;

		[NonSerialized]
		public bool freezeContentEndEdgeOnCountChange;
	}


	public class BasicModel
	{
		// Data state
		public int id;
		public readonly Color color = DemosUtil.GetRandomColor(true);

		// View state
		public bool isSelected;
	}


	/// <summary>All views holders used with GridAdapter should inherit from <see cref="CellViewsHolder"/></summary>
	public class MyCellViewsHolder : CellViewsHolder
	{
		public Text title;
		public Toggle toggle;
		public LongClickableItem longClickableComponent;
		public Image background;
		public ExpandCollapseOnClick expandCollapseComponent;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			toggle = views.Find("Toggle").GetComponent<Toggle>();
			title = views.Find("TitleText").GetComponent<Text>();
			longClickableComponent = views.Find("LongClickableArea").GetComponent<LongClickableItem>();
			background = views.GetComponent<Image>();
			expandCollapseComponent = views.GetComponent<ExpandCollapseOnClick>();
			expandCollapseComponent.nonExpandedSize = .001f;
			expandCollapseComponent.expandFactor = 1 / expandCollapseComponent.nonExpandedSize;
			expandCollapseComponent.expanded = true;
		}

		public void UpdateViews(BasicModel model)
		{
			title.text = "#" + ItemIndex + " [id:" + model.id + "]";
			background.color = model.color;
		}
	}
}
