using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using frame8.Logic.Misc.Other;
using frame8.Logic.Misc.Other.Extensions;
using frame8.Logic.Misc.Visual.UI;

namespace Com.TheFallenGames.OSA.Core
{
	/// <summary>
	/// <para>Old name: ScrollRectItemsAdapter (renamed in 3.0 to SRIA, and in v4.1 to OSA as a final name - short for Optimized ScrollView Adapter)</para>
	/// <para>Base abstract component that you need to extend in order to provide an implementation for <see cref="CreateViewsHolder(int)"/> and <see cref="UpdateViewsHolder(TItemViewsHolder)"/>. 
	/// Should be attached instead of the Unity's ScrollRect component.
	/// Any views holder should extend <see cref="BaseItemViewsHolder"/>, so you can provide it as the generic parameter <typeparamref name="TItemViewsHolder"/> when implementing OSA.
	/// Extending <see cref="BaseParams"/> is optional. Based on your needs. Provide it as generic parameter <typeparamref name="TParams"/> when implementing OSA</para>
	/// <para>How it works, in a nutshell (it's recommended to manually go through the example code in order to fully understand the mechanism):</para>
	/// <para>1. create your own implementation of <see cref="BaseItemViewsHolder"/>, let's name it MyItemViewsHolder</para>
	/// <para>2. create your own implementation of <see cref="BaseParams"/> (if needed), let's name it MyParams</para>
	/// <para>3. create your own implementation of OSA&lt;MyParams, MyItemViewsHolder&gt;, let's name it MyScrollViewAdapter</para>
	/// <para>4. instantiate MyScrollViewAdapter</para>
	/// <para>5. call MyScrollViewAdapter.ResetItems(int) once (and any time your dataset is changed) and the following things will happen:</para>
	/// <para>    5.1. <see cref="CollectItemsSizes(ItemCountChangeMode, int, int, ItemsDescriptor)"/> will be called (which you can optionally implement to provide your own sizes, if known beforehand)</para>
	/// <para>    5.2. <see cref="CreateViewsHolder(int)"/> will be called for enough items to fill the viewport. Once a ViewsHolder is created, it'll be re-used when it goes off-viewport </para>
	/// <para>          - newOrRecycledViewsHolder.root will be null, so you need to instantiate your prefab, assign it and call newOrRecycledViewsHolder.CollectViews(). Alternatively, you can call its <see cref="AbstractViewsHolder.Init(GameObject, int, bool, bool)"/> method, which can do a lot of things for you, mainly instantiate the prefab and (if you want) call CollectViews() for you</para>
	/// <para>          - after creation, only <see cref="UpdateViewsHolder(TItemViewsHolder)"/> will be called for it when its represented item changes and becomes visible</para>
	/// <para>    5.3. <see cref="UpdateViewsHolder(TItemViewsHolder)"/> will be called when an item is to be displayed or simply needs updating:</para>
	/// <para>        - use <see cref="AbstractViewsHolder.ItemIndex"/> to get the item index, so you can retrieve its associated model from your data set (most common practice is to store the data list in your Params implementation)</para>
	/// <para>        - <see cref="AbstractViewsHolder.root"/> is not null here (given the views holder was properly created in CreateViewsHolder(..)). It's assigned a valid object whose UI elements only need their values changed (common practice is to implement helper methods in the views holder that take the model and update the views themselves)</para>
	/// <para> <see cref="ResetItems(int, bool, bool)"/> is also called when the viewport's size changes (like for orientation changes on mobile or window resizing on sandalone platforms)</para>
	/// <para></para>
	/// <para> *NOTE: No LayoutGroup (vertical/horizontal/grid) on content panel are allowed, since all the layouting is delegated to this adapter</para>
	/// </summary>
	/// <typeparam name="TParams">The params type to use</typeparam>
	/// <typeparam name="TItemViewsHolder"></typeparam>
	/// <seealso cref="IOSA"/>
	/// <seealso cref="BaseParams"/>
	/// <seealso cref="BaseItemViewsHolder"/>
	public abstract partial class OSA<TParams, TItemViewsHolder> : MonoBehaviour, IOSA
	where TParams : BaseParams
	where TItemViewsHolder : BaseItemViewsHolder
	{
		#region Configuration
		/// <summary>Parameters displayed in inspector, which can be tweaked based on your needs</summary>
		[SerializeField]
		protected TParams _Params = null;
		#endregion

		#region IScrollRectProxy events & properties implementaion
		/// <inheritdoc/>
		public event Action<double> ScrollPositionChanged;
		/// <summary>Becomes true after <see cref="OSA{TParams, TItemViewsHolder}.Init"/> and false in <see cref="OSA{TParams, TItemViewsHolder}.Dispose"/></summary>
		public bool IsInitialized { get { return _Initialized; } }
		/// <inheritdoc/>
		public Vector2 Velocity { get { return _Velocity; }
			set
			{
				_Velocity = value;
				var magToMaxVelocity = _Velocity.magnitude / _Params.effects.maxSpeed;
				if (magToMaxVelocity > 1f)
					_Velocity /= magToMaxVelocity;
			}
		}
		// Only one of IsHorizontal and IsHorizontal should be true at any moment
		public bool IsHorizontal { get { return _Params.IsHorizontal; } }
		public bool IsVertical { get { return !_Params.IsHorizontal; } }
		public RectTransform Content { get { return _Params.Content; } }
		public RectTransform Viewport { get { return _Params.Viewport; } }
		#endregion

		#region IOSA events & properties implementaion
		/// <summary> Fired at the end of an <see cref="Init"/> call, which is usually done in Start(), if you're not calling it manually</summary>
		public virtual event Action Initialized;
		/// <summary> Fired when the item count changes or the views are refreshed (more exactly, after each <see cref="ChangeItemsCount(ItemCountChangeMode, int, int, bool, bool)"/> call). Params are (1st=prevCount, 2nd=newCount)</summary>
		public virtual event Action<int, int> ItemsRefreshed;
		/// <summary>The adapter's params that can be retrieved from anywhere through an <see cref="IOSA"/> reference to this adapter</summary>
		public BaseParams BaseParameters { get { return Parameters; } }
		/// <summary>Simply casts the adapter to a MonoBehaviour and returns it. Guaranteed to be non-null, because <see cref="OSA{TParams, TItemViewsHolder}"/> implements MonoBehaviour</summary>
		public MonoBehaviour AsMonoBehaviour { get { return this; } }
		[Obsolete("Use IScrollRectProxy.GetContentSizeToViewportRatio() (extension method)", true)]
		public double ContentVirtualSizeToViewportRatio { get { throw new NotImplementedException(); } }
		/// <inheritdoc/>
		public double ContentVirtualInsetFromViewportStart { get { return _InternalState.ctVirtualInsetFromVPS_Cached; } }
		/// <inheritdoc/>
		public double ContentVirtualInsetFromViewportEnd { get { return _InternalState.CTVirtualInsetFromVPE_Cached; } }
		/// <summary> The number of currently visible items (views holders). Can be used to iterate through all of them using <see cref="GetItemViewsHolder(int)"/></summary>
		public int VisibleItemsCount { get { return _VisibleItemsCount; } }
		/// <summary> The number of items that are cached and waiting to be recycled </summary>
		public int RecyclableItemsCount { get { return _RecyclableItems.Count; } }
		/// <summary> Wether the scrollrect is currently dragged (i.e. the finger/mouse holds onto it) </summary>
		public bool IsDragging { get { return _IsDragging; } }
		/// <summary> Whether <see cref="InsertItems(int, int, bool, bool)"/> is supported. If false, <see cref="ResetItems(int, bool, bool)"/> should be used instead</summary>
		public virtual bool InsertAtIndexSupported { get { return true; } }
		/// <summary> Whether <see cref="RemoveItems(int, int, bool, bool)"/> is supported. If false, <see cref="ResetItems(int, bool, bool)"/> should be used instead</summary>
		public virtual bool RemoveFromIndexSupported { get { return true; } }
		#endregion

		/// <summary>The adapter's parameters as seen in inspector</summary>
		public TParams Parameters { get { return _Params; } }

		protected List<TItemViewsHolder> _VisibleItems;
		protected int _VisibleItemsCount;
		protected List<TItemViewsHolder> _RecyclableItems = new List<TItemViewsHolder>();
		//protected List<TItemViewsHolder> _StolenItems = new List<TItemViewsHolder>();
		protected NestingManager _NestingManager;

		InternalState _InternalState;
        ItemsDescriptor _ItemsDesc;
		Coroutine _SmoothScrollCoroutine;
		bool _SkipComputeVisibilityInUpdateOrOnScroll;
		float _PrevGalleryEffectAmount;
		double _AVGVisibleItemsCount; // never reset
		bool _Initialized, _SkipInitializationChecks;
		Vector2 _Velocity, _VelocityToAddOnDragEnd;
		bool _IsDragging;
		Vector2 _OnInitializePotentialDragPointerPosInCTSpace;
		Vector2 _LastOnDragPointerPosInCTSpace;
		float _LastOnDragTime;
		ComputeVisibilityManager _ComputeVisibilityManager;
		ReleaseFromPullManager _ReleaseFromPull;


		#region Unity methods
		protected virtual void Awake() { }
		protected virtual void Start() { Init(); }
		protected virtual void Update() { MyUpdate(); }
		protected virtual void LateUpdate() { MyLateUpdate(); }
		protected virtual void OnDestroy() { Dispose(); }
		#endregion

		#region IScrollRectProxy methods implementaion
		/// <summary>Floating point rounding errors occur the bigger the content size, but generally it's accurrate enough</summary>
		/// <seealso cref="IScrollRectProxy.SetNormalizedPosition"/>
		public void SetNormalizedPosition(double normalizedPosition)
		{
			double abstractNormPos = _Params.IsHorizontal ? 1d - normalizedPosition : normalizedPosition;
			bool _;
			SetVirtualAbstractNormalizedScrollPosition(abstractNormPos, true, out _);
		}

		/// <summary>Floating point rounding errors occur the bigger the content size, but generally it's accurrate enough</summary>
		/// <seealso cref="IScrollRectProxy.GetNormalizedPosition"/>
		public double GetNormalizedPosition()
		{
			double abstractVirtNormPos = _InternalState.GetVirtualAbstractNormalizedScrollPosition();
			return _Params.IsHorizontal ? 1d - abstractVirtNormPos : abstractVirtNormPos;
		}

		/// <summary>Floating point rounding errors occur the bigger the content size, but generally it's accurrate enough. Returns <see cref="float.MaxValue"/> if the (virtual) content size is bigger than <see cref="float.MaxValue"/></summary>
		/// <seealso cref="IScrollRectProxy.GetContentSize"/>
		public double GetContentSize() { return Math.Min(_InternalState.ctVirtualSize, float.MaxValue); }

		/// <summary>Self-explanatory</summary>
		/// <seealso cref="IScrollRectProxy.GetViewportSize"/>
		public double GetViewportSize() { return _InternalState.vpSize; }
		#endregion

		#region Unity UI events callbacks
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (_Params.ForwardDragToParents)
				_NestingManager.OnInitializePotentialDrag(eventData);

			if (!_Params.DragEnabled)
				return;

			if (!_Params.TransientSpeedBetweenDrags)
			{
				StopMovement();
				_VelocityToAddOnDragEnd = Vector2.zero;
			}

			_OnInitializePotentialDragPointerPosInCTSpace = _InternalState.GetPointerPositionInCTSpace(eventData);

			_ReleaseFromPull.inProgress = false;
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;
			
			if (_Params.ForwardDragToParents)
			{
				_NestingManager.OnBeginDrag(eventData);
				if (_NestingManager.CurrentDragCapturedByParent)
					return;
			}

			if (!isActiveAndEnabled)
				return;

			if (!_Params.DragEnabled)
				return;

			// Hotfix: drags that last only 1 frame did not drag/speed the content
			//_LastOnDragPointerPosInCTSpace = _InternalState.GetPointerPositionInCTSpace(eventData);
			_LastOnDragPointerPosInCTSpace = _OnInitializePotentialDragPointerPosInCTSpace;
			_VelocityToAddOnDragEnd = _Velocity;
			_Velocity = Vector2.zero;
			_IsDragging = true;
			//_LastDragAmount = 0d;
			_LastOnDragTime = Time.time;
			//_PosOnBeginDrag = _PosInLastOnDrag = _InternalState.ctVirtualInsetFromVPS_Cached;
			_ReleaseFromPull.inProgress = false;
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			//Debug.Log("OnDrag " + eventData.delta);

			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (_Params.ForwardDragToParents && _NestingManager.CurrentDragCapturedByParent)
			{
				_NestingManager.OnDrag(eventData);
				return;
			}

			if (!isActiveAndEnabled)
				return;

			if (!_Params.DragEnabled)
				return;

			//var deltaFromStartDragWorldSpace = (eventData.position - _StartDragPointerPosWorldSpace);
			//var deltaFromStartDragContentSpace = _Params.Content.InverseTransformVector(deltaFromStartDragWorldSpace);

			var pointerPosInCTSpace = _InternalState.GetPointerPositionInCTSpace(eventData);
			var ctSpaceDelta = pointerPosInCTSpace - _LastOnDragPointerPosInCTSpace;
			double abstrDeltaInCTSpace = _InternalState.GetCTAbstractSpaceVectorLongitudinalComponentFromCTSpaceVector(ctSpaceDelta);
			//Debug.Log(
			//	"\n" + name + " isDragging " + _IsDragging +
			//	"\nlastOnDragPointerPosInCTSpace " + _LastOnDragPointerPosInCTSpace +
			//	"\npressWorld " + eventData.pressPosition + ", pressCTSpace " + _OnInitializePotentialDragPointerPosInCTSpace +
			//	"\npointerPosWorld " + eventData.position + ", pointerPosCTSpace " + pointerPosInCTSpace +
			//	"\ncurDeltaWorld " + eventData.delta + ", curDeltaCTSpace " + ctSpaceDelta +
			//	//"\nInverseTransformVector(deltaWorldSpace) " + deltaInContentSpace +
			//	"\nabstrDeltaInCTSpace " + abstrDeltaInCTSpace.ToString("#######.###"));

			var alllowOutOfBoundsMode = 
				(_Params.effects.elasticMovement || _Params.effects.loopItems && _InternalState.VirtualScrollableArea > 0d) ? 
					AllowContentOutsideBoundsMode.ALLOW 
					: AllowContentOutsideBoundsMode.DO_NOT_ALLOW;

			//double ctInsetBeforeDrag = _InternalState.ctVirtualInsetFromVPS_Cached;
			//bool looped = 
			Drag(abstrDeltaInCTSpace, alllowOutOfBoundsMode, true);
			//double ctInsetAfterDrag = _InternalState.ctVirtualInsetFromVPS_Cached;

			//if (looped)
			//{
			//	// Visualization for positive scroll: 
			//	// * VSA = 100, _PosOnBeginDrag = ctInset = -10
			//	// * dragging positively for a few frames, _PosInLastOnDrag increases to near 0
			//	// * it loops, 
			//	// * ctInset is now -100
			//	// * _PosInLastOnDrag is probably near 0 (since it looped)
			//	// * dInset = ctInset - _PosInLastOnDrag = -100 - 0 = -100 => whereas the perceived dInset should be exactly <abstrDeltaInCTSpace>,
			//	// which is probably something like 2.3, so we need to modify _PosInLastOnDrag to obtain that. 
			//	// _PosOnBeginDrag should also be modified the same way, for the same reason

			//	double ctInsetDelta = ctInsetAfterDrag - ctInsetBeforeDrag;
			//	_PosInLastOnDrag += ctInsetDelta;
			//	_PosOnBeginDrag += ctInsetDelta;
			//}

			// TODO maybe inertia should only be derived from the eventData and from nowhere else. 
			// This frees us from keeping track from looping, but it also seems a more natural way of calculating it (directly from the source)
			if (_Params.effects.inertia)
			{
				float dtStrictlyPositive = Time.time - _LastOnDragTime;
				if (dtStrictlyPositive == 0f)
					dtStrictlyPositive = Time.deltaTime;

				//double dInset = ctInsetAfterDrag - _PosInLastOnDrag;
				double dInset = abstrDeltaInCTSpace;
				double velocityThisFrameAbstr = dInset / dtStrictlyPositive;
				double velocityThisFrame = velocityThisFrameAbstr * _InternalState.hor1_vertMinus1;
				float velocityBef = _Velocity[_InternalState.hor0_vert1];
				if (Math.Abs(dInset) < 1d)
				//if (Math.Abs(velocityThisFrame) < 1d)
						velocityThisFrame = 0d;
				_Velocity[_InternalState.hor0_vert1] = Mathf.Clamp((float)((velocityBef + velocityThisFrame) / 2d), -_Params.effects.maxSpeed, _Params.effects.maxSpeed);
			}
			//_LastDragAmount = abstrDeltaInCTSpace;
			_LastOnDragTime = Time.time;
			_LastOnDragPointerPosInCTSpace = pointerPosInCTSpace;
			//_PosInLastOnDrag = ctInsetAfterDrag;

		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (_Params.ForwardDragToParents && _NestingManager.CurrentDragCapturedByParent)
			{
				_NestingManager.OnEndDrag(eventData);
				return;
			}

			_IsDragging = false;

			if (!isActiveAndEnabled)
				return;

			if (!_Params.DragEnabled)
				return;

			if (_Params.effects.inertia)
			{
				var ctSpaceDeltaFromPressPosition = _InternalState.GetPointerPositionInCTSpace(eventData) - _OnInitializePotentialDragPointerPosInCTSpace;
				double abstrDeltaFromPressPositionInCTSpace = _InternalState.GetCTAbstractSpaceVectorLongitudinalComponentFromCTSpaceVector(ctSpaceDeltaFromPressPosition);

				//double distFromInitialPos = Math.Abs(_PosOnBeginDrag - _InternalState.ctVirtualInsetFromVPS_Cached);
				double distFromInitialPos = Math.Abs(abstrDeltaFromPressPositionInCTSpace);
				if (distFromInitialPos < 3d || eventData.delta.magnitude < 3d)
				{
					_Velocity[_InternalState.hor0_vert1] = 0f;
					return;
				}

				float velocityToAdd = _VelocityToAddOnDragEnd[_InternalState.hor0_vert1];
				float curVelocity = _Velocity[_InternalState.hor0_vert1];

				if (Mathf.Sign(velocityToAdd) == Mathf.Sign(curVelocity))
					_Velocity[_InternalState.hor0_vert1] = Mathf.Clamp(curVelocity + velocityToAdd, -_Params.effects.maxSpeed, _Params.effects.maxSpeed);
			}
		}

