//#define MIGRATE_3_2_TO_4_1_AVAILABLE
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using frame8.Logic.Misc.Other.Extensions;

namespace Com.TheFallenGames.OSA.Core
{
	/// <summary>
	/// <para>Input params to be passed to <see cref="OSA{TParams, TItemViewsHolder}.Init()"/></para>
	/// <para>This can be used Monobehaviour's field and exposed via inspector (most common case)</para>
	/// <para>Or can be manually constructed, depending on what's easier in your context</para>
	/// </summary>
	[System.Serializable]
	public class BaseParams
	{
		#region Configuration

		#region Core params
		[SerializeField]
		RectTransform _Content = null;
		[Obsolete("Use Content instead")]
		public RectTransform content { get { return Content; } set { Content = value; } }
		public RectTransform Content { get { return _Content; } set { _Content = value; } }

		[Tooltip("If null, the scrollRect is considered to be the viewport")]
		[SerializeField]
		//[HideInInspector]
		[FormerlySerializedAs("viewport")]
		RectTransform _Viewport = null;
		[Obsolete("Use Viewport instead")]
		public RectTransform viewport { get { return Viewport; } set { Viewport = value; } }
		/// <summary>If null, <see cref="ScrollViewRT"/> is considered to be the viewport</summary>
		public RectTransform Viewport { get { return _Viewport; } set { _Viewport = value; } }

		[SerializeField]
		OrientationEnum _Orientation = OrientationEnum.VERTICAL;
		[Obsolete("Use Orientation instead")]
		public OrientationEnum orientation { get { return Orientation; } set { Orientation = value; } }
		public OrientationEnum Orientation { get { return _Orientation; } set { _Orientation = value; } }

		[SerializeField]
		Scrollbar _Scrollbar = null;
		[Obsolete("Use Scrollbar instead")]
		public Scrollbar scrollbar { get { return Scrollbar; } set { Scrollbar = value; } }
		public Scrollbar Scrollbar { get { return _Scrollbar; } set { _Scrollbar = value; } }

		[Tooltip("The sensivity to the Mouse's scrolling wheel or similar input methods. Not related to dragging or scrolling via scrollbar")]
		[SerializeField]
		float _ScrollSensivity = 100f;
		/// <summary>The sensivity to the Mouse's scrolling wheel or similar input methods. Not related to dragging or scrolling via scrollbar</summary>
		public float ScrollSensivity { get { return _ScrollSensivity; } protected set { _ScrollSensivity = value; } }

		[SerializeField]
		//[HideInInspector]
		[FormerlySerializedAs("contentPadding")]
		RectOffset _ContentPadding = new RectOffset();
		[Obsolete("Use ContentPadding instead")]
		public RectOffset contentPadding { get { return ContentPadding; } set { ContentPadding = value; } }
		/// <summary>Padding for the 4 edges of the content panel</summary>
		public RectOffset ContentPadding { get { return _ContentPadding; } set { _ContentPadding = value; } }

		[SerializeField]
		//[HideInInspector]
		[FormerlySerializedAs("contentGravity")]
		ContentGravity _Gravity = ContentGravity.START;
		[Obsolete("Use Gravity instead")]
		public ContentGravity contentGravity { get { return Gravity; } set { Gravity = value; } }
		/// <summary>
		/// The effect of this property can only be seen when the content size is smaller than the viewport, case in which there are 3 possibilities: 
		/// place the content at the start, middle or end. <see cref="ContentGravity.FROM_PIVOT"/> doesn't change the content's position (it'll be preserved from the way you aligned it in edit-mode)
		/// </summary>
		public ContentGravity Gravity { get { return _Gravity; } set { _Gravity = value; } }

