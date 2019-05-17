using UnityEngine;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using Com.TheFallenGames.OSA.Demos.MultiplePrefabs.Models;
using Com.TheFallenGames.OSA.Demos.MultiplePrefabs.ViewsHolders;

namespace Com.TheFallenGames.OSA.Demos.MultiplePrefabs
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class MultiplePrefabsSceneEntry : SceneEntry<MultiplePrefabsExample, MyParams, BaseVH>
	{
		int _NextID = 0;

		/// <summary>Shows the average of all <see cref="BidirectionalModel.value"/>s in all models of type <see cref="BidirectionalModel"/></summary>
		public Text averageValuesInModelsText;


		/// <inheritdoc/>
		protected override void Update()
		{
			base.Update();

			if (_Adapters == null || _Adapters.Length == 0)
				return;

			var adapter = _Adapters[0];

			if (adapter == null || !adapter.IsInitialized || adapter.Data == null)
			{
				averageValuesInModelsText.text = "0";
				return;
			}

			if (adapter.Data.Count > 10000)
			{
				averageValuesInModelsText.text = "too much data";
				averageValuesInModelsText.color = Color.red * .5f;
				return;
			}
			averageValuesInModelsText.color = Color.white;

			// Keeping the update rate the smaller the bigger the data set. Then, clamped between 1 and 60. 
			// This way, the performance of computing the average stays relatively constant, regardless of the data set
			float frameStep = (adapter.Data.Count / 100000f);
			int frameStepInt = Mathf.Min(60, Mathf.Max(1, (int)(frameStep * 60)));

			if (Time.frameCount % frameStepInt == 0)
			{
				float avg = 0f;
				int bidiNum = 0;
				foreach (var model in adapter.Data)
				{
					var asBidi = model as BidirectionalModel;
					if (asBidi != null)
					{
						++bidiNum;
						avg += asBidi.value;
					}
				}
				averageValuesInModelsText.text = (avg / bidiNum).ToString("0.000");
			}
		}


		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, true, true,
				false // no server delay command
			);
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
		void OnFreezeItemEndEdgeToggleValueChanged(bool isOn)
		{
			foreach (var adapter in _Adapters)
				adapter.Parameters.freezeItemEndEdgeWhenResizing = isOn;
		}

		protected override void OnAddItemRequested(MultiplePrefabsExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			int id = _NextID++;
			adapter.Data.InsertOne(index, CreateRandomModel(id), _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnRemoveItemRequested(MultiplePrefabsExample adapter, int index)
		{
			base.OnRemoveItemRequested(adapter, index);

			if (adapter.Data.Count == 0)
				return;

			adapter.Data.RemoveOne(index, _Drawer.freezeContentEndEdgeToggle.isOn);
		}
		protected override void OnItemCountChangeRequested(MultiplePrefabsExample adapter, int count)
		{
			base.OnItemCountChangeRequested(adapter, count);

			// Generating some random models
			var newModels = new BaseModel[count];
			for (int i = 0; i < count; ++i)
				newModels[i] = CreateRandomModel(i);

			adapter.Data.List.Clear();
			adapter.Data.List.AddRange(newModels);
			adapter.Data.NotifyListChangedExternally(_Drawer.freezeContentEndEdgeToggle.isOn);
		}
		#endregion


		BaseModel CreateRandomModel(int id)
		{
			BaseModel model;
			Rect prefabRect;
			float initialSize;
			var parameters = _Adapters[0].Parameters;
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				prefabRect = parameters.expandablePrefab.rect;
				initialSize = parameters.IsHorizontal ? prefabRect.width : prefabRect.height;
				model = new ExpandableModel() { imageURL = DemosUtil.GetRandomSmallImageURL(), nonExpandedSize = initialSize };
			}
			else
			{
				prefabRect = parameters.bidirectionalPrefab.rect;
				initialSize = parameters.IsHorizontal ? prefabRect.width : prefabRect.height;
				model = new BidirectionalModel() { value = UnityEngine.Random.Range(-5f, 5f) };
			}
			model.id = id;

			return model;
		}
	}
}
