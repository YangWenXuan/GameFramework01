using System;
using System.Collections;
using UnityEngine;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;

namespace Com.TheFallenGames.OSA.Demos.Simple
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class SimpleSceneEntry : SceneEntry<SimpleExample, MyParams, MyItemViewsHolder>
	{
		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters);
			_Drawer.serverDelaySetting.inputField.text = Mathf.Max(0, _Adapters[0].Parameters.initialSimulatedServerDelay) + "";
			_Drawer.galleryEffectSetting.slider.value = 0f;

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
		protected override void OnAddItemRequested(SimpleExample adapter, int index)
		{
			base.OnAddItemRequested(adapter, index);

			_Adapters[0].LazyData.InsertItems(index, 1, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnRemoveItemRequested(SimpleExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			if (_Adapters[0].LazyData.Count == 0)
				return;

			_Adapters[0].LazyData.RemoveItems(index, 1, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnItemCountChangeRequested(SimpleExample adapter, int newCount)
		{
			base.OnItemCountChangeRequested(adapter, newCount);

			StartCoroutine(FetchItemModelsFromServer(newCount, () => OnReceivedNewModels(adapter, newCount)));
		}
		void OnFreezeItemEndEdgeToggleValueChanged(bool isOn)
		{
			_Adapters[0].Parameters.freezeItemEndEdgeWhenResizing = isOn;
		}
		#endregion

		IEnumerator FetchItemModelsFromServer(int count, Action onDone)
		{
			// Simulating server delay
			yield return new WaitForSeconds(_Drawer.serverDelaySetting.InputFieldValueAsInt);

			onDone();
		}

		void OnReceivedNewModels(SimpleExample adapter, int newCount)
		{
			adapter.LazyData.ResetItems(newCount, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
	}
}