		[Tooltip("The space between items")]
		[SerializeField]
		//[HideInInspector]
		[FormerlySerializedAs("contentSpacing")]
		float _ContentSpacing = 0f;
		[Obsolete("Use ContentSpacing instead")]
		public float contentSpacing { get { return ContentSpacing; } set { ContentSpacing = value; } }
		/// <summary>Spacing between items (horizontal if the ScrollView is horizontal. else, vertical)</summary>
		public float ContentSpacing { get { return _ContentSpacing; } set { _ContentSpacing = value; } }

		[Tooltip("The size of all items for which the size is not specified in CollectItemSizes()")]
		[SerializeField]
		//[HideInInspector]
		float _DefaultItemSize = 60f;
		/// <summary>The size of all items for which the size is not specified</summary>
		public float DefaultItemSize { get { return _DefaultItemSize; } protected set { _DefaultItemSize = value; } }

		[Tooltip("You'll probably need this if the scroll view is a child of another scroll view." +
		" If enabled, the first parent that implements all of IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler and IEndDragHandler will be informed" +
		" about these events when they occur on this scroll view. This works both with Unity's ScrollRect and OSA")]
		[SerializeField]
		bool _ForwardDragToParents = false;
		/// <summary>You'll probably need this if the scroll view is a child of another scroll view.
		/// If enabled, the first parent that implements all of IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler and IEndDragHandler will be informed 
		/// about these events when they occur on this scroll view. This works both with Unity's ScrollRect and OSA</summary>
		public bool ForwardDragToParents { get { return _ForwardDragToParents; } protected set { _ForwardDragToParents = value; } }

		[Tooltip("Allows you to click and drag the content directly (enabled by default). The property ForwardDragToParents is not affected by this")]
		[SerializeField]
		bool _DragEnabled = true;
		/// <summary>
		/// Allows you to click and drag the content directly (enabled by default). 
		/// The <see cref="ForwardDragToParents"/> is not affected by this.
		/// NOTE: Do not change this property during a drag event (i.e. when <see cref="OSA{TParams, TItemViewsHolder}.IsDragging"/> is true)
		/// </summary>
		public bool DragEnabled { get { return _DragEnabled; } protected set { _DragEnabled = value; } }

		[Tooltip("Allows you to scroll by mouse wheel or other similar input devices (enabled by default). The property ForwardDragToParents is not affected by this")]
		[SerializeField]
		bool _ScrollEnabled = true;
		/// <summary>
		/// Allows you to scroll by mouse wheel or other similar input devices (enabled by default). 
		/// The <see cref="ForwardDragToParents"/> is not affected by this.
		/// NOTE: Do not change this property during a drag event (i.e. when <see cref="OSA{TParams, TItemViewsHolder}.IsDragging"/> is true)
		/// </summary>
		public bool ScrollEnabled { get { return _ScrollEnabled; } protected set { _ScrollEnabled = value; } }

		[Tooltip("If enabled, multiple drags in the same direction will lead to greater speeds")]
		[SerializeField]
		bool _TransientSpeedBetweenDrags = true;
		/// <summary>If enabled, multiple drags in the same direction will lead to greater speeds</summary>
		public bool TransientSpeedBetweenDrags { get { return _TransientSpeedBetweenDrags; } protected set { _TransientSpeedBetweenDrags = value; } }
		#endregion

		public Effects effects = new Effects();

		//[SerializeField]
		//[HideInInspector]
		//[FormerlySerializedAs("contentVisual")]
		//RawImage _ContentVisual;
		//[Obsolete("Use Content instead")]
		//public RawImage contentVisual { get { return _ContentVisual; } }

		//[SerializeField]
		//public float contentVisualParallaxEffect = -.85f;

		//[SerializeField]
		//[HideInInspector]
		//[FormerlySerializedAs("elasticMovement")]
		//bool _ElasticMovement = true;
		//[Obsolete("Use ElasticMovement instead")]
		//public bool elasticMovement { get { return ElasticMovement; } set { ElasticMovement = value; } }
		//public bool ElasticMovement { get { return _ElasticMovement; } set { _ElasticMovement = effects.elasticMovement = value; } }