		public virtual void OnScroll(PointerEventData eventData)
		{
			if (!isActiveAndEnabled)
				return;

			if (_IsDragging)
				return;

			if (!_Params.ScrollEnabled)
				return;

			//if (_ReleaseFromPull.inProgress)
			//	return;

			Vector2 scrollDelta = eventData.scrollDelta;
			double rawDelta;
			if (Mathf.Abs(scrollDelta.x) > Mathf.Abs(scrollDelta.y))
				rawDelta = -scrollDelta.x;
			else
				rawDelta = scrollDelta.y;

			StopMovement();
			rawDelta *= _Params.ScrollSensivity;
			Drag(rawDelta, AllowContentOutsideBoundsMode.DO_NOT_ALLOW, true);
		}
		#endregion

		/// <summary>
		/// <para>Initialize the adapter. This is automatically called in Start(), but it may also be called manually, if for some reason you implement Start() and don't want to call base.Start()</para>
		/// <para>Will call Canvas.ForceUpdateCanvases(), Params.InitIfNeeded(), will initialize the internal state and will change the items count to 0</para>
		/// <para>IMPORTANT: Do not call it in Awake(), OnEnable(), OnDisable(). OnStart() is the best place to do it.</para>
		/// </summary>
		public void Init()
		{
			if (!enabled || !gameObject.activeInHierarchy)
				throw new UnityException("OSA: Init should only be called when the GameObject is active in hierarchy and the OSA component is enabled");

			_ReleaseFromPull = new ReleaseFromPullManager(this);
			if (!_Params.Content)
				throw new UnityException("OSA: Content cannot be null");
			_Params.Content.MatchParentSize(true);

			//if (_Params.Viewport.parent)
			//	DemosUtil.RebuildLayoutImmediateCompat(_Params.Viewport.parent as RectTransform);
			//else
			//	DemosUtil.RebuildLayoutImmediateCompat(_Params.Viewport);
			Canvas.ForceUpdateCanvases();

			_Params.InitIfNeeded(this);
			if (_Params.Snapper)
				_Params.Snapper.Adapter = this;

			//Func<int, float> getSizeFn; if (_Params.IsHorizontal) getSizeFn = i => GetItemWidth(i); else getSizeFn = i => GetItemHeight(i);
			_ItemsDesc = new ItemsDescriptor(_Params.DefaultItemSize);//, _Params.DefaultItemSizeUsage == BaseParams.DefaultSizeUsage.PLACEHOLDER_SIZE);
            _InternalState = InternalState.CreateFromSourceParamsOrThrow(_Params, _ItemsDesc);

			_NestingManager = new NestingManager(this);
			// Commented: this should be delayed until the first OnBeginDrag, for 2 reasons, in this order of importance: 
			// 1. the child may not be attached to a parent immediately after this method returns
			// 2. performance - init only if/when needed :)
			//if (_Params.ForwardDragToParents && !_NestingManager.SearchedParentAtLeastOnce)
			//	_NestingManager.FindAndStoreNestedParent();

			_ComputeVisibilityManager = new ComputeVisibilityManager(this);

			_VisibleItems = new List<TItemViewsHolder>();
			_AVGVisibleItemsCount = 0d;

			_SkipInitializationChecks = true;
			Refresh();
			_InternalState.UpdateLastProcessedCTVirtualInsetFromVPStart();
			bool _;
			SetVirtualAbstractNormalizedScrollPosition(1d, false, out _); // scroll to start

			OnScrollPositionChanged();

			// Debug stuff
#if UNITY_EDITOR && !UNITY_WSA && !UNITY_WSA_10_0 // UNITY_WSA uses .net core, which OSADebugger is not compatible with
			var debugger = GameObject.FindObjectOfType<OSADebugger>();
			if (debugger)
				debugger.InitWithAdapter(this);
#endif

			_SkipInitializationChecks = false;
			OnInitialized();

			// The inheritors might've canceled the initialization, so we check again for it
			if (_Initialized)
			{
				if (Initialized != null)
					Initialized();
			}
		}

