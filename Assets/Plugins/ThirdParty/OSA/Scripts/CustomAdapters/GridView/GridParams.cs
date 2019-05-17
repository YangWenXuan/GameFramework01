//#define MIGRATE_3_2_TO_4_1_AVAILABLE
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Com.TheFallenGames.OSA.Core;

namespace Com.TheFallenGames.OSA.CustomAdapters.GridView
{
	/// <summary> <para>Base class for params to beused with a <see cref="GridAdapter{TParams, TCellVH}"/></para>
	/// </summary>
	[Serializable] // serializable, so it can be shown in inspector
	public class GridParams : BaseParams
	{
		#region Configuration
		public GridConfig grid = new GridConfig();

		/// <summary>The max. number of cells in a row group (for vertical ScrollView) or column group (for horizontal ScrollView). 
		/// Set to -1 to fill with cells when there's space - note that it only works when cell's flexibleWidth is not used (flexibleHeight for horizontal scroll views)</summary>
		[SerializeField]
		[FormerlySerializedAs("numCellsPerGroup")] // in pre v4.0
		[Tooltip("The max. number of cells in a row group (for vertical ScrollView) or column group (for horizontal ScrollView).\nSet to -1 to fill with cells when there's space - note that it only works when cell's flexibleWidth is not used (flexibleHeight for horizontal scroll views)")]
		int _MaxCellsPerGroup = -1;
		[Obsolete("Use grid.MaxCellsPerGroup instead", true)]
		public int MaxCellsPerGroup { get { return _MaxCellsPerGroup; } protected set { _MaxCellsPerGroup = grid.MaxCellsPerGroup = value; } }
		
		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("cellPrefab")]
		RectTransform _CellPrefab = null;
		[Obsolete("Use grid.cellPrefab instead", true)]
		public RectTransform cellPrefab { get { return grid.cellPrefab; } set { grid.cellPrefab = value; } }

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("alignmentOfCellsInGroup")]
		TextAnchor _AlignmentOfCellsInGroup = TextAnchor.UpperLeft;
		[Obsolete("Use grid.alignmentOfCellsInGroup instead", true)]
		public TextAnchor alignmentOfCellsInGroup { get { return grid.alignmentOfCellsInGroup; } set { grid.alignmentOfCellsInGroup = value; } }

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("spacingInGroup")]
		float _SpacingInGroup = -1f;
		[Obsolete("Use grid.spacingInGroup instead", true)]
		public float spacingInGroup { get { return grid.spacingInGroup; } set { grid.spacingInGroup = value; } }

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("groupPadding")]
		RectOffset _GroupPadding = null;
		[Obsolete("Use grid.groupPadding instead", true)]
		public RectOffset groupPadding { get { return grid.groupPadding; } set { grid.groupPadding = value; } }

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("cellWidthForceExpandInGroup")]
		bool _CellWidthForceExpandInGroup = false;
		[Obsolete("Use grid.cellWidthForceExpandInGroup instead", true)]
		public bool cellWidthForceExpandInGroup { get { return grid.cellWidthForceExpandInGroup; } set { grid.cellWidthForceExpandInGroup = value; } }

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("cellHeightForceExpandInGroup")]
		bool _CellHeightForceExpandInGroup = false;
		[Obsolete("Use grid.cellHeightForceExpandInGroup instead", true)]
		public bool cellHeightForceExpandInGroup { get { return grid.cellHeightForceExpandInGroup; } set { grid.cellHeightForceExpandInGroup = value; } }
		#endregion

		public int CurrentUsedNumCellsPerGroup { get { return _CurrentUsedNumCellsPerGroup; } }
		public LayoutElement CellPrefabLayoutElement { get { return _CellPrefabLayoutElement; } }

		//// Both of these should be at least 1
		//int NumCellsPerGroupHorizontally { get { return IsHorizontal ? 1 : numCellsPerGroup; } }
		//int NumCellsPerGroupVertically { get { return IsHorizontal ? numCellsPerGroup : 1; } }

		/// <summary>Cached prefab, auto-generated at runtime, first time <see cref="GetGroupPrefab(int)"/> is called</summary>
		HorizontalOrVerticalLayoutGroup _TheOnlyGroupPrefab;
		int _CurrentUsedNumCellsPerGroup;
		LayoutElement _CellPrefabLayoutElement;