		//[SerializeField]
		//[HideInInspector]
		//[FormerlySerializedAs("pullElasticity")]
		//float _PullElasticity = .3f;
		//[Obsolete("Use PullElasticity instead")]
		//public float pullElasticity { get { return PullElasticity; } }
		//public float PullElasticity { get { return _PullElasticity; } }

		//[SerializeField]
		//[HideInInspector]
		//[FormerlySerializedAs("releaseTime")]
		//float _ReleaseTime = .1f;
		//[Obsolete("Use ReleaseTime instead")]
		//public float releaseTime { get { return ReleaseTime; } }
		//public float ReleaseTime { get { return _ReleaseTime; } }

		//[SerializeField]
		//[HideInInspector]
		//[FormerlySerializedAs("inertia")]
		//bool _Inertia = true;
		//[Obsolete("Use Inertia instead")]
		//public bool inertia { get { return Inertia; } }
		//public bool Inertia { get { return _Inertia; } }

		//[SerializeField]
		//[HideInInspector]
		//[FormerlySerializedAs("inertiaDecelerationRate")]
		//float _InertiaDecelerationRate = 1f - .135f;
		//[Obsolete("Use InertiaDecelerationRate instead")]
		//public float inertiaDecelerationRate { get { return InertiaDecelerationRate; } set { InertiaDecelerationRate = value; } }
		//public float InertiaDecelerationRate { get { return _InertiaDecelerationRate; } set { _InertiaDecelerationRate = effects.inertiaDecelerationRate = value; } }

		//[SerializeField]
		//[HideInInspector]
		//[FormerlySerializedAs("maxSpeed")]
		//float _MaxSpeed = 20 * 1000;
		//[Obsolete("Use MaxSpeed instead")]
		//public float maxSpeed { get { return MaxSpeed; } set { MaxSpeed = value; } }
		//public float MaxSpeed { get { return _MaxSpeed; } set { _MaxSpeed = effects.maxSpeed = value; } }

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("loopItems")]
		bool _LoopItems = false;
		[Obsolete("Use effects.loopItems instead", true)]
		public bool loopItems { get { return effects.loopItems; } }

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("galleryEffectAmount")]
		float _GalleryEffectAmount = 0f;
		[Obsolete("Use effects.galleryEffectAmount instead", true)]
		public float galleryEffectAmount { get { return effects.galleryEffectAmount; }  set { effects.galleryEffectAmount = value; } }

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("galleryEffectViewportPivot")]
		float _GalleryEffectViewportPivot = .5f;
		[Obsolete("Use effects.galleryEffectViewportPivot instead", true)]
		public float galleryEffectViewportPivot { get { return effects.galleryEffectViewportPivot; } }


		public Optimization optimization = new Optimization();

		[SerializeField]
		[HideInInspector]
		[FormerlySerializedAs("recycleBinCapacity")]
		int _RecycleBinCapacity = -1;
		[Obsolete("Use optimization.recycleBinCapacity instead", true)]
		public int recycleBinCapacity { get { return optimization.recycleBinCapacity; } }

		//[SerializeField]
		//[HideInInspector]
		//[FormerlySerializedAs("updateMode")]
		//UpdateModeEnum _UpdateMode = UpdateModeEnum.ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE;
		//[Obsolete("Use optimization.updateMode instead", true)]
		//public UpdateModeEnum updateMode { get { return optimization.updateMode; } set { optimization.updateMode = value; } }
		#endregion


		public bool IsHorizontal { get { return _Orientation == OrientationEnum.HORIZONTAL; } }
		//public DefaultSizeUsage DefaultItemSizeUsage { get { return _DefaultItemSizeUsage; } }
		public RectTransform ScrollViewRT { get { return _ScrollViewRT; } }
		public Snapper8 Snapper { get { return _Snapper; } }

		RectTransform _ScrollViewRT;
		Snapper8 _Snapper;


		/// <summary>It's here just so the class can be serialized by Unity when used as a MonoBehaviour's field</summary>
		public BaseParams() { }

