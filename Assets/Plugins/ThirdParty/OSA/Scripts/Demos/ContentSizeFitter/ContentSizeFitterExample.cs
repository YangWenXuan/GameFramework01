using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.Util;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;

namespace Com.TheFallenGames.OSA.Demos.ContentSizeFitter
{
	/// <summary>
	/// <para>The prefabhas a disabled ContentSizeFitter added, which will be enabled in <see cref="UpdateViewsHolder(MyItemViewsHolder)"/> </para> 
	/// <para>if the size was not already calculated (in a previous call), then <see cref="OSA{TParams, TItemViewsHolder}.ScheduleComputeVisibilityTwinPass(bool)"/> should be called. </para> 
	/// <para>After that, as soon as <see cref="UpdateViewsHolder(MyItemViewsHolder)"/> was called for all visible items, you'll receive a callback to <see cref="OSA{TParams, TItemViewsHolder}.OnItemHeightChangedPreTwinPass(TItemViewsHolder)"/> </para> 
	/// <para>(or <see cref="OSA{TParams, TItemViewsHolder}.OnItemWidthChangedPreTwinPass(TItemViewsHolder)"/> for horizontal ScrollRects) for each of them,</para> 
	/// <para>where you can disable the content size fitter.</para> 
	/// <para>A "Twin" <see cref="OSA{TParams, TItemViewsHolder}.ComputeVisibilityManager.ComputeVisibility(double)"/> pass is executed after the current one has finished (meaning <see cref="UpdateViewsHolder(MyItemViewsHolder)"/> was called for all visible items).</para>
	/// </summary>
	public class ContentSizeFitterExample : OSA<MyParams, MyItemViewsHolder>
	{
		public LazyDataHelper<ExampleItemModel> LazyData { get; private set; }


		#region OSA implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			LazyData = new LazyDataHelper<ExampleItemModel>(this, CreateRandomModel);

			base.Start();
		}

		/// <inheritdoc/>
		protected override MyItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new MyItemViewsHolder();
			instance.Init(_Params.itemPrefab, itemIndex);

			return instance;
		}

		/// <inheritdoc/>
		protected override void OnItemHeightChangedPreTwinPass(MyItemViewsHolder vh)
		{
			base.OnItemHeightChangedPreTwinPass(vh);

			LazyData.GetOrCreate(vh.ItemIndex).HasPendingSizeChange = false;
			vh.contentSizeFitter.enabled = false;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(MyItemViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			ExampleItemModel model = LazyData.GetOrCreate(newOrRecycled.ItemIndex);

			newOrRecycled.UpdateFromModel(model, _Params.availableIcons);

			if (newOrRecycled.contentSizeFitter.enabled)
				newOrRecycled.contentSizeFitter.enabled = false;

			if (model.HasPendingSizeChange)
			{
				//Debug.Log(newOrRecycled.ItemIndex + " pendsiz");
				// Height will be available before the next 'twin' pass, inside OnItemHeightChangedPreTwinPass() callback (see above)
				ScheduleViewsHolderForRebuild(newOrRecycled);
			}
		}

		/// <summary>
		/// <para>Because the index is shown in the title, this may lead to a content size change, so mark the ViewsHolder to have its size recalculated</para>
		/// <para>For more info, see <see cref="OSA{TParams, TItemViewsHolder}.OnItemIndexChangedDueInsertOrRemove(TItemViewsHolder, int, bool, int)"/> </para>
		/// </summary>
		protected override void OnItemIndexChangedDueInsertOrRemove(MyItemViewsHolder shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{
			base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

			shiftedViewsHolder.UpdateTitleByItemIndex(LazyData.GetOrCreate(shiftedViewsHolder.ItemIndex));
			ScheduleViewsHolderForRebuild(shiftedViewsHolder);
		}
		/// <inheritdoc/>
		protected override void RebuildLayoutDueToScrollViewSizeChange()
		{
			// Invalidate the last sizes so that they'll be re-calculated
			foreach (var model in LazyData.GetEnumerableForExistingItems())
				model.HasPendingSizeChange = true;

			base.RebuildLayoutDueToScrollViewSizeChange();
		}
		#endregion


		public ExampleItemModel CreateRandomModel(int itemIdex)
		{
			return new ExampleItemModel()
			{
				//Title = LOREM_IPSUM.Substring(400),
				Title = DemosUtil.GetRandomTextBody((DemosUtil.LOREM_IPSUM.Length) / 50 + 1, DemosUtil.LOREM_IPSUM.Length / 2),
				IconIndex = UnityEngine.Random.Range(0, _Params.availableIcons.Length)
			};
		}

		// Shorthand
		void ScheduleViewsHolderForRebuild(MyItemViewsHolder vh)
		{
			vh.MarkForRebuild(); // will enable the content size fitter
			ScheduleComputeVisibilityTwinPass(_Params.freezeContentEndEdgeOnCountChange);
		}
	}


	public class ExampleItemModel
	{
		/// <summary><see cref="HasPendingSizeChange"/> is set to true each time this property changes</summary>
		public string Title
		{
			get { return _Title; }
			set
			{
				if (_Title != value)
				{
					_Title = value;
					HasPendingSizeChange = true;
				}
			}
		}

		public int IconIndex { get; set; }

		/// <summary>This will be true when the item size may have changed and the ContentSizeFitter component needs to be updated</summary>
		public bool HasPendingSizeChange { get; set; }

		string _Title;

		public ExampleItemModel()
		{
			// By default, the model's size is unknown, so mark it for size re-calculation
			HasPendingSizeChange = true;
		}
	}


	// This in almost all cases will contain the prefab and the list of models
	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		public Texture2D[] availableIcons; // used to randomly generate models;


		[NonSerialized]
		public bool freezeContentEndEdgeOnCountChange;
	}


	/// <summary>The ContentSizeFitter should be attached to the item itself</summary>
	public class MyItemViewsHolder : BaseItemViewsHolder
	{
		public Text titleText;
		public RawImage icon1Image;

		public UnityEngine.UI.ContentSizeFitter contentSizeFitter { get; private set; }


		public override void CollectViews()
		{
			base.CollectViews();

			contentSizeFitter = root.GetComponent<UnityEngine.UI.ContentSizeFitter>();
			contentSizeFitter.enabled = false; // the content size fitter should not be enabled during normal lifecycle, only in the "Twin" pass frame
			root.GetComponentAtPath("TitlePanel/TitleText", out titleText);
			root.GetComponentAtPath("Icon1Image", out icon1Image);
		}

		public override void MarkForRebuild()
		{
			base.MarkForRebuild();
			if (contentSizeFitter)
				contentSizeFitter.enabled = true;
		}

		/// <summary>Utility getting rid of the need of manually writing assignments</summary>
		public void UpdateFromModel(ExampleItemModel model, Texture2D[] availableIcons)
		{
			UpdateTitleByItemIndex(model);
			var tex = availableIcons[model.IconIndex];
			if (icon1Image.texture != tex)
				icon1Image.texture = tex;
		}

		public void UpdateTitleByItemIndex(ExampleItemModel model)
		{
			string title = "[#" + ItemIndex + "] " + model.Title;
			if (titleText.text != title)
				titleText.text = title;
		}
	}
}
