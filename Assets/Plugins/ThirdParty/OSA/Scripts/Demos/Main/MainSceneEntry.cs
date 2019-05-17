using UnityEngine.UI;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using Com.TheFallenGames.OSA.Demos.Common.CommandPanels;

namespace Com.TheFallenGames.OSA.Demos.Main
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class MainSceneEntry : SceneEntry<MainExample, MyParams, ClientItemViewsHolder>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(
				_Adapters,
				true, true, true,
				false
			);

			AddLoadNonOptimizedExampleButton();

			var scrollToAndResizeSetting = _Drawer.AddButtonWithInputPanel("ScrollTo & Resize");
			scrollToAndResizeSetting.button.onClick.AddListener(() =>
			{
				int location = scrollToAndResizeSetting.InputFieldValueAsInt;
				if (location < 0)
					return;

				_Drawer.RequestSmoothScrollTo(
					location,
					() =>
					{
						foreach (var a in _Adapters) // using foreach because can't access GetItemViewsHolderIfVisible via IOSA
						{
							var vh = a.GetItemViewsHolderIfVisible(location);
							if (vh != null && vh.expandCollapseComponent != null)
								vh.expandCollapseComponent.OnClicked();
						}
					}
				);
			});
			scrollToAndResizeSetting.transform.SetSiblingIndex(4);

			_Drawer.freezeItemEndEdgeToggle.onValueChanged.AddListener(OnFreezeItemEndEdgeToggleValueChanged);
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			OnFreezeItemEndEdgeToggleValueChanged(_Drawer.freezeItemEndEdgeToggle.isOn);

			// Initially set the number of items to the number in the input field
			_Drawer.RequestChangeItemCountToSpecified();
		}

		#region events from DrawerCommandPanel
		void OnFreezeItemEndEdgeToggleValueChanged(bool isOn)
		{
			foreach (var adapter in _Adapters)
				adapter.Parameters.freezeItemEndEdgeWhenResizing = isOn;
		}

		protected override void OnAddItemRequested(MainExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			adapter.LazyData.InsertItems(index, 1, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnRemoveItemRequested(MainExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			if (index < adapter.LazyData.Count)
				adapter.LazyData.RemoveItems(index, 1, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnItemCountChangeRequested(MainExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			adapter.LazyData.ResetItems(count, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		#endregion


		void AddLoadNonOptimizedExampleButton()
		{
			var buttons = _Drawer.AddButtonsWithOptionalInputPanel("Compare to classic ScrollView");
			buttons.button1.gameObject.AddComponent<LoadSceneOnClick>().sceneName = "non_optimized";
			buttons.button1.image.color = _Drawer.backButtonBehavior.GetComponent<Image>().color;
			var backButtonText = _Drawer.backButtonBehavior.GetComponentInChildren<Text>();
			var loadNonOptimizedButtonText = buttons.button1.GetComponentInChildren<Text>();
			loadNonOptimizedButtonText.font = backButtonText.font;
			loadNonOptimizedButtonText.resizeTextForBestFit = backButtonText.resizeTextForBestFit;
			loadNonOptimizedButtonText.fontStyle = backButtonText.fontStyle;
			loadNonOptimizedButtonText.fontSize = backButtonText.fontSize;
			buttons.transform.SetAsFirstSibling();
		}
	}
}