#if MIGRATE_3_2_TO_4_1_AVAILABLE
		public virtual bool MigrateFieldsToVersion4(ScrollRect scrollRect, out string error, out string additionalInfoOnSuccess)
		{
			additionalInfoOnSuccess = null;
			if (!scrollRect)
			{
				error = "ScrollRect component not found. It's needed during the migration. If this is unexpected, maybe this adapter already migrated?";
				return false;
			}
			if (!scrollRect.content)
			{
				error = "ScrollRect.content wasn't set";
				return false;
			}
			Content = scrollRect.content;

			// Commented: already serialized
			//Viewport = scrollRect.viewport;

			if (scrollRect.horizontal == scrollRect.vertical)
			{
				error = "Both horizontal and vertical properties of the ScrollRect are set to " + scrollRect.horizontal + ". Exactly one of them should be active";
				return false;
			}
			Orientation = scrollRect.horizontal ? OrientationEnum.HORIZONTAL : OrientationEnum.VERTICAL;

			if (scrollRect.horizontalScrollbar && scrollRect.verticalScrollbar)
			{
				additionalInfoOnSuccess = "Both horizontal and vertical scrollbars are assigned. Only the " + (Orientation.ToString().ToLowerInvariant()) + " one was grabbed";
				Scrollbar = Orientation == OrientationEnum.HORIZONTAL ? scrollRect.horizontalScrollbar : scrollRect.verticalScrollbar;
			}

			ScrollSensivity = scrollRect.scrollSensitivity;

			if (effects == null)
				effects = new Effects();
			//effects.contentVisual = contentVisual;
			//effects.ContentVisualParallaxEffect = contentVisualParallaxEffect;
			//effects.elasticMovement = elasticMovement;
			effects.galleryEffectAmount = _GalleryEffectAmount;
			effects.galleryEffectViewportPivot = _GalleryEffectViewportPivot;
			//effects.inertia = inertia;
			//effects.inertiaDecelerationRate = inertiaDecelerationRate;
			//effects.maxSpeed = maxSpeed;
			effects.loopItems = _LoopItems;
			//effects.pullElasticity = pullElasticity;
			//effects.releaseTime = releaseTime;

			if (optimization == null)
				optimization = new Optimization();
			optimization.recycleBinCapacity = _RecycleBinCapacity;
			//optimization.updateMode = _UpdateMode;

			error = null;
			return true;
		}