		/// <summary>Same as ResetItems(&lt;currentCount&gt;). <see cref="ChangeItemsCount(ItemCountChangeMode, int, int, bool, bool)"/></summary>
		public virtual void Refresh(bool contentPanelEndEdgeStationary = false, bool keepVelocity=false) { ChangeItemsCount(ItemCountChangeMode.RESET, _ItemsDesc.itemsCount, -1, contentPanelEndEdgeStationary, keepVelocity); }

		/// <summary>It clears any previously cached sizes. <see cref="ChangeItemsCount(ItemCountChangeMode, int, int, bool, bool)"/></summary>
		public virtual void ResetItems(int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{ ChangeItemsCount(ItemCountChangeMode.RESET, itemsCount, -1, contentPanelEndEdgeStationary, keepVelocity); }

		/// <summary>It preserves previously cached sizes. <see cref="ChangeItemsCount(ItemCountChangeMode, int, int, bool, bool)"/></summary>
		public virtual void InsertItems(int index, int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{ ChangeItemsCount(ItemCountChangeMode.INSERT, itemsCount, index, contentPanelEndEdgeStationary, keepVelocity); }

		/// <summary>It preserves previously cached sizes. <see cref="ChangeItemsCount(ItemCountChangeMode, int, int, bool, bool)"/></summary>
		public virtual void RemoveItems(int index, int itemsCount, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{ ChangeItemsCount(ItemCountChangeMode.REMOVE, itemsCount, index, contentPanelEndEdgeStationary, keepVelocity); }

		/// <summary>
		/// <para>Self-explanatory. See <see cref="ItemCountChangeMode"/> in order to understand how change modes differ from each other.</para>
		/// <para>Every count change operation (<see cref="Refresh(bool, bool)"/>, <see cref="InsertItems(int, int, bool, bool)"/> etc.) ultimately calls this method, 
		/// so it's a good place for example to fire a custom "ItemsChanged" event, if you need to</para>
		/// </summary>
		/// <seealso cref="OnItemIndexChangedDueInsertOrRemove(TItemViewsHolder, int, bool, int)"/>
		public virtual void ChangeItemsCount(ItemCountChangeMode changeMode, int itemsCount, int indexIfInsertingOrRemoving = -1, bool contentPanelEndEdgeStationary = false, bool keepVelocity = false)
		{ ChangeItemsCountInternal(changeMode, itemsCount, indexIfInsertingOrRemoving, contentPanelEndEdgeStationary, keepVelocity, false); }

		/// <summary>Returns the last value that was passed to <see cref="ChangeItemsCount(ItemCountChangeMode, int, int, bool, bool)"/></summary>
		public virtual int GetItemsCount() { return _ItemsDesc.itemsCount; }

		/// <summary>
		/// <para>Get the viewsHolder with a specific index in the "visible items" list.</para>
		/// <para>Example: if you pass 0, the first visible ViewsHolder will be returned (if there's any)</para>
		/// <para>Not to be mistaken to the other method 'GetItemViewsHolderIfVisible(int withItemIndex)', which uses the itemIndex, i.e. the index in the list of data models.</para>
		/// <para>Returns null if the supplied parameter is >= <see cref="VisibleItemsCount"/></para>
		/// </summary>
		/// <param name="vhIndex"> the index of the ViewsHolder in the visible items array</param>
		public TItemViewsHolder GetItemViewsHolder(int vhIndex)
		{
			if (vhIndex >= _VisibleItemsCount)
				return null;
			return _VisibleItems[vhIndex];
		}

		/// <summary>Gets the views holder representing the <paramref name="withItemIndex"/>'th item in the list of data models, if it's visible.</summary>
		/// <returns>null, if not visible</returns>
		public TItemViewsHolder GetItemViewsHolderIfVisible(int withItemIndex)
		{
			int curVisibleIndex = 0;
			int curIndexInList;
			TItemViewsHolder curItemViewsHolder;
			for (curVisibleIndex = 0; curVisibleIndex < _VisibleItemsCount; ++curVisibleIndex)
			{
				curItemViewsHolder = _VisibleItems[curVisibleIndex];
				curIndexInList = curItemViewsHolder.ItemIndex;
				// Commented: with introduction of itemIndexInView, this check is no longer useful
				//if (curIndexInList > withItemIndex) // the requested item is before the visible ones, so no viewsHolder for it
				//    break;
				if (curIndexInList == withItemIndex)
					return curItemViewsHolder;
			}

			return null;
		}

		/// <summary>Same as GetItemViewsHolderIfVisible(int withItemIndex), but searches by the root RectTransform reference, rather than the item index</summary>
		/// <param name="withRoot">RectTransform reference to the searched viw holder's root</param>
		public TItemViewsHolder GetItemViewsHolderIfVisible(RectTransform withRoot)
		{
			TItemViewsHolder curItemViewsHolder;
			for (int i = 0; i < _VisibleItemsCount; ++i)
			{
				curItemViewsHolder = _VisibleItems[i];
				if (curItemViewsHolder.root == withRoot)
					return curItemViewsHolder;
			}

			return null;
		}

		/// <summary>
		/// Converts from viewport local point to a point in range [0..1] longitudinally (in the ScrollView's orientation direction). 0=start, 1=end. 
		/// The transversal component is ignored, of course (x for vertical, y for horizontal)
		/// In english, 0=top, 1=bottom, assuming vertical ScrollView. 0=left, 1=right, if horizontal ScrollView
		/// </summary>
		/// <param name="localPositionInViewport"></param>
		/// <returns></returns>
		public float ConvertViewportLocalPointToViewportLongitudinalPointStart0End1(Vector2 localPositionInViewport)
		{
			//Debug.Log(localPositionInViewport);
			var vBottomLeftToNormPositionLocal = _Params.Viewport.ConvertLocalPointToPointNormalizedBySize(localPositionInViewport);

			//Debug.Log(vBottomLeftToPositionLocal);
			float posSelected = vBottomLeftToNormPositionLocal[_InternalState.hor0_vert1];
			if (!_Params.IsHorizontal)
				posSelected = 1f - posSelected;

			return posSelected;
		}

		/// <summary>
		/// See <see cref="GetViewsHolderClosestToViewportLongitudinalNormalizedAbstractPoint(Canvas, RectTransform, float, float, out float)"/>
		/// </summary>
		public virtual TItemViewsHolder GetViewsHolderClosestToViewportPoint(
			Canvas c, 
			RectTransform canvasRectTransform, 
			Vector2 localPositionInViewport, 
			float itemNormalizedAbstractPoint, 
			out float distance
		){
			float posSelected = ConvertViewportLocalPointToViewportLongitudinalPointStart0End1(localPositionInViewport);
			return GetViewsHolderClosestToViewportLongitudinalNormalizedAbstractPoint(c, canvasRectTransform, posSelected, itemNormalizedAbstractPoint, out distance) as TItemViewsHolder;
		}

		/// <summary>
		/// Will set <paramref name="distance"/> to <see cref="float.MaxValue"/> if no ViewsHolder is found. 
		/// The point's format is in range [0=startEdge(top or left) .. 1=endEdge (bottom or right)]
		/// The transversal component of the point is considered to be 0.5f (middle).
		/// Transversal = vertical, if horizontal ScrollView. Else, horizontal
		/// </summary>
		public virtual AbstractViewsHolder GetViewsHolderClosestToViewportLongitudinalNormalizedAbstractPoint(
			Canvas c, 
			RectTransform canvasRectTransform, 
			float viewportNormalizedAbstractPoint, 
			float itemNormalizedAbstractPoint, 
			out float distance
		){
			int h0v1 = _InternalState.hor0_vert1;
			int h1v0 = 1 - h0v1;
			Vector2 viewportPointNormalizedWithYInverted =
				new Vector2(
					viewportNormalizedAbstractPoint * h1v0 + .5f * h0v1,
					viewportNormalizedAbstractPoint * h0v1 + .5f * h1v0
				);
			Vector2 itemPointNormalizedWithYInverted =
				new Vector2(
					itemNormalizedAbstractPoint * h1v0 + .5f * h0v1,
					itemNormalizedAbstractPoint * h0v1 + .5f * h1v0
				);

			return GetViewsHolderClosestoViewportNormalizedAbastractPoint(
				_VisibleItems, 
				c, 
				canvasRectTransform, 
				viewportPointNormalizedWithYInverted, 
				itemPointNormalizedWithYInverted, 
				out distance
			);
		}

		public virtual void RemoveItemWithViewsHolder(TItemViewsHolder viewsHolder, bool stealViewsHolderInsteadOfRecycle, bool contentPanelEndEdgeStationary)
		{
			//if (_StolenItems.Contains(viewsHolder))
			//	throw new UnityException("OSA.RemoveItemWithViewsHolder: The viewsholder is in 'STOLEN' state: ItemIndex " + viewsHolder.ItemIndex + ", viewsHolder.root " + viewsHolder.root);
			
			ChangeItemsCountInternal(ItemCountChangeMode.REMOVE, 1, viewsHolder.ItemIndex, contentPanelEndEdgeStationary, false, stealViewsHolderInsteadOfRecycle);
			if (stealViewsHolderInsteadOfRecycle)
			{
				//if (!_StolenItems.Contains(viewsHolder))
				//	throw new UnityException("OSA.RemoveItemWithViewsHolder: Internal error: Not in Stolen list. ItemIndex " + viewsHolder.ItemIndex + ", viewsHolder.root " + viewsHolder.root);

				if (!viewsHolder.root)
					throw new UnityException("OSA.RemoveItemWithViewsHolder: Internal error: vh.root destroyed. ItemIndex " + viewsHolder.ItemIndex + ", viewsHolder.root " + viewsHolder.root);
			}
		}

		/// <summary>
		/// <para>Call it when a stolen ViewsHolder won't be added to the adapter and so the adapter should not be responsible for its lifecycle anymore.</para>
		/// <para>If the item was stolen, it'll be unregistered automatically </para>
		/// </summary>
		/// <param name="viewsHolder"></param>
		public virtual void InsertItemWithViewsHolder(TItemViewsHolder viewsHolder, int atIndex, bool contentPanelEndEdgeStationary)
		{
			if (atIndex < 0 || atIndex > _ItemsDesc.itemsCount)
				throw new ArgumentOutOfRangeException("atIndex", atIndex, "should be >=0 and <= itemsCount(="+_ItemsDesc.itemsCount+")");

			if (!viewsHolder.root)
				throw new ArgumentOutOfRangeException("Could not give a viewsholder that has its root destroyed/set to null");

			//int idx = _StolenItems.IndexOf(viewsHolder);
			//if (idx != -1)
			//	UnregisterStolenViewsHolderWithoutChecks(idx, false);

			ClearCachedRecyclableItems(); // this is a temporary workaround so that GetNumExcessObjects() won't throw an exception (it allows a maximum of 1 excess objects)
			_RecyclableItems.Insert(0, viewsHolder);
			ChangeItemsCountInternal(ItemCountChangeMode.INSERT, 1, atIndex, contentPanelEndEdgeStationary, false, false);
		}


		/// <summary>
		/// positite => scroll towards start
		/// negative => scroll towards end
		/// </summary>
		/// <param name="abstractDeltaInCTSpace"></param>
		/// <returns>Whether succeeded</returns>
		public virtual bool ScrollByAbstractDelta(float abstractDeltaInCTSpace)
		{
			if (!isActiveAndEnabled)
				return false;

			if (_IsDragging)
				return false;

			StopMovement();
			Drag(abstractDeltaInCTSpace, AllowContentOutsideBoundsMode.DO_NOT_ALLOW, true);

			return true;
		}

		/// <summary> 
		/// <para>By default, it aligns the ScrollView's content so that the item with <paramref name="itemIndex"/> will be at the top.</para>
		/// <para>But the two optional parameters can be used for more fine-tuning. One common use-case is to set them both at 0.5 so the item will be end up exactly in the middle of the viewport</para>
		/// </summary>
		/// <param name="itemIndex">The item with this index will be considered</param>
		/// <param name="normalizedOffsetFromViewportStart">0f=no effect; 0.5f= the item's start edge (top or left) will be at the viewport's center; 1f=the item's start edge will be exactly at the viewport's end (thus, the item will be completely invisible)</param>
		/// <param name="normalizedPositionOfItemPivotToUse">For even more fine-adjustment, you can also specify what point on the item will be used to bring it to <paramref name="normalizedOffsetFromViewportStart"/>. The same principle applies as to the <paramref name="normalizedOffsetFromViewportStart"/> parameter: 0f=start(top/left), 1f=end(bottom/right)</param>
		public virtual void ScrollTo(int itemIndex, float normalizedOffsetFromViewportStart = 0f, float normalizedPositionOfItemPivotToUse = 0f)
		{
			//if (_Params.effects.loopItems)
			//	throw new UnityException("If looping is enabled, ScrollTo is not yet available, only SmoothScrollTo, preferably with a duration bigger than 0.5 seconds");

			CancelAnimationsIfAny();

			double vsa = _InternalState.VirtualScrollableArea;
			bool ctBiggerThanVP = vsa > 0d;
			double newInset = ScrollToHelper_GetContentStartVirtualInsetFromViewportStart_Clamped(
					vsa,
					itemIndex,
					normalizedOffsetFromViewportStart,
					normalizedPositionOfItemPivotToUse
				);
			var p = new ContentSizeOrPositionChangeParams
			{
				computeVisibilityParams = _ComputeVisibilityParams_Reusable_Empty,
				fireScrollPositionChangedEvent = true,
				allowOutsideBounds = _Params.effects.loopItems && ctBiggerThanVP
			};
			bool _;
			SetContentVirtualInsetFromViewportStart(newInset, ref p, out _);

			//// The shift wasn't possible
			//if (double.IsNaN(deltaInset))
			//	return;

			//// This is a semi-hack-lazy hot-fix because when the scroll is immediate, sometimes the visibility isn't computed well
			//// Same thing is done in SmoothScrollTo if duration is 0 or close to 0
			//ComputeVisibilityForCurrentPosition(false, -.1);
			//ComputeVisibilityForCurrentPosition(true, +.1);
		}

		/// <summary> Utility to smooth scroll. Identical to <see cref="ScrollTo(int, float, float)"/> in functionality, but the scroll is animated (scroll is done gradually, throughout multiple frames) </summary>
		/// <param name="onProgress">gets the progress (0f..1f) and returns if the scrolling should continue</param>
		/// <returns>True, if no smooth scroll animation was already playing or <paramref name="overrideCurrentScrollingAnimation"/> is true</returns>
		public virtual bool SmoothScrollTo(int itemIndex, float duration, float normalizedOffsetFromViewportStart = 0f, float normalizedPositionOfItemPivotToUse = 0f, Func<float, bool> onProgress = null, bool overrideCurrentScrollingAnimation = false)
		{
			//if (_Params.effects.loopItems && duration < .5f)
			//	throw new UnityException("If looping is enabled, SmoothScrollTo best works with a duration bigger than 0.5 seconds");

			if (_SmoothScrollCoroutine != null)
			{
				if (overrideCurrentScrollingAnimation)
				{
					CancelAnimationsIfAny();

					//Debug.Log("cancel - other started");
				}
				else
					return false;
			}

			_SmoothScrollCoroutine = StartCoroutine(SmoothScrollProgressCoroutine(itemIndex, duration, normalizedOffsetFromViewportStart, normalizedPositionOfItemPivotToUse, onProgress));

			return true;
		}

		/// <summary>
		/// Similar to <see cref="SmoothScrollTo(int, float, float, float, Func{float, bool}, bool)"/> (see it for more info about the other params), 
		/// but scrolls the content only by the minimum needed amount to make the item fully visible and 
		/// optionally adding some spacing as specified by <paramref name="spacingFromViewportEdge"/>
		/// </summary>
		/// <param name="spacingFromViewportEdge">
		/// if not set, <see cref="BaseParams.ContentSpacing"/> will be used. Set to 0 to align the item's edge to the viewport's edge exactly, with no spacing
		/// </param>
		/// <returns>True, if the animation started. For details, see also the return value of <see cref="SmoothScrollTo(int, float, float, float, Func{float, bool}, bool)"/></returns>
		public virtual bool SmoothBringToView(int itemIndex, float duration, float? spacingFromViewportEdge = null, Func<float, bool> onProgress = null, bool overrideCurrentScrollingAnimation = false)
		{
			double spacingToUse = spacingFromViewportEdge ?? _Params.ContentSpacing;
			float spacing01RelativeToViewportSize = (float)(spacingToUse / _InternalState.vpSize);
			var vhIfVisible = GetItemViewsHolderIfVisible(itemIndex);
			bool visible = vhIfVisible != null;
			int indexInView = _ItemsDesc.GetItemViewIndexFromRealIndexChecked(itemIndex);

			double startInset = visible ? GetItemRealInsetFromParentStart(vhIfVisible.root) : _InternalState.GetItemInferredRealInsetFromParentStart(indexInView);
			if (startInset < spacingToUse) // has its start before the viewport's start => align the start with viewport's start
				return SmoothScrollTo(itemIndex, duration, 0f + spacing01RelativeToViewportSize, 0f, onProgress, overrideCurrentScrollingAnimation);
			else
			{
				double endInset = visible ? GetItemRealInsetFromParentEnd(vhIfVisible.root) : _InternalState.GetItemInferredRealInsetFromParentEnd(indexInView);
				if (endInset < spacingToUse) // same thing for the end
					return SmoothScrollTo(itemIndex, duration, 1f - spacing01RelativeToViewportSize, 1f, onProgress, overrideCurrentScrollingAnimation);
			}

			return false;
		}

		/// <summary>Stops any snapping. Stops any animation triggered by <see cref="SmoothScrollTo(int, float, float, float, Func{float, bool}, bool)"/></summary>
		public virtual void CancelAnimationsIfAny()
		{
			if (_Params.Snapper)
				_Params.Snapper.CancelSnappingIfInProgress();
			if (_SmoothScrollCoroutine != null)
			{
				StopCoroutine(_SmoothScrollCoroutine);
				_SmoothScrollCoroutine = null;

				// Bugfix: if the routine is stopped, this is not restored back. Setting it to false is the best thing we can do
				_SkipComputeVisibilityInUpdateOrOnScroll = false;

				//Debug.Log("cancel - manual");
			}
		}

		/// <summary> See <see cref="RequestChangeItemSizeAndUpdateLayout(int, float, bool, bool)"/> for additional info or if you want to resize an item which isn't visible</summary>
		/// <param name="withVH">the views holder. A common usage for an "expand on click" behavior is to have a button on a view whose onClick fires a method in the adapter where it retrieves the views holder via <see cref="GetItemViewsHolderIfVisible(RectTransform)"/> </param>
		public float RequestChangeItemSizeAndUpdateLayout(TItemViewsHolder withVH, float requestedSize, bool itemEndEdgeStationary = false, bool computeVisibility = true)
		{ return RequestChangeItemSizeAndUpdateLayout(withVH.ItemIndex, requestedSize, itemEndEdgeStationary, computeVisibility); }

		/// <summary>
		/// <para>An item width/height can be changed with this method. </para>
		/// <para>Should NOT be called during <see cref="ComputeVisibilityForCurrentPosition(bool, bool)"/>, <see cref="UpdateViewsHolder(TItemViewsHolder)"/>, <see cref="CreateViewsHolder(int)"/> or from any critical view-recycling code. Suggestion: call it from MonBehaviour.Update()</para>
		/// <para>Will change the size of the item's RectTransform to <paramref name="requestedSize"/> and will shift the other items accordingly, if needed.</para>
		/// </summary>
		/// <param name="itemIndex">the index of the item to be resized. It doesn't need to be visible(case in which only the cached size will be updated and, obviously, the visible items will shift accordingly) </param>
		/// <param name="requestedSize">the height or width (depending on scrollview's orientation)</param>
		/// <param name="itemEndEdgeStationary">if to grow to the top/left (less common) instead of down/right (more common)</param>
		/// <returns>the resolved size. This can be slightly different than <paramref name="requestedSize"/> if the number of items is huge (>100k))</returns>
		public float RequestChangeItemSizeAndUpdateLayout(int itemIndex, float requestedSize, bool itemEndEdgeStationary = false, bool computeVisibility = true)
		{
			//double vsa = _InternalState.VirtualScrollableArea;
			//if (_Params.effects.loopItems)
			//{
			//	if (vsa <= 0f)
			//		throw new UnityException("OSA: If looping is enabled, changing an item's size while the Content's size is smaller than the viewport is not supported yet");
			//}

			CancelAnimationsIfAny();

			var skipCompute_oldValue = _SkipComputeVisibilityInUpdateOrOnScroll;
			_SkipComputeVisibilityInUpdateOrOnScroll = true;

			StopMovement(); // we don't want a ComputeVisibility() during changing an item's size, so we cut off any inertia 

			int itemIndexInView = _ItemsDesc.GetItemViewIndexFromRealIndexChecked(itemIndex);
			var viewsHolderIfVisible = GetItemViewsHolderIfVisible(itemIndex);
			bool vrtContentPanelIsAtOrBeforeEnd = _InternalState.CTVirtualInsetFromVPE_Cached >= 0d;

			double curSize = _ItemsDesc[itemIndexInView];
			double resolvedSize = 
				ChangeItemSizeAndUpdateContentSizeAccordingly(
					viewsHolderIfVisible,
					itemIndexInView,
					curSize,
					requestedSize, 
					itemEndEdgeStationary
				);

			//if (_Params.effects.loopItems)
			//{
			//	float deltaSize = resolvedSize - curSize;
			//	if (vsa + deltaSize <= 0f)
			//		throw new UnityException("OSA: If looping is enabled, shrinking the Content's size below viewport's size is not supported yet");
			//}

			// TODO this is surely not always correct,so investigate
			double reportedScrollDelta;
			if (itemEndEdgeStationary)
				reportedScrollDelta = .1d;
			else
			{
				// If start edge is stationary, either if the item shrinks or expands the reportedDelta should be negative, 
				// indicating that a fake "slight scroll towards end" was done. This triggers a virtualization of the the content's position correctly to compensate for the new ctEnd 
				// and makes any item after it be visible again (in the shirnking case) if it was after viewport
				reportedScrollDelta = -.1d;

				// ..but if the ctEnd is fully visible, the items will act as if they're shrinking with itemEndEdgeStationary=true, because the content's end can't exceed the vpEnd
				if (vrtContentPanelIsAtOrBeforeEnd)
					reportedScrollDelta = .1d;
			}

			//CorrectPositionsOfVisibleItemsBasedOnCachedCTInsetFromVPS(true);

			if (computeVisibility)
				ComputeVisibilityForCurrentPositionRawParams(true, false, reportedScrollDelta);

			_SkipComputeVisibilityInUpdateOrOnScroll = skipCompute_oldValue;

			return (float)resolvedSize;
		}

		/// <summary>
		/// <para>returns the VIRTUAL distance of the item's left (if scroll view is Horizontal) or top (if scroll view is Vertical) edge </para>
		/// <para>from the parent's left (respectively, top) edge</para>
		/// </summary>
		public double GetItemVirtualInsetFromParentStart(int itemIndex)
		{ return _InternalState.GetItemVirtualInsetFromParentStartUsingItemIndexInView(_ItemsDesc.GetItemViewIndexFromRealIndexChecked(itemIndex)); }

		/// <summary>
		/// <para>returns the REAL distance of the item's left (if scroll view is Horizontal) or top (if scroll view is Vertical) edge </para>
		/// <para>from the parent's left (respectively, top) edge</para>
		/// </summary>
		public float GetItemRealInsetFromParentStart(RectTransform withRoot)
		{
			return withRoot.GetInsetFromParentEdge(_Params.Content, _InternalState.startEdge);
		}

		/// <summary>
		/// <para>returns the REAL distance of the item's right (if scroll view is Horizontal) or bottom (if scroll view is Vertical) edge </para>
		/// <para>from the parent's right (respectively, bottm) edge</para>
		/// </summary>
		public float GetItemRealInsetFromParentEnd(RectTransform withRoot)
		{
			return withRoot.GetInsetFromParentEdge(_Params.Content, _InternalState.endEdge);
		}

		/// <summary>
		/// <para>Used internally. Returns values in [0f, 1f] interval, 1 meaning the scrollrect is at start, and 0 meaning end.</para>
		/// <para>It uses a different approach when content size is smaller than viewport's size, so it can yield consistent results for <see cref="OSA{TParams, TItemViewsHolder}.ComputeVisibilityManager.ComputeVisibility(double)"/></para>
		/// </summary>
		/// <returns>1 Meaning Start And 0 Meaning End</returns> 
		public double GetVirtualAbstractNormalizedScrollPosition() { return _InternalState.GetVirtualAbstractNormalizedScrollPosition(); }

		/// <summary>
		/// Same thing as <see cref="ScrollRect.normalizedPosition"/>, just that the position is 1 for start and 0 for end, regardless if using a horizontal or vertical ScrollRect
		/// </summary>
		/// <param name="pos">1=start, 0=end</param>
		public double SetVirtualAbstractNormalizedScrollPosition(double pos, bool computeVisibilityNow, out bool looped, bool keepVelocity = false)
		{
			CancelAnimationsIfAny();

			bool ignoreOnScroll_valueBefore = _SkipComputeVisibilityInUpdateOrOnScroll;
			_SkipComputeVisibilityInUpdateOrOnScroll = true;

			double newInset = (1d - pos) * (-_InternalState.VirtualScrollableArea);
			var p = new ContentSizeOrPositionChangeParams
			{
				computeVisibilityParams = computeVisibilityNow ? _ComputeVisibilityParams_Reusable_Empty : null,
				fireScrollPositionChangedEvent = true,
				keepVelocity = keepVelocity,
			};
			double deltaInset = SetContentVirtualInsetFromViewportStart(newInset, ref p, out looped);
			_SkipComputeVisibilityInUpdateOrOnScroll = ignoreOnScroll_valueBefore;

			return deltaInset;
		}

		public void StopMovement()
		{
			_Velocity = Vector2.zero;
		}

		/// <summary>
		/// Called at the end of the <see cref="Init"/> method, if everything was successfully set up.
		/// If overriding it, don't forget to call the base's implementation.
		/// </summary>
		protected virtual void OnInitialized()
		{
			_Initialized = true;
		}

		/// <summary>
		/// Called at after every successful item count change or refresh. See also <see cref="ItemsRefreshed"/>
		/// </summary>
		protected virtual void OnItemsRefreshed(int prevCount, int newCount)
		{

		}

		/// <summary>
		/// This is called during changing the items count. 
		/// The base implementation reinitializes the items descriptor so that all items will have the same size, specified in <see cref="BaseParams.DefaultItemSize"/>
		/// If overriding the method and the item default size should remain the same as <see cref="BaseParams.DefaultItemSize"/>, 
		/// don't forget to call the base implementation! Otherwise, call <see cref="ItemsDescriptor.ReinitializeSizes(ItemCountChangeMode, int, int, double?)"/> with the new default size as parameter.
		/// Use <see cref="ItemsDescriptor.BeginChangingItemsSizes(int)"/> before and <see cref="ItemsDescriptor.EndChangingItemsSizes()"/> after
		/// setting sizes. The indices of items for which you set custom sizes must be one after another (4,5,6,7.. etc). Gaps are not allowed.
		/// Use "itemsDesc[itemIndexInView] = size" syntax for setting custom sizes. In this call, <see cref="AbstractViewsHolder.ItemIndex"/> will be the same as <see cref="BaseItemViewsHolder.itemIndexInView"/>, even if looping is enabled.
		/// </summary>
		/// <param name="itemsDesc">The container for all the info related to items' sizes</param>
		protected virtual void CollectItemsSizes(ItemCountChangeMode changeMode, int count, int indexIfInsertingOrRemoving, ItemsDescriptor itemsDesc)
		{
			int oldCount = itemsDesc.itemsCount;
			itemsDesc.ReinitializeSizes(changeMode, count, indexIfInsertingOrRemoving, _Params.DefaultItemSize);
			int itemIndexOfFirstVH = _VisibleItemsCount > 0 ? _VisibleItems[0].ItemIndex : -1;
			itemsDesc.ReinitializeRealIndexOfFirstItemInView(oldCount, changeMode, count, indexIfInsertingOrRemoving, itemIndexOfFirstVH, _Params.effects.loopItems);
		}

		/// <summary> 
		/// <para>Called when there are no recyclable views for itemIndex. Provide a new viewsholder instance for itemIndex. This is the place where you must initialize the viewsholder </para>
		/// <para>via <see cref="AbstractViewsHolder.Init(GameObject, int, bool, bool)"/> shortcut or manually set its itemIndex, instantiate the prefab and call its <see cref="AbstractViewsHolder.CollectViews"/></para>
		/// </summary>
		/// <param name="itemIndex">the index of the model that'll be presented by this views holder</param>
		protected abstract TItemViewsHolder CreateViewsHolder(int itemIndex);

		/// <summary>
		/// <para>Here the data in your model should be bound to the views. Use newOrRecycled.ItemIndex (<see cref="AbstractViewsHolder.ItemIndex"/>) to retrieve its associated model</para>
		/// <para>Note that views holders are re-used (this is the main purpose of this adapter), so a views holder's views will contain data from its previously associated model and if, </para>
		/// <para>for example, you're downloading an image to be set as an icon, it makes sense to first clear the previous one (and probably temporarily replace it with a generic "Loading" image)</para>
		/// <para>Note that this is not called for items that will remain visible after an Insert or Remove operation is done</para>
		/// </summary>
		/// <param name="newOrRecycled"></param>
		protected abstract void UpdateViewsHolder(TItemViewsHolder newOrRecycled);

		/// <summary>
		/// <para>Called when an insert or remove event happens. You'll only need this if some of your views depend on the item's index itself as opposed to depending only on the model's data</para>
		/// <para>For example, if your item is from a leaderboard and each player's place is given by the order of the models (i.e. you don't have an int in the model named 'place'),
		/// you may want to display the item's title as '#233 PlayerName'. This works well if you're only using <see cref="ResetItems(int, bool, bool)"/>, but if you'll call
		/// <see cref="InsertItems(int, int, bool, bool)"/> or <see cref="RemoveItems(int, int, bool, bool)"/>, the indices of some views holders are shifted, while they'll maintain their data.
		/// In this case, you'll override this method and only update the title from its model</para>
		/// <para>This is an important optimization, because you shouldn't update items that are already updated, especially when fetching them from the web</para>
		/// </summary>
		/// <param name="shiftedViewsHolder">The views holder associated with the item whose index was shifted</param>
		/// <param name="oldIndex">the item's old index</param>
		/// <param name="wasInsert">true, if <see cref="InsertItems(int, int, bool, bool)"/> was called. false if <see cref="RemoveItems(int, int, bool, bool)"/> was called</param>
		/// <param name="removeOrInsertIndex">the index at which an insert or remove operation was made</param>
		protected virtual void OnItemIndexChangedDueInsertOrRemove(TItemViewsHolder shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{

		}

		/// <summary> Self-explanatory. The default implementation returns true each time</summary>
		/// <returns>If the provided views holder is compatible with the item with index <paramref name="indexOfItemThatWillBecomeVisible"/></returns>
		protected virtual bool IsRecyclable(TItemViewsHolder potentiallyRecyclable, int indexOfItemThatWillBecomeVisible, double sizeOfItemThatWillBecomeVisible)
		{ return true; }

		/// <summary> Self-explanatory. The default implementation returns true if <paramref name="isInExcess"/> is true </summary>
		/// <param name="inRecycleBin">an item in the recycle bin (not visible, disabled)</param>
		/// <param name="isInExcess">this will be true if the current number of items exceeded the allowed one (as inferred from the parameters given at initialization)</param>
		protected virtual bool ShouldDestroyRecyclableItem(TItemViewsHolder inRecycleBin, bool isInExcess)
		{ return isInExcess; }

		/// <summary>
		/// Perfect place to clean the views in order to prepare them to be potentially recycled this frame or soon. <paramref name="newItemIndex"/> will be -1 if the item will be disabled/destroyed instead of being recycled.
		/// </summary>
		/// <param name="inRecycleBinOrVisible"></param>
		/// <param name="newItemIndex">-1 means it'll only be disabled and/or destroyed, not recycled ATM</param>
		protected virtual void OnBeforeRecycleOrDisableViewsHolder(TItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{ }

		/// <summary>Destroying any remaining game objects in the <see cref="_RecyclableItems"/> list and clearing it</summary>
		protected virtual void ClearCachedRecyclableItems()
		{
			if (_RecyclableItems != null)
			{
				foreach (var item in _RecyclableItems)
				{
					if (item != null && item.root != null)
						try { GameObject.Destroy(item.root.gameObject); } catch (Exception e) { Debug.LogException(e); }
				}
				_RecyclableItems.Clear();
			}
			//if (_StolenItems != null)
			//{
			//	foreach (var item in _StolenItems)
			//	{
			//		if (item != null && item.root != null)
			//			try { GameObject.Destroy(item.root.gameObject); } catch (Exception e) { Debug.LogException(e); }
			//	}
			//	_StolenItems.Clear();
			//}
		}

		/// <summary>Destroying any remaining game objects in the <see cref="_VisibleItems"/> list, clearing it and setting <see cref="VisibleItemsCount"/> to 0</summary>
		protected virtual void ClearVisibleItems()
		{
			// Not checking the IsInitialized, because in case of a partial initialization, it'd be false, but the visible items list still needs cleanup
			if (_VisibleItems != null)
			{
				foreach (var item in _VisibleItems)
				{
					if (item != null)
					{
						OnBeforeRecycleOrDisableViewsHolder(item, -1);
						if (item.root != null)
							try { GameObject.Destroy(item.root.gameObject); } catch (Exception e) { Debug.LogException(e); }
					}
				}
				_VisibleItems.Clear();
				_VisibleItemsCount = 0;
			}

			// Bugfix 05.11.2018 (thanks LordBelasco (Unity forum)).
			if (_ReleaseFromPull != null)
			{
				// There's no first item to calculate inset, so cancel any pending progress
				_ReleaseFromPull.inProgress = false;
			}
		}

		/// <summary>This is called automatically when the size of this ScrollView changes</summary>
		protected virtual void OnScrollViewSizeChanged()
		{
			// Commented: refresh already does that
			//CancelAnimationsIfAny();

			//Debug.Log("TODO test virtualization when scroll view size changes");
			//_InternalState.layoutRebuildPendingDueToScrollViewSizeChangeEvent = true;

			//// Commented: is now called in 
			//Refresh();

			//CollectItemsSizes(ItemCountChangeMode.INSERT, 0, 0, _ItemsDesc);
		}

		/// <summary>
		/// <para>Called mainly when it's detected that the scroll view's size has changed. Marks everything for a layout rebuild and then calls Canvas.ForceUpdateCanvases(). </para>
		/// <para>IMPORTANT: Make sure to override <see cref="AbstractViewsHolder.MarkForRebuild"/> in your views holder implementation if you have child layout groups and call LayoutRebuilder.MarkForRebuild() on them</para> 
		/// </summary>
		protected virtual void RebuildLayoutDueToScrollViewSizeChange()
		{
			//float viewportSizeBefore = _InternalState.viewportSize;

			// Commented: Refresh will clear them anyway
			//MarkViewsHoldersForRebuild(_VisibleItems);
			////MarkViewsHoldersForRebuild(_RecyclableItems);
			ClearCachedRecyclableItems();

			LayoutRebuilder.ForceRebuildLayoutImmediate(_Params.ScrollViewRT);
			//Canvas.ForceUpdateCanvases();

			_Params.InitIfNeeded(this);
			if (_Params.Snapper)
				_Params.Snapper.Adapter = this;

			_InternalState.CacheScrollViewInfo(); // update vp size etc.
            _ItemsDesc.maxVisibleItemsSeenSinceLastScrollViewSizeChange = 0;
            _ItemsDesc.destroyedItemsSinceLastScrollViewSizeChange = 0;

			Refresh(false, true);
			//ChangeItemsCount(ItemCountChangeMode.RESET, GetItemsCount(), -1, false, true); // keeping velocity
		}

		/// <summary> 
		/// Only called for vertical ScrollRects. Called just before a "Twin" ComputeVisibility will execute. 
		/// This can be used, for example, to disable a ContentSizeFitter on the item which was used to externally calculate the item's size in the current Twin ComputeVisibility pass</summary>
		/// <seealso cref="ScheduleComputeVisibilityTwinPass(bool)"/>
		protected virtual void OnItemHeightChangedPreTwinPass(TItemViewsHolder viewsHolder) { }

		/// <summary> Same as <see cref="OnItemHeightChangedPreTwinPass(TItemViewsHolder)"/>, but for horizontal ScrollRects</summary>
		/// <seealso cref="ScheduleComputeVisibilityTwinPass(bool)"/>
		protected virtual void OnItemWidthChangedPreTwinPass(TItemViewsHolder viewsHolder) { }

		/// <summary>
		/// <para>This can be called in order to schedule a "Twin" ComputeVisibility() call after exactly 1 frame.</para> 
		/// <para>A use case is to enable a ContentSizeFitter on your item, call this, </para> 
		/// <para>and then disable the ContentSizeFitter in <see cref="OnItemHeightChangedPreTwinPass(TItemViewsHolder)"/> (or <see cref="OnItemWidthChangedPreTwinPass(TItemViewsHolder)"/> if horizontal ScrollView)</para> 
		/// </summary>
		/// <param name="preferContentEndEdgeStationaryIfSizeChanges">this will only be considered if the scroll position didn't change since the last visibility computation</param>
		protected void ScheduleComputeVisibilityTwinPass(bool preferContentEndEdgeStationaryIfSizeChanges = false)
		{
			//if (_Params.effects.loopItems)
			//	throw new UnityException(
			//		"OSA.ScheduleComputeVisibilityTwinPass: Looping is enabled. "+
			//		"Dynamically inferring item's sizes through a ContentSizeFitter or similar techniques is not supported yet. Disable the looping effect if you need this functionality"
			//	);

			// Commented: since now the twin pass is done immediately after the usual pass, the update mode is not restricted anymore
			//if (_Params.updateMode != BaseParams.UpdateMode.MONOBEHAVIOUR_UPDATE)
			//	throw new UnityException("Twin pass is only possible if updateMode is " + BaseParams.UpdateMode.MONOBEHAVIOUR_UPDATE);

			_InternalState.computeVisibilityTwinPassScheduled = true;
			_InternalState.preferKeepingCTEndEdgeStationaryInNextComputeVisibilityTwinPass = preferContentEndEdgeStationaryIfSizeChanges;
		}

		/// <summary>
		/// Format of normalized points:
		/// X: 0=left .. 1=right
		/// Y: 0=bottom .. 1=top
		/// See <see cref="GetViewsHolderClosestToViewportLongitudinalNormalizedAbstractPoint(Canvas, RectTransform, float, float, out float)"/>
		/// </summary>
		protected virtual AbstractViewsHolder GetViewsHolderClosestToViewportNormalizedPoint(
			ICollection<TItemViewsHolder> viewsHolders,
			Canvas c,
			RectTransform canvasRectTransform,
			Vector2 viewportPointNormalized,
			Vector2 itemPointNormalized,
			out float distance
		)
		{
			TItemViewsHolder result = null;
			float minDistance = float.MaxValue;
			float curDistance;

			foreach (var vh in viewsHolders)
			{
				var v = UIUtils8.Instance.GetWorldVectorBetweenCustomLocalPivots(_Params.Viewport, viewportPointNormalized, vh.root, itemPointNormalized, c, canvasRectTransform);

				curDistance = v.magnitude;
				if (curDistance < minDistance)
				{
					result = vh;
					minDistance = curDistance;
				}
			}

			distance = minDistance;
			return result;
		}

		/// <summary>
		/// Format of abstract normalized points:
		/// X: 0=left .. 1=right
		/// Y: 0=top .. 1=bottom (inverted)
		/// See <see cref="GetViewsHolderClosestToViewportLongitudinalNormalizedAbstractPoint(Canvas, RectTransform, float, float, out float)"/>
		/// </summary>
		protected virtual AbstractViewsHolder GetViewsHolderClosestoViewportNormalizedAbastractPoint(
			ICollection<TItemViewsHolder> viewsHolders,
			Canvas c,
			RectTransform canvasRectTransform,
			Vector2 viewportPointNormalizedWithYInverted,
			Vector2 itemPointNormalizedWithYInverted,
			out float distance
		){
			viewportPointNormalizedWithYInverted.y = 1f - viewportPointNormalizedWithYInverted.y;
			itemPointNormalizedWithYInverted.y = 1f - itemPointNormalizedWithYInverted.y;

			return GetViewsHolderClosestToViewportNormalizedPoint(
				viewsHolders, c, canvasRectTransform, viewportPointNormalizedWithYInverted, itemPointNormalizedWithYInverted, out distance
			);
		}

		public void SetViewsHolderEnabled(TItemViewsHolder vh, bool vhEnabled)
		{
			if (_Params.optimization.scaleToZeroInsteadOfDisable)
			{
				Vector3 scale;
				if (vhEnabled)
				{
					scale = Vector3.one;
					if (!vh.root.gameObject.activeSelf)
					{
						// The game object is expected to be active
						vh.root.gameObject.SetActive(true);
					}
				}
				else
					scale = Vector3.zero;
				vh.root.localScale = scale;
			}
			else if (vh.root.gameObject.activeSelf != vhEnabled)
				vh.root.gameObject.SetActive(vhEnabled);
		}

		public void SetViewsHolderEnabled(TItemViewsHolder vh)
		{
			if (_Params.optimization.scaleToZeroInsteadOfDisable)
			{
				if (!vh.root.gameObject.activeSelf)
					// The game object is expected to be active
					vh.root.gameObject.SetActive(true);
				vh.root.localScale = Vector3.one;
			}
			else if (!vh.root.gameObject.activeSelf)
				vh.root.gameObject.SetActive(true);
		}

		public bool IsViewsHolderEnabled(TItemViewsHolder vh)
		{
			if (!vh.root.gameObject.activeSelf)
				return false;

			if (_Params.optimization.scaleToZeroInsteadOfDisable)
				return vh.root.localScale != Vector3.zero;
			return true;
		}

		public void SetViewsHolderDisabled(TItemViewsHolder vh)
		{
			if (_Params.optimization.scaleToZeroInsteadOfDisable)
				vh.root.localScale = Vector3.zero;
			else if (vh.root.gameObject.activeSelf)
				vh.root.gameObject.SetActive(false);
		}

		/// <summary>Called automatically in <see cref="OnDestroy"/></summary>
		protected virtual void Dispose()
		{
			_Initialized = false;
			
			if (_SmoothScrollCoroutine != null)
			{
				try { StopCoroutine(_SmoothScrollCoroutine); } catch { }

				_SmoothScrollCoroutine = null;
			}

			ClearCachedRecyclableItems();
			_RecyclableItems = null;
			//_StolenItems = null;

			ClearVisibleItems();
			_VisibleItems = null;

			_Params = null;
			_InternalState = null;

			if (ItemsRefreshed != null)
				ItemsRefreshed = null;
		}
	}
}