#if MIGRATE_3_2_TO_4_1_AVAILABLE
		public override bool MigrateFieldsToVersion4(ScrollRect scrollRect, out string error, out string additionalInfoOnSuccess)
		{
			if (!base.MigrateFieldsToVersion4(scrollRect, out error, out additionalInfoOnSuccess))
				return false;

			if (grid == null)
				grid = new GridConfig();
			grid.alignmentOfCellsInGroup = _AlignmentOfCellsInGroup;
			grid.cellHeightForceExpandInGroup = _CellHeightForceExpandInGroup;
			grid.cellPrefab = _CellPrefab;
			grid.cellWidthForceExpandInGroup = _CellWidthForceExpandInGroup;
			grid.groupPadding = _GroupPadding;
			grid.MaxCellsPerGroup = _MaxCellsPerGroup;
			grid.spacingInGroup = _SpacingInGroup;

			return true;
		}
#endif

		/// <inheritdoc/>
		public override void InitIfNeeded(IOSA iAdapter)
		{
			base.InitIfNeeded(iAdapter);

			if (!grid.cellPrefab)
				throw new UnityException("OSA: " + typeof(GridParams) + ": the prefab was not set. Please set it through inspector or in code");
			_CellPrefabLayoutElement = grid.cellPrefab.GetComponent<LayoutElement>();
			if (!_CellPrefabLayoutElement)
				throw new UnityException("OSA: " + typeof(GridParams) + ": no LayoutElement found on the cellPrefab: you should add one to configure how the cell's parent LayoutGroup should position/size it");
			
			if (grid.spacingInGroup == -1f)
				grid.spacingInGroup = ContentSpacing;


			if (!grid.UseDefaulfItemSizeForCellGroupSize)
			{
				// DefaultItemSize refers to the group's size here, so we're also adding the groupPadding to it
				if (IsHorizontal)
				{
					DefaultItemSize = _CellPrefabLayoutElement.preferredWidth;
					if (DefaultItemSize < 0)
					{
						DefaultItemSize = _CellPrefabLayoutElement.minWidth;
						if (DefaultItemSize < 0)
						{
							if (_CellPrefabLayoutElement.flexibleWidth == -1)
								throw new UnityException(
									"OSA: " + typeof(GridParams) + ".cellPrefab.LayoutElement: Could not determine the cell group's width(UseDefaulfItemSizeForCellGroupSize=false). " +
									"Please specify at least preferredWidth, minWidth or flexibleWidth(case in which the current width of the cell will be used as the group's width)"
								);
							DefaultItemSize = _CellPrefab.rect.width;
						}
					}

					DefaultItemSize += grid.groupPadding.horizontal;
				}
				else {
					DefaultItemSize = _CellPrefabLayoutElement.preferredHeight;
					if (DefaultItemSize < 0)
					{
						DefaultItemSize = _CellPrefabLayoutElement.minHeight;
						if (DefaultItemSize < 0)
						{
							if (_CellPrefabLayoutElement.flexibleHeight == -1)
								throw new UnityException(
									"OSA: " + typeof(GridParams) + ".cellPrefab.LayoutElement: Could not determine the cell group's height(UseDefaulfItemSizeForCellGroupSize=false). " +
									"Please specify at least preferredHeight, minHeight or flexibleHeight(case in which the current height of the cell will be used as the group's height)"
								);
							DefaultItemSize = _CellPrefab.rect.height;
						}
					}

					DefaultItemSize += grid.groupPadding.vertical;
				}
			}

			if (_MaxCellsPerGroup == 0)
				_MaxCellsPerGroup = -1;

			_CurrentUsedNumCellsPerGroup = CalculateCurrentNumCellsPerGroup();

			// Hotfix 12.10.2017 14:45: There's a bug in Unity on some versions: creating a new GameObject at runtime and adding it a RectTransform cannot be done in Awake() or OnEnabled().
			// See: https://issuetracker.unity3d.com/issues/error-when-creating-a-recttransform-component-in-an-awake-call-of-an-instantiated-monobehaviour
			// The bug was initially found in a project where the initial count is 0 (when Start() is called), then the scrollview is disabled, set a non-zero count, then enabled back,
			// and in OnEnable() the user called ResetItems(), which triggered the lazy-instantiation of the group prefab - since it's created in the first GetGroupPrefab() call.
			// Solved it by creating the prefab here, because InitIfNeeded(IOSA) is called at Init time (in MonoBehaviour.Start())
			CreateOrReinitCellGroupPrefab();
		}

		public int CalculateCurrentNumCellsPerGroup()
		{
			if (_MaxCellsPerGroup > 0)
			{
				if (IsHorizontal)
				{
					if (_CellPrefabLayoutElement.preferredHeight == -1f 
						&& _CellPrefabLayoutElement.minHeight == -1f 
						&& _CellPrefabLayoutElement.flexibleHeight == -1f
						&& !grid.cellHeightForceExpandInGroup)
						Debug.Log(
							"OSA: " + typeof(GridParams) +
							".cellPrefab.LayoutElement: Using a fixed number of rows (MaxCellsPerGroup is " + _MaxCellsPerGroup +
							"), but none of the prefab's minHeight/preferredHeight/flexibleHeight is set, nor cellHeightForceExpandInGroup is true. " +
							"The cells will have 0 height initially. This is rarely the intended behavior" +
							"), but none of the prefab's minWidth/preferredWidth/flexibleWidth is set. " +
							"Could not determine the cell group's width. Using the current prefab's width (" + DefaultItemSize + ")"
						);
				}
				else
				{
					if (_CellPrefabLayoutElement.minWidth == -1f
						&& _CellPrefabLayoutElement.preferredWidth == -1f
						&& _CellPrefabLayoutElement.flexibleWidth == -1f
						&& !grid.cellWidthForceExpandInGroup)
						Debug.Log(
							"OSA: " + typeof(GridParams) +
							".cellPrefab.LayoutElement: Using a fixed number of columns (MaxCellsPerGroup is " + _MaxCellsPerGroup +
							"), but none of the prefab's minWidth/preferredWidth/flexibleWidth is set, nor cellWidthForceExpandInGroup is true. " +
							"The cells will have 0 width initially. This is rarely the intended behavior"
						);
				}

				return _MaxCellsPerGroup;
			}

			var scrollViewSize = ScrollViewRT.rect.size;
			float cellSize, availSize;
			if (IsHorizontal)
			{
				//if (_CellPrefabLayoutElement.flexibleHeight >= 0f)
				//{
				//	Debug.Log("OSA: " + typeof(GridParams) + ".cellPrefab.LayoutElement: Can't use flexibleHeight with variable number of cells per group (numCellsPerGroup is -1). Disabling..");
				//	_CellPrefabLayoutElement.flexibleHeight = -1f;
				//}

				cellSize = _CellPrefabLayoutElement.preferredHeight;
				if (cellSize <= 0f)
				{
					cellSize = _CellPrefabLayoutElement.minHeight;
					if (cellSize <= 0f)
						throw new UnityException("OSA: " + typeof(GridParams) + ".cellPrefab.LayoutElement: Please specify at least preferredHeight or minHeight when using a variable number of cells per group (MaxCellsPerGroup is "+ _MaxCellsPerGroup + ")");
				}
				availSize = scrollViewSize.y - grid.groupPadding.vertical - ContentPadding.vertical;
			}
			else
			{
				//if (_CellPrefabLayoutElement.flexibleWidth >= 0f)
				//{
				//	Debug.Log("OSA: " + typeof(GridParams) + ".cellPrefab.LayoutElement: Can't use flexibleWidth with variable number of cells per group (numCellsPerGroup == -1). Disabling..");
				//	_CellPrefabLayoutElement.flexibleWidth = -1f;
				//}

				cellSize = _CellPrefabLayoutElement.preferredWidth;
				if (cellSize <= 0f)
				{
					cellSize = _CellPrefabLayoutElement.minWidth;
					if (cellSize <= 0f)
						throw new UnityException("OSA: " + typeof(GridParams) + ".cellPrefab.LayoutElement: Please specify at least preferredWidth or minWidth when using a variable number of cells per group (MaxCellsPerGroup is "+ _MaxCellsPerGroup + ")");
				}
				availSize = scrollViewSize.x - grid.groupPadding.horizontal - ContentPadding.horizontal;
			}

			int numCellsPerGroupToUse = Mathf.FloorToInt((availSize + grid.spacingInGroup) / (cellSize + grid.spacingInGroup));
			if (numCellsPerGroupToUse < 1)
				numCellsPerGroupToUse = 1;

			return numCellsPerGroupToUse;
		}

		/// <summary>Returns the prefab to use as LayoutGroup for the group with index <paramref name="forGroupAtThisIndex"/></summary>
		public virtual HorizontalOrVerticalLayoutGroup GetGroupPrefab(int forGroupAtThisIndex)
		{
			if (_TheOnlyGroupPrefab == null)
				throw new UnityException("GridParams.InitIfNeeded() was not called by OSA. Did you forget to call base.Start() in <YourAdapter>.Start()?");

			return _TheOnlyGroupPrefab;
		}

		public virtual int GetGroupIndex(int cellIndex) {
			if (this._CurrentUsedNumCellsPerGroup == 0) {
				return 0;
			}
			return cellIndex / _CurrentUsedNumCellsPerGroup;
		}

		public virtual int GetNumberOfRequiredGroups(int numberOfCells) { return numberOfCells == 0 ? 0 : GetGroupIndex(numberOfCells - 1) + 1; }

		protected void CreateOrReinitCellGroupPrefab()
		{
			if (!_TheOnlyGroupPrefab)
			{
				var go = new GameObject(ScrollViewRT.name + "_CellGroupPrefab", typeof(RectTransform));

				// Additional reminder of the "add recttransform in awake" bug explained in InitIfNeeded()
				if (!(go.transform is RectTransform))
					Debug.LogException(new UnityException("OSA: Don't call OSA.Init() outside MonoBehaviour.Start()!"));

				// TODO also integrate the new SetViewsHolderEnabled functionality here, for grids
				go.SetActive(false);
				go.transform.SetParent(ScrollViewRT, false);
				if (IsHorizontal)
					_TheOnlyGroupPrefab = go.AddComponent<VerticalLayoutGroup>(); // groups are columns in a horizontal scrollview
				else
					_TheOnlyGroupPrefab = go.AddComponent<HorizontalLayoutGroup>(); // groups are rows in a vertical scrollview
			}

			_TheOnlyGroupPrefab.spacing = grid.spacingInGroup;
			_TheOnlyGroupPrefab.childForceExpandWidth = grid.cellWidthForceExpandInGroup;
			_TheOnlyGroupPrefab.childForceExpandHeight = grid.cellHeightForceExpandInGroup;
			_TheOnlyGroupPrefab.childAlignment = grid.alignmentOfCellsInGroup;
			_TheOnlyGroupPrefab.padding = grid.groupPadding;
		}

		[Serializable]
		public class GridConfig
		{
			/// <summary>The max. number of cells in a row group (for vertical ScrollView) or column group (for horizontal ScrollView). 
			/// Set to -1 to fill with cells when there's space - note that it only works when cell's flexibleWidth is not used (flexibleHeight for horizontal scroll views)</summary>
			[SerializeField]
			[HideInInspector]
			[FormerlySerializedAs("numCellsPerGroup")] // in pre v4.0
			[Tooltip("The max. number of cells in a row group (for vertical ScrollView) or column group (for horizontal ScrollView).\nSet to -1 to fill with cells when there's space - note that it only works when cell's flexibleWidth is not used (flexibleHeight for horizontal scroll views)")]
			int _MaxCellsPerGroup = -1;
			public int MaxCellsPerGroup { get { return _MaxCellsPerGroup; } set { _MaxCellsPerGroup = value; } }

			/// <summary> If true, the <see cref="BaseParams.DefaultItemSize"/> property will be used for the height of a row (the width of a column, for horizotal orientation). Leave to false to automatically infer based on the cellPrefab's LayoutElement's values.</summary>
			[SerializeField]
			[Tooltip("If true, the DefaultItemSize property will be used for the height of a row, if using a verical scroll view (the width of a column, otherwise). Leave to false to automatically infer based on the cellPrefab's LayoutElement's values")]
			bool _UseDefaulfItemSizeForCellGroupSize = false;
			public bool UseDefaulfItemSizeForCellGroupSize { get { return _UseDefaulfItemSizeForCellGroupSize; } protected set { _UseDefaulfItemSizeForCellGroupSize = value; } }

			/// <summary>The prefab to use for each cell</summary>
			public RectTransform cellPrefab = null;

			/// <summary>The alignment of cells inside their parent LayoutGroup (Vertical or Horizontal, depending on ScrollView's orientation)</summary>
			[Tooltip("The alignment of cells inside their parent LayoutGroup (Vertical or Horizontal, depending on ScrollView's orientation)")]
			public TextAnchor alignmentOfCellsInGroup = TextAnchor.UpperLeft;

			[Tooltip("The spacing between the cells of a group. Leave it to -1 to use the same value as contetnSpacing (i.e. the spacing between the groups), so verical and horizontal spacing will be the same")]
			public float spacingInGroup = -1f;

			/// <summary>The padding of cells as a whole inside their parent LayoutGroup</summary>
			public RectOffset groupPadding = new RectOffset();

			/// <summary>Wether to force the cells to expand in width inside their parent LayoutGroup</summary>
			public bool cellWidthForceExpandInGroup = false;

			/// <summary>Wether to force the cells to expand in height inside their parent LayoutGroup</summary>
			public bool cellHeightForceExpandInGroup = false;
		}
	}
}