#endif

		/// <summary>
		/// Called internally in <see cref="OSA{TParams, TItemViewsHolder}.Init()"/> and every time the scrollview's size changes. 
		/// This makes sure the content and viewport have valid values. It can also be overridden to initialize custom data
		/// </summary>
		public virtual void InitIfNeeded(IOSA iAdapter)
        {
			_ScrollViewRT = iAdapter.AsMonoBehaviour.transform as RectTransform;
			var sr = ScrollViewRT.GetComponent<ScrollRect>();
			if (sr && sr.enabled)
				throw new UnityException("OSA: The ScrollRect is not needed anymore starting with v4.0. Remove or disable it!");

			if (!Content)
				throw new UnityException("OSA: Content not set!");
			if (!Viewport)
			{
				Viewport = ScrollViewRT;
				if (Content.parent != ScrollViewRT)
					throw new UnityException("OSA: Content's parent should be the ScrollView itself if there's no viewport specified!");
			}
			if (!_Snapper)
				_Snapper = ScrollViewRT.GetComponent<Snapper8>();

			effects.InitIfNeeded();

			// There's no concept of content padding when looping. spacinf should be used instead
			if (effects.loopItems)
			{
				bool showLog = false;
				int ctSp = (int)ContentSpacing;
				if (IsHorizontal)
				{
					if (ContentPadding.left != ctSp)
					{
						showLog = true;
						ContentPadding.left = ctSp;
					}

					if (ContentPadding.right != ctSp)
					{
						showLog = true;
						ContentPadding.right = ctSp;
					}
				}
				else
				{
					if (ContentPadding.top != ctSp)
					{
						showLog = true;
						ContentPadding.top = ctSp;
					}

					if (ContentPadding.bottom != ctSp)
					{
						showLog = true;
						ContentPadding.bottom = ctSp;
					}
				}

				if (showLog)
					Debug.Log("OSA: setting conteng padding to be the same as content spacing (" + ContentSpacing.ToString("#############.##")+"), because looping is enabled");
			}
		}

		[System.Obsolete("This method was moved to the OSA class", true)]
		public float GetAbstractNormalizedScrollPosition() { return 0f; }

		/// <summary>
		/// See <see cref="ContentGravity"/>
		/// </summary>
		public void UpdateContentPivotFromGravityType()
		{
			if (Gravity != ContentGravity.FROM_PIVOT)
			{
				int v1_h0 = IsHorizontal ? 0 : 1;

				var piv = Content.pivot;

				// The transfersal position is at the center
				piv[1 - v1_h0] = .5f;

				int contentGravityAsInt = ((int)Gravity);
				float pivotInScrollingDirection_IfVerticalScrollView;
				if (contentGravityAsInt < 3)
					// 1 = TOP := 1f;
					// 2 = CENTER := .5f;
					pivotInScrollingDirection_IfVerticalScrollView = 1f / contentGravityAsInt;
				else
					// 3 = BOTTOM := 0f;
					pivotInScrollingDirection_IfVerticalScrollView = 0f;

				piv[v1_h0] = pivotInScrollingDirection_IfVerticalScrollView;
				if (v1_h0 == 0) // i.e. if horizontal
					piv[v1_h0] = 1f - piv[v1_h0];

				Content.pivot = piv;
			}
		}


		public enum OrientationEnum
		{
			VERTICAL,
			HORIZONTAL
		}


		// Commented: UPDATE mode is not supported anymore, because we couldn't control the unpredictable behavior.
		// Maybe it'll be added back as an internal method which you can call, something like "ActivateExperimentalUpdateMode()"
		///// <summary> Represents how often or when the optimizer does his core loop: checking for any items that need to be created, destroyed, disabled, displayed, recycled</summary>
		//public enum UpdateModeEnum
		//{
		//	/// <summary>
		//	/// <para>Updates are triggered by a MonoBehaviour.Update() (i.e. each frame the ScrollView is active) and at each OnScroll event</para>
		//	/// <para>Moderate performance when scrolling, but works in all cases</para>
		//	/// </summary>
		//	ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE,

		//	/// <summary>
		//	/// <para>Updates ar triggered by each OnScroll event</para>
		//	/// <para>Experimental. However, if you use it and see no issues, it's recommended over ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE.</para>
		//	/// <para>This is also useful if you don't want the optimizer to use CPU when idle.</para>
		//	/// <para>A bit better performance when scrolling</para>
		//	/// </summary>
		//	ON_SCROLL,

		//	/// <summary>
		//	/// <para>Update is triggered by a MonoBehaviour.Update() (i.e. each frame the ScrollView is active)</para>
		//	/// <para>In this mode, some temporary gaps appear when fast-scrolling. If this is not acceptable, use other modes.</para>
		//	/// <para>Best performance when scrolling, items appear a bit delayed when fast-scrolling</para>
		//	/// </summary>
		//	MONOBEHAVIOUR_UPDATE
		//}


		/// <summary> Represents how often or when the optimizer does his core loop: checking for any items that need to be created, destroyed, disabled, displayed, recycled</summary>
		public enum ContentGravity
		{
			/// <summary>you set it up manually</summary>
			FROM_PIVOT,

			/// <summary>top if vertical scrollview, else left</summary>
			START,

			/// <summary>top if vertical scrollview, else left</summary>
			CENTER,

			/// <summary>bottom if vertical scrollview, else right</summary>
			END

		}


		[Serializable]
		public class Effects
		{
			public const float DEFAULT_MAX_SPEED = 10 * 1000f;
			public const float MAX_SPEED = DEFAULT_MAX_SPEED * 100;
			public const float MAX_SPEED_IF_LOOPING = DEFAULT_MAX_SPEED;

			[Tooltip("This RawImage will be scrolled together with the content. \n" +
				"The content is always stationary (this is the way the recycling process works), so the scrolling effect is faked by scrolling the texture's x/y.\n" +
				"Tip: use a seamless/looping background texture for best visual results")]
			public RawImage contentVisual = null;

			public bool elasticMovement = true;

			public float pullElasticity = .3f;

			public float releaseTime = .1f;

			public bool inertia = true;

			/// <summary>What amount of the velociy will be lost per second after the drag ended</summary>
			[Tooltip("What percent (0=0%, 1=100%) of the velociy will be lost per second after the drag ended. 1=all(immediate stop), 0=none(maitain constant scrolling speed indefinitely)")]
			// Unity's original ScrollRect mistakenly names "deceleration rate" the amount that should REMAIN, 
			// not the one that will be REMOVED from the velocity. And its deault value is 0.135. 
			// Here, we're setting the correct default value. A 0 deceleration rate should mean no deceleration
			[Range(0f, 1f)]
			public float inertiaDecelerationRate = 1f - .135f;

			public float maxSpeed = DEFAULT_MAX_SPEED;

			/// <summary>
			/// <para>If true: When the last item is reached, the first one appears after it, basically allowing you to scroll infinitely.</para>
			/// <para>Initially intended for things like spinners, but it can be used for anything alike. It may interfere with other functionalities in some very obscure/complex contexts/setups, so be sure to test the hell out of it.</para>
			/// <para>Also please note that sometimes during dragging the content, the actual looping changes the Unity's internal PointerEventData for the current click/touch pointer id, </para>
			/// <para>so if you're also externally tracking the current click/touch, in this case only 'PointerEventData.pointerCurrentRaycast' and 'PointerEventData.position'(current position) are </para>
			/// <para>preserved, the other ones are reset to defaults to assure a smooth loop transition</para>
			/// </summary>
			[Tooltip("If true: When the last item is reached, the first one appears after it, basically allowing you to scroll infinitely.\n" +
				" Initially intended for things like spinners, but it can be used for anything alike.\n" +
				" It may interfere with other functionalities in some very obscure/complex contexts/setups, so be sure to test the hell out of it.\n" +
				" Also please note that sometimes during dragging the content, the actual looping changes the Unity's internal PointerEventData for the current click/touch pointer id, so if you're also externally tracking the current click/touch, in this case only 'PointerEventData.pointerCurrentRaycast' and 'PointerEventData.position'(current position) are preserved, the other ones are reset to defaults to assure a smooth loop transition. Sorry for the long decription. Here's an ASCII potato: (@)")]
			public bool loopItems = false;

			//public bool contentVisualAllowSmallerThanViewport;

			[Tooltip("The contentVisual's additional drag factor. Examples:\n" +
				"-2: the contentVisual will move exactly by the same amount as the items, but in the opposite direction\n" +
				"-1: no movement\n" +
				" 0: same speed (together with the items)\n" +
				" 1: 2x faster in the same direction\n" +
				" 2: 3x faster etc.")]
			[Range(-5f, 5f)]
			[SerializeField]
			float _ContentVisualParallaxEffect = -.85f;
			public float ContentVisualParallaxEffect { get { return _ContentVisualParallaxEffect; } set { _ContentVisualParallaxEffect = value; } }

			/// <summary>Applies 1 scale for the item in the middle and gradually lowers the scale of the side items. 0=no effect, 1=the most sideways items will have 0 scale. <see cref="galleryEffectViewportPivot"/> can be used to apply scaling weight in other place than the middle</summary>
			[Range(0f, 1f)]
			public float galleryEffectAmount = 0f;

			/// <summary>0=start, 1=end</summary>
			[Range(0f, 1f)]
			public float galleryEffectViewportPivot = .5f;


			public bool HasContentVisual { get { return _HasContentVisual; } }

			bool _HasContentVisual;


			public void InitIfNeeded()
			{
				_HasContentVisual = contentVisual != null;

				float maxAllowed;
				string asString;
				if (loopItems)
				{
					maxAllowed = MAX_SPEED_IF_LOOPING;
					asString = "MAX_SPEED_IF_LOOPING";
				}
				else
				{
					maxAllowed = MAX_SPEED;
					asString = "MAX_SPEED";
				}
				float maxSpeedClamped = Mathf.Clamp(maxSpeed, 0f, maxAllowed);
				if (Math.Abs(maxSpeedClamped - maxSpeed) > 1f)
				{
					Debug.Log("OSA: maxSpeed(" + maxSpeed.ToString("#########.00") + ") value is negative or exceeded "+ asString + "(" +
						maxAllowed.ToString("#########.00") +
						"). Clamped it to " + maxSpeedClamped.ToString("#########.00")
					);
					maxSpeed = maxSpeedClamped;
				}

				if (elasticMovement && loopItems)
				{
					elasticMovement = false;
					Debug.Log("OSA: 'elasticMovement' was set to false, because 'loopItems' is true. Elasticity only makes sense when there is an end");
				}

				if (HasContentVisual)
					contentVisual.rectTransform.MatchParentSize(true);
			}
		}


		[Serializable]
		public class Optimization
		{
			/// <summary>
			/// <para>How much objects besides the visible ones to keep in memory at max, besides the visible ones</para>
			/// <para>By default, no more than the heuristically found "ideal" number of items will be held in memory</para>
			/// <para>Set to a positive integer to limit it - Not recommended, unless you're OK with more GC calls (i.e. occasional FPS hiccups) in favor of using less RAM</para>
			/// </summary>
			[Tooltip("How much objects besides the visible ones to keep in memory at max. \n" +
				"By default, no more than the heuristically found \"ideal\" number of items will be held in memory.\n" +
				"Set to a positive integer to limit it - Not recommended, unless you're OK with more GC calls (i.e. occasional FPS hiccups) in favor of using less RAM")]
			public int recycleBinCapacity = -1;

			/// <summary>
			/// Enables ability to scale out-of-view objects to zero instead of de-activating them, 
			/// since GameObject.SetActive is slightly more expensive to call each frame (especially when scrolling via the scrollbar). 
			/// This is not a major speed improvement, but rather a slight memory improvement. 
			/// It's recommended to use this option if your game/business logic doesn't require the game objects to be de-activated.
			/// </summary>
			public bool scaleToZeroInsteadOfDisable = false;

			// Not implemented yet
			///// <summary>
			///// <para>The bigger, the more items will be active past the minimum needed to fill the viewport - with a performance cost, of course</para>
			///// <para>1f = generally, the number of visible items + 1 will always be active</para>
			///// <para>2f = twice the number of visible items + 1 will be always active</para>
			///// <para>2.5f = 2.5 * (the number of visible items) + 1 will be always active</para>
			///// </summary>
			//[Range(1f, 5f)]
			//public float recyclingToleranceFactor = 1f;

			///// <summary>See <see cref="UpdateMode"/> enum for full description. The default is <see cref="UpdateModeEnum.ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE"/> and if the framerate is acceptable, it should be leaved this way</summary>
			//[Tooltip("See BaseParams.UpdateMode enum for full description. The default is ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE and if the framerate is acceptable, it should be leaved this way")]
			//public UpdateModeEnum updateMode = UpdateModeEnum.ON_SCROLL_THEN_MONOBEHAVIOUR_UPDATE;
		}
	}
}
