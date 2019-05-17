using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using Com.TheFallenGames.OSA.Util.IO;
using Com.TheFallenGames.OSA.DataHelpers;
using Com.TheFallenGames.OSA.Demos.Common;

namespace Com.TheFallenGames.OSA.Demos.Grid
{
    /// <summary>
    /// Implementation demonstrating the usage of a <see cref="GridAdapter{TParams, TCellVH}"/> for a simple gallery of remote images downloaded with a <see cref="SimpleImageDownloader"/>.
    /// </summary>
    public class GridExample : GridAdapter<GridParams, MyCellViewsHolder>//, ILazyListSimpleDataManager<BasicModel>
	{
		public LazyDataHelper<BasicModel> LazyData { get; private set; }

		public bool freezeContentEndEdgeOnCountChange;

		#region GridAdapter implementation
		/// <inheritdoc/>
		protected override void Start()
		{
			LazyData = new LazyDataHelper<BasicModel>(this, CreateRandomModel);

			base.Start();
		}

		/// <seealso cref="GridAdapter{TParams, TCellVH}.Refresh(bool, bool)"/>
		public override void Refresh(bool contentPanelEndEdgeStationary = false /*ignored*/, bool keepVelocity = false)
		{
			_CellsCount = LazyData.Count;
			base.Refresh(freezeContentEndEdgeOnCountChange, keepVelocity);
		}

		///// </inheritdoc>
		//protected override CellGroupViewsHolder<MyCellViewsHolder> CreateViewsHolder(int itemIndex)
		//{
		//	var groupVH = base.CreateViewsHolder(itemIndex);

		//	for (int i = 0; i < groupVH.ContainingCellViewsHolders.Length; i++)
		//	{
		//		var cellVH = groupVH.ContainingCellViewsHolders[i];
		//		cellVH.flexibleHeightToggle.onValueChanged.AddListener(_ => OnFlexibleHeightToggledOnCell(cellVH));
		//	}

		//	return groupVH;
		//}

		/// <summary> Called when a cell becomes visible </summary>
		/// <param name="holder"> use viewsHolder.ItemIndexto find your corresponding model and feed data into its views</param>
		protected override void UpdateCellViewsHolder(MyCellViewsHolder holder)
		{
			var model = LazyData.GetOrCreate(holder.ItemIndex);

			holder.title.text = "Loading";
			holder.overlayImage.color = Color.white;
			int itemIndexAtRequest = holder.ItemIndex;
			var imageURLAtRequest = model.imageURL;
			holder.iconRemoteImageBehaviour.Load(imageURLAtRequest, true, (fromCache, success) => {
				if (!IsRequestStillValid(holder.ItemIndex, itemIndexAtRequest, imageURLAtRequest))
					return;

				holder.overlayImage.CrossFadeAlpha(0f, .5f, false);
				holder.title.text = model.title;
			});
			//viewsHolder.flexibleHeightToggle.isOn = model.flexibleHeight;
			//viewsHolder.UpdateFlexibleHeightFromToggleState();
		}
		#endregion

		//void OnFlexibleHeightToggledOnCell(MyCellViewsHolder cellVH)
		//{
		//	// Update the model this cell is representing
		//	var model = _Data[cellVH.ItemIndex];
		//	model.flexibleHeight = cellVH.flexibleHeightToggle.isOn;
		//	cellVH.UpdateFlexibleHeightFromToggleState();
		//}

		bool IsRequestStillValid(int itemIndex, int itemIdexAtRequest, string imageURLAtRequest)
		{
			return
				_CellsCount > itemIndex // be sure the index still points to a valid model
				&& itemIdexAtRequest == itemIndex // be sure the view's associated model index is the same (i.e. the viewsHolder wasn't re-used)
				&& imageURLAtRequest == LazyData.GetOrCreate(itemIndex).imageURL; // be sure the model at that index is the same (could have changed if ChangeItemCountTo would've been called meanwhile)
		}

		BasicModel CreateRandomModel(int index)
		{
			return new BasicModel()
			{
				title = index + "",
				imageURL = DemosUtil.GetRandomSmallImageURL()
			};
		}
	}


	public class BasicModel
	{
		public string title;
		public bool flexibleHeight;
		public string imageURL;
	}


	/// <summary>All views holders used with GridAdapter should inherit from <see cref="CellViewsHolder"/></summary>
	public class MyCellViewsHolder : CellViewsHolder
	{
		public RemoteImageBehaviour iconRemoteImageBehaviour;
		public Image loadingProgress, overlayImage;
		public Text title;
		//public Toggle flexibleHeightToggle;


		public override void CollectViews()
		{
			base.CollectViews();

			views.GetComponentAtPath("IconRawImage", out iconRemoteImageBehaviour);
			views.GetComponentAtPath("OverlayImage", out overlayImage);
			views.GetComponentAtPath("LoadingProgressImage", out loadingProgress);
			views.GetComponentAtPath("TitleText", out title);
			//views.GetComponentAtPath("FlexibleHeightToggle", out flexibleHeightToggle);
		}

		//public void UpdateFlexibleHeightFromToggleState()
		//{
		//	rootLayoutElement.flexibleHeight = flexibleHeightToggle.isOn ? 1f : -1f;
		//}
	}
}
