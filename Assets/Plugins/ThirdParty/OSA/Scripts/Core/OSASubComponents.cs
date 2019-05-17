//#define DEBUG_COMPUTE_VISIBILITY

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using frame8.Logic.Misc.Other.Extensions;
using frame8.Logic.Misc.Other;
using frame8.Logic.Misc.Visual.UI.MonoBehaviours;

namespace Com.TheFallenGames.OSA.Core
{
	public abstract partial class OSA<TParams, TItemViewsHolder> : MonoBehaviour, IOSA
	where TParams : BaseParams
	where TItemViewsHolder : BaseItemViewsHolder
	{
		/// <summary>
		/// Contains cached variables, helper methods and generally things that are not exposed to inheritors. Note: the LayoutGroup component on content, if any, will be disabled.
		/// <para>Comments format: value if vertical scrolling/value if horizontal scrolling</para>
		/// </summary>
		class InternalState
		{
			#region Fields & Props
			// Constant params (until the scrollview size changes)
			//public readonly double proximityToLimitNeeded01ToResetPos = .95d;
			internal double ProximityToLimitNeeded01ToResetPos { get { return _SourceParams.effects.elasticMovement ? 1d : .9999995d; } }
			internal readonly Vector2 constantAnchorPosForAllItems = new Vector2(0f, 1f); // top-left
			internal double vpSize;
			internal double paddingContentStart; // top/left
			internal double transversalPaddingContentStart; // left/top
			internal double paddingContentEnd; // bottom/right
			internal double paddingStartPlusEnd;
			internal double spacing;
			internal RectTransform.Edge startEdge; // RectTransform.Edge.Top/RectTransform.Edge.Left
			internal RectTransform.Edge endEdge; // RectTransform.Edge.Bottom/RectTransform.Edge.Right
			internal RectTransform.Edge transvStartEdge; // RectTransform.Edge.Left/RectTransform.Edge.Top

			// Cache params
			internal double lastProcessedCTVirtualInsetFromVPS;
			internal double ctVirtualInsetFromVPS_Cached { get; private set; } // todo set back to field
																			   //internal double ctVirtualInsetFromVPS_Cached_NotConsideringNegativeVSA { get { return VirtualScrollableArea > 0 ? ctVirtualInsetFromVPS_Cached : 0d; } }
			internal Vector2 scrollViewSize;
			//internal float ctRealSize; // height/width // same as vpSize for now
			internal double ctVirtualSize; // height/width
										   //internal bool updateRequestPending;
			internal bool computeVisibilityTwinPassScheduled;
			internal bool preferKeepingCTEndEdgeStationaryInNextComputeVisibilityTwinPass;
			internal bool lastComputeVisibilityHadATwinPass;
			//internal Func<RectTransform, float> getRTCurrentSizeFn;
			internal int hor0_vert1, hor1_vertMinus1;

			internal bool HasScrollViewSizeChanged { get { return scrollViewSize != _SourceParams.ScrollViewRT.rect.size; } }
			internal double CTVirtualInsetFromVPE_Cached { get { return -ctVirtualSize + vpSize - ctVirtualInsetFromVPS_Cached; } }
			internal double VirtualScrollableArea { get { return ctVirtualSize - vpSize; } } // negative/zero when all the content is inside vp, positive else
			internal double AbstractPivot01 { get { return hor0_vert1 + hor1_vertMinus1 * _SourceParams.Content.pivot[hor0_vert1]; } }

			ItemsDescriptor _ItemsDesc;
			TParams _SourceParams;
			#endregion

			internal static InternalState CreateFromSourceParamsOrThrow(TParams sourceParams, ItemsDescriptor itemsDescriptor)
			{
				return new InternalState(sourceParams, itemsDescriptor);
			}

			protected InternalState(TParams sourceParams, ItemsDescriptor itemsDescriptor)
			{
				_SourceParams = sourceParams;
				_ItemsDesc = itemsDescriptor;

				var lg = sourceParams.Content.GetComponent<LayoutGroup>();
				if (lg && lg.enabled)
				{
					lg.enabled = false;
					Debug.Log("LayoutGroup on GameObject " + lg.name + " has beed disabled in order to use OSA");
				}

				var contentSizeFitter = sourceParams.Content.GetComponent<ContentSizeFitter>();
				if (contentSizeFitter && contentSizeFitter.enabled)
				{
					contentSizeFitter.enabled = false;
					Debug.Log("ContentSizeFitter on GameObject " + contentSizeFitter.name + " has beed disabled in order to use OSA");
				}

				var layoutElement = sourceParams.Content.GetComponent<LayoutElement>();
				if (layoutElement)
				{
					GameObject.Destroy(layoutElement);
					Debug.Log("LayoutElement on GameObject " + contentSizeFitter.name + " has beed DESTROYED in order to use OSA");
				}

				if (sourceParams.IsHorizontal)
				{
					startEdge = RectTransform.Edge.Left;
					endEdge = RectTransform.Edge.Right;
					transvStartEdge = RectTransform.Edge.Top;
					//getRTCurrentSizeFn = root => root.rect.width;
				}
				else
				{
					startEdge = RectTransform.Edge.Top;
					endEdge = RectTransform.Edge.Bottom;
					transvStartEdge = RectTransform.Edge.Left;
					//getRTCurrentSizeFn = root => root.rect.height;
				}

				_SourceParams.UpdateContentPivotFromGravityType();

				CacheScrollViewInfo();
			}


			internal void CacheScrollViewInfo()
			{
				scrollViewSize = _SourceParams.ScrollViewRT.rect.size;
				RectTransform vpRT = _SourceParams.Viewport;
				Rect vpRect = vpRT.rect;
				Rect ctRect = _SourceParams.Content.rect;
				double ctH = ctRect.height, ctW = ctRect.width;


				if (_SourceParams.IsHorizontal)
				{
					hor0_vert1 = 0;
					hor1_vertMinus1 = 1;
					vpSize = vpRect.width;
					//ctRealSize = ctW;
					paddingContentStart = _SourceParams.ContentPadding.left;
					paddingContentEnd = _SourceParams.ContentPadding.right;
					transversalPaddingContentStart = _SourceParams.ContentPadding.top;
					_ItemsDesc.itemsConstantTransversalSize = ctH - (transversalPaddingContentStart + _SourceParams.ContentPadding.bottom);
				}
				else
				{
					hor0_vert1 = 1;
					hor1_vertMinus1 = -1;
					vpSize = vpRect.height;
					//ctRealSize = ctH;
					paddingContentStart = _SourceParams.ContentPadding.top;
					paddingContentEnd = _SourceParams.ContentPadding.bottom;
					transversalPaddingContentStart = _SourceParams.ContentPadding.left;
					_ItemsDesc.itemsConstantTransversalSize = ctW - (transversalPaddingContentStart + _SourceParams.ContentPadding.right);
				}

				spacing = _SourceParams.ContentSpacing;

				// There's no concept of content start/end padding when looping. instead, the spacing amount is appended before+after the first+last item
				if (_SourceParams.effects.loopItems && (paddingContentStart != spacing || paddingContentEnd != spacing))
					throw new UnityException(
						"OSA: When looping is active, the content padding should be the same as content spacing. " +
						"This is handled automatically in Params.InitIfNeeded(). " +
						"If you overrode method, please call base's implementation first"
					);
				//paddingContentStart = paddingContentEnd = spacing;

				paddingStartPlusEnd = paddingContentStart + paddingContentEnd;
			}

			//internal void CorrectPositionsBasedOnCachedCTInsetFromVPS(List<TItemViewsHolder> vhs, bool alsoCorrectTransversalPositioning)//, bool itemEndEdgeStationary)
			//{
			//	// Update the positions of the provided vhs so they'll retain their position relative to the viewport
			//	TItemViewsHolder vh;
			//	int count = vhs.Count;

			//	double insetStartOfCurItem = GetItemVirtualInsetFromParentStartUsingItemIndexInView(vhs[0].itemIndexInView);
			//	float curSize;
			//	float realInset;
			//	for (int i = 0; i < count; ++i)
			//	{
			//		vh = vhs[i];
			//		curSize = _ItemsDesc[vh.itemIndexInView];
			//		realInset = ConvertItemInsetFromParentStart_FromVirtualToInferredReal(insetStartOfCurItem);
			//		vh.root.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(
			//			_SourceParams.Content,
			//			startEdge,
			//			realInset,
			//			curSize
			//		);
			//		insetStartOfCurItem += curSize + spacing;

			//		if (alsoCorrectTransversalPositioning && realInset >= 0f && realInset < viewportSize)
			//			vh.root.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(transvStartEdge, transversalPaddingContentStart, _ItemsDesc.itemsConstantTransversalSize);
			//	}
			//}

			// Gives a consistent value regardless if horizontal or vertical scrollview (1 = start, 0 = end)
			public Vector2 GetPointerPositionInCTSpace(PointerEventData currentPointerEventData)
			{
				return UIUtils8.Instance.ScreenPointToLocalPointInRectangle(_SourceParams.Content, currentPointerEventData);
			}

			//public Vector2 GetVectorInCTSpaceFrom(Vector2 startPosInCTSpace, PointerEventData currentPointerEventData)
			//{
			//	Vector2 curLocalPos = GetPointInCTSpaceFrom(currentPointerEventData);
			//	return curLocalPos - startPosInCTSpace;
			//}

			public double GetCTAbstractSpaceVectorLongitudinalComponentFromCTSpaceVector(Vector2 vectorCTSpace)
			{
				double abstrDeltaInCTSpace = (double)vectorCTSpace[hor0_vert1] * hor1_vertMinus1;

				return abstrDeltaInCTSpace;
			}

			internal double CalculateContentVirtualSize() { return _ItemsDesc.CumulatedSizeOfAllItems + spacing * Math.Max(0, _ItemsDesc.itemsCount - 1) + paddingStartPlusEnd; }

			// Don't abuse this! It's only used when the items' sizes have externally changed and thus we don't know if their 
			// positions remained the same or not (most probably, not)
			internal void CorrectPositions(List<TItemViewsHolder> vhs, bool alsoCorrectTransversalPositioning)//, bool itemEndEdgeStationary)
			{
				// Update the positions of the provided vhs so they'll retain their position relative to the viewport
				TItemViewsHolder vh;
				int count = vhs.Count;
				//var edge = itemEndEdgeStationary ? endEdge : startEdge;
				//Func<int, float> getInferredRealOffsetFromParentStartOrEndFn;
				//if (itemEndEdgeStationary)
				//	getInferredRealOffsetFromParentStartOrEndFn = GetItemInferredRealOffsetFromParentEnd;
				//else
				//	getInferredRealOffsetFromParentStartOrEndFn = GetItemInferredRealOffsetFromParentStart;



				//double insetStartOfCurItem = GetItemVirtualInsetFromParentStartUsingItemIndexInView(vhs[0].itemIndexInView);
				double insetStartOfCurItem = GetItemInferredRealInsetFromParentStart(vhs[0].itemIndexInView);
				double curSize;

				//Debug.Log("CorrectPositions:" + vhs[0].ItemIndex + " to " + vhs[vhs.Count-1].ItemIndex);
				for (int i = 0; i < count; ++i)
				{
					vh = vhs[i];
					curSize = _ItemsDesc[vh.itemIndexInView];
					vh.root.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(
						startEdge,
						//ConvertItemInsetFromParentStart_FromVirtualToInferredReal(insetStartOfCurItem),
						(float)insetStartOfCurItem,
						(float)curSize
					);
					insetStartOfCurItem += curSize + spacing;

					if (alsoCorrectTransversalPositioning)
						// Transversal float precision doesn't matter
						vh.root.SetInsetAndSizeFromParentEdgeWithCurrentAnchors(transvStartEdge, (float)transversalPaddingContentStart, (float)_ItemsDesc.itemsConstantTransversalSize);
				}
			}

			internal void UpdateLastProcessedCTVirtualInsetFromVPStart() { lastProcessedCTVirtualInsetFromVPS = ctVirtualInsetFromVPS_Cached; }

			/// <summary> See the <see cref="OSA{TParams, TItemViewsHolder}.GetVirtualAbstractNormalizedScrollPosition"/> for documentation</summary>
			internal double GetVirtualAbstractNormalizedScrollPosition()
			{
				var vsa = VirtualScrollableArea;
				if (vsa <= 0) // vp bigger than- or equal (avoiding div by zero below) to ct
					return 1d;

				var insetClamped = Math.Min(0d, ctVirtualInsetFromVPS_Cached);
				return 1d + insetClamped / vsa;
			}

			internal void UpdateCachedCTVirtInsetFromVPS(double virtualInset, bool allowOutsideBounds)
			{
				if (!allowOutsideBounds)
				{
					double maxInsetStart, minInsetStart;
					double emptyArea = -VirtualScrollableArea;
					if (emptyArea > 0d)
					{
						//maxInsetStart = GetTargetCTVirtualInsetFromVPSWhenCTSmallerThanVP(emptyArea);
						//double maxInsetEnd = vpSize - (ctVirtualSize + maxInsetStart);
						//minInsetStart = vpSize - (ctVirtualSize + maxInsetEnd);
						minInsetStart = maxInsetStart = GetTargetCTVirtualInsetFromVPSWhenCTSmallerThanVP(emptyArea);
					}
					else
					{
						double vsa = VirtualScrollableArea;
						maxInsetStart = 0d;
						minInsetStart = -vsa;
					}

					if (minInsetStart > maxInsetStart)
						throw new UnityException(string.Format("[OSAInternal] Clamping content offset failed: minInsetStart(={0}) > maxInsetStart(-{1})", minInsetStart, maxInsetStart));

					virtualInset = Math.Max(minInsetStart, Math.Min(maxInsetStart, virtualInset));
				}
				//double prev = ctVirtualInsetFromVPS_Cached;

				// This is the only place the ct inset should be changed. 
				ctVirtualInsetFromVPS_Cached = virtualInset;

				// TODO see if needed
				//Canvas.ForceUpdateCanvases();
				RebuildLayoutImmediateCompat(_SourceParams.ScrollViewRT);

				//return ctVirtualInsetFromVPS_Cached - prev;
			}

			internal double GetItemVirtualInsetFromParentStartUsingItemIndexInView(int itemIndexInView)
			{
				double cumulativeSizeOfAllItemsBeforePlusSpacing = 0d;
				if (itemIndexInView > 0)
					cumulativeSizeOfAllItemsBeforePlusSpacing = _ItemsDesc.GetItemSizeCumulative(itemIndexInView - 1) + itemIndexInView * spacing;

				var inset = paddingContentStart + cumulativeSizeOfAllItemsBeforePlusSpacing;

				//double emptyAreaWhenCTSmallerThanVP = -VirtualScrollableArea;
				//if (emptyAreaWhenCTSmallerThanVP > 0)
				//	inset += GetTargetCTVirtualInsetFromVPSWhenCTSmallerThanVP(emptyAreaWhenCTSmallerThanVP);

				return inset;
			}

			internal double GetItemVirtualInsetFromParentEndUsingItemIndexInView(int itemIndexInView)
			{ return ctVirtualSize - GetItemVirtualInsetFromParentStartUsingItemIndexInView(itemIndexInView) - _ItemsDesc[itemIndexInView]; }

			internal double GetItemInferredRealInsetFromParentStart(int itemIndexInView)
			{ return ConvertItemInsetFromParentStart_FromVirtualToInferredReal(GetItemVirtualInsetFromParentStartUsingItemIndexInView(itemIndexInView)); }

			internal double GetItemInferredRealInsetFromParentEnd(int itemIndexInView)
			{ return vpSize - GetItemInferredRealInsetFromParentStart(itemIndexInView) - _ItemsDesc[itemIndexInView]; }

			internal double GetContentInferredRealInsetFromVPS(TItemViewsHolder firstVH)
			{ return firstVH.root.GetInsetFromParentEdge(_SourceParams.Content, startEdge) - paddingContentStart; }

			internal double GetContentInferredRealInsetFromVPE(TItemViewsHolder lastVH)
			{ return lastVH.root.GetInsetFromParentEdge(_SourceParams.Content, endEdge) - paddingContentEnd; }

			//internal double ConvertItemOffsetFromParentStart_FromRealToVirtual(float realOffsetFromParrentStart)
			//{ return -contentPanelSkippedInsetDueToVirtualization + realOffsetFromParrentStart; }
			internal double ConvertItemInsetFromParentStart_FromVirtualToInferredReal(double virtualInsetFromParrentStart)
			{ return ctVirtualInsetFromVPS_Cached + virtualInsetFromParrentStart; }

			// This assumes vsa is negative
			internal double GetTargetCTVirtualInsetFromVPSWhenCTSmallerThanVP()
			{
				double emptyAreaWhenCTSmallerThanVP = -VirtualScrollableArea;
				return GetTargetCTVirtualInsetFromVPSWhenCTSmallerThanVP(emptyAreaWhenCTSmallerThanVP);
			}

			internal double GetTargetCTVirtualInsetFromVPSWhenCTSmallerThanVP(double emptyAreaWhenCTSmallerThanVP)
			{
				var target = AbstractPivot01 * emptyAreaWhenCTSmallerThanVP;

				return target;
			}

			internal void CorrectParametersOnCTSizeChange(bool contentPanelEndEdgeStationary, out double? ctInsetFromVPSOverride, ref double additionalCTDragAbstrDelta, double newCTSize, double deltaSize)
			{
				if (deltaSize < 0) // shrinking
				{
					double newVirtualizedAmount = newCTSize - vpSize;
					double emptyAreaInViewport = -newVirtualizedAmount;
					// In case the ct is smaller than vp, we set the inset from start manually, as it's done when correcting the position according to pivot, in late update
					if (emptyAreaInViewport > 0)
					{
						ctInsetFromVPSOverride = GetTargetCTVirtualInsetFromVPSWhenCTSmallerThanVP(emptyAreaInViewport);
						return;
					}

					double cut = -deltaSize;
					if (contentPanelEndEdgeStationary)
					{
						double contentAmountBeforeVP = -ctVirtualInsetFromVPS_Cached;
						if (contentAmountBeforeVP < 0d)
						{
							ctInsetFromVPSOverride = 0d;
							additionalCTDragAbstrDelta = contentAmountBeforeVP - cut;
						}
						else
						{
							double cutAmountInsideVP = cut - contentAmountBeforeVP;
							if (cutAmountInsideVP >= 0d)
							{
								// Commented: the non-virtualized ct case is handled before
								//if (vpSize > newCTSize)
								//{
								//	ctInsetFromVPSOverride = vpSize - newCTSize;
								//	double uncutAmountInsideVP = vpSize - cutAmountInsideVP;
								//	double contentAmountAfterVP = newCTSize - uncutAmountInsideVP;
								//	additionalCTDragAbstrDelta = -contentAmountAfterVP;
								//}
								//else
								//{
								//	ctInsetFromVPSOverride = 0d;
								//	additionalCTDragAbstrDelta = -cutAmountInsideVP;
								//}
								ctInsetFromVPSOverride = 0d;
								additionalCTDragAbstrDelta = -cutAmountInsideVP;
							}
							else
								ctInsetFromVPSOverride = null;
						}
						//Debug.Log("contentAmountBeforeVP:" + contentAmountBeforeVP + ", additionalCTDragAbstrDelta=" + additionalCTDragAbstrDelta);
					}
					else
					{
						double contentAmountAfterVP = -CTVirtualInsetFromVPE_Cached;

						if (contentAmountAfterVP < 0d)
						{
							ctInsetFromVPSOverride = vpSize - newCTSize;
							additionalCTDragAbstrDelta = -contentAmountAfterVP + cut;
						}
						else
						{
							double cutAmountInsideVP = cut - contentAmountAfterVP;
							if (cutAmountInsideVP >= 0d)
							{
								// Commented: the non-virtualized ct case is handled before
								//if (vpSize > newCTSize)
								//{
								//	ctInsetFromVPSOverride = 0d;
								//	additionalCTDragAbstrDelta = -ctVirtualInsetFromVPS_Cached;
								//}
								//else
								//{
								//	ctInsetFromVPSOverride = vpSize - newCTSize;
								//	additionalCTDragAbstrDelta = cutAmountInsideVP;
								//}
								ctInsetFromVPSOverride = vpSize - newCTSize;
								additionalCTDragAbstrDelta = cutAmountInsideVP;
							}
							else
								ctInsetFromVPSOverride = null;
						}
						//Debug.Log("contentAmountAfterVP:" + contentAmountAfterVP + ", additionalCTDragAbstrDelta=" + additionalCTDragAbstrDelta);
					}
				}
				else
				{
					ctInsetFromVPSOverride = null;
				}
			}

			internal void RebuildLayoutImmediateCompat(RectTransform rectTransform)
			{
				//Canvas.ForceUpdateCanvases();
				LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
			}
		}


		struct ContentSizeOrPositionChangeParams
		{
			public bool cancelSnappingIfAny,
			fireScrollPositionChangedEvent,
			keepVelocity,
			allowOutsideBounds, // bounds are VPS/VPE when virtualizing and "ContentInferredStart_AccordingToPivot"/"ContentInferredEnd_AccordingToPivot" when not
			contentEndEdgeStationary;
			internal double? contentInsetOverride;

			public ComputeVisibilityParams computeVisibilityParams;
		}


		// Assigned def values to avoid compiler warnings
		class ComputeVisibilityParams
		{
			public double? overrideDelta = null;
			public bool potentialTwinPassCTEndStationaryPrioritizeUserPreference = false;
			public bool forceFireScrollPositionChangedEvent = false;
		}
		

		class ReleaseFromPullManager
		{
			public bool inProgress;
			//public RectTransform.Edge pulledEdge;
			public double targetCTInsetFromVPS;


			OSA<TParams, TItemViewsHolder> _Adapter;
			ComputeVisibilityParams _ComputeVisibilityParams_Reusable = new ComputeVisibilityParams();


			public ReleaseFromPullManager(OSA<TParams, TItemViewsHolder> adapter) { _Adapter = adapter; }


			public double CalculateFirstItemTargetInsetFromVPS() { return targetCTInsetFromVPS + _Adapter._InternalState.paddingContentStart; }

			public double CalculateFirstItemInsetFromVPS()
			{
				var firstVH = _Adapter._VisibleItems[0];
				//float firstItemInsetFromVPS = _VisibleItems[0].root.GetInsetFromParentEdge(Parameters.content, _InternalState.startEdge);
				double firstItemInsetFromVPS = firstVH.root.GetInsetFromParentEdge(_Adapter._Params.Content, _Adapter._InternalState.startEdge);
				if (firstVH.itemIndexInView > 0)
					firstItemInsetFromVPS -= _Adapter._InternalState.GetItemVirtualInsetFromParentStartUsingItemIndexInView(firstVH.itemIndexInView) - _Adapter._InternalState.paddingContentStart;

				return firstItemInsetFromVPS;
			}

			// Only call this if in progress
			public void FinishNowByDraggingItems(bool computeVisibility)
			{
				if (!inProgress)
					return;

				var abstrDelta = CalculateFirstItemTargetInsetFromVPS() - CalculateFirstItemInsetFromVPS();
				if (abstrDelta != 0d)
					_Adapter.DragVisibleItemsRangeUnchecked(0, _Adapter._VisibleItemsCount, abstrDelta, true, computeVisibility);

				inProgress = false;
			}

			public void FinishNowBySettingContentInset(bool computeVisibility)
			{
				// Don't let it infer the delta, since we already know its value
				_ComputeVisibilityParams_Reusable.overrideDelta = targetCTInsetFromVPS - _Adapter._InternalState.ctVirtualInsetFromVPS_Cached;
				var contentPosChangeParams = new ContentSizeOrPositionChangeParams
				{
					cancelSnappingIfAny = true,
					computeVisibilityParams = computeVisibility ? _ComputeVisibilityParams_Reusable : null,
					fireScrollPositionChangedEvent = true,
					keepVelocity = true,
				};

				bool _;
				_Adapter.SetContentVirtualInsetFromViewportStart(targetCTInsetFromVPS, ref contentPosChangeParams, out _);

				inProgress = false;
			}
		}


		class ComputeVisibilityManager
		{
			OSA<TParams, TItemViewsHolder> _Adapter;
			TParams _Params;
			InternalState _InternalState;
			ItemsDescriptor _ItemsDesc;

			#region Per-ComputeVisibility call params
			string debugString;
			bool negativeScroll;
			double vpSize;
			RectTransform.Edge negStartEdge_posEndEdge;
			TItemViewsHolder nlvHolder = null;
			int endItemIndexInView,
				  neg1_posMinus1,
				  neg1_pos0,
				  neg0_pos1;
			double ctVrtInsetFromVPS;
			int itemsCount;
			//int estimatedAVGVisibleItems;
			bool thereWereVisibletems;
			//int currentLVItemIndexInView;
			double currentItemRealInset_negStart_posEnd;
			bool negNLVCandidateIsBeforeVP_posNLVCandidateIsAfterVP;
			// The item that was the last in the _VisibleItems (first, if pos scroll); We're inferring the positions of the other ones after(below/to the right, depending on hor/vert scroll) it this way, since the heights(widths for hor scroll) are known
			TItemViewsHolder startingLVHolder = null;
			double negCTVrtInsetFromVPS_posCTVrtInsetFromVPE;
			int nlvIndexInView;
			double nlvSize, nlvSizePlusSpacing;
			double currentRealInsetToUseForNLV_negFromCTS_posFromCTE;
			#endregion


			public ComputeVisibilityManager(OSA<TParams, TItemViewsHolder> adapter)
			{
				_Adapter = adapter;
				_Params = _Adapter._Params;
				_InternalState = _Adapter._InternalState;
				_ItemsDesc = _Adapter._ItemsDesc;
			}


			/// <summary>The very core of <see cref="OSA{TParams, TItemViewsHolder}"/>. You must be really brave if you think about trying to understand it :)</summary>
			public void ComputeVisibility(double abstractDelta)
			{
				#region visualization & info
				// ALIASES:
				// scroll down = the content goes down(the "view point" goes up); scroll up = analogue
				// the notation "x/y" means "x, if vertical scroll; y, if horizontal scroll"
				// positive scroll = down/right; negative scroll = up/left
				// [start] = usually refers to some point above/to-left-of [end], if negativeScroll; 
				//          else, [start] is below/to-right-of [end]; 
				//          for example: -in context of _VisibleItems, [start] is 0 for negativeScroll and <_VisibleItemsCount-1> for positiveScroll;
				//                       -in context of an item, [start] is its top for negativeScroll and bottom for positiveScroll;
				//                       - BUT for ct and vp, they have fixed meaning, regardless of the scroll sign. they only depend on scroll direction (if vert, start = top, end = bottom; if hor, start = left, end = right)
				// [end] = inferred from definition of [start]
				// LV = last visible (the last item that was closest to the negVPEnd_posVPStart in the prev call of this func - if applicable)
				// NLV = new last visible (the next one closer to the negVPEnd_posVPStart than LV)
				// neg = negative scroll (down or right)
				// pos =positive scroll (up or left)
				// ch = child (i.e. ctChS = content child start(first child) (= ct.top - ctPaddingTop, in case of vertical scroll))

				// So, again, this is the items' start/end notions! Viewport's and Content's start/end are constant throughout the session
				// Assume the following scroll direction (hor) and sign (neg) (where the VIEWPORT+SCROLLBAR goes, opposed to where the CONTENT goes):
				// hor, negative:
				// O---------------->
				//      [vpStart]  [start]item[end] .. [start]item2[end] .. [start]LVItem[end] [vpEnd]
				// hor, positive:
				// <----------------O
				//      [vpStart]  [end]item[start] .. [end]item2[start] .. [end]LVItem[start] [vpEnd]
				#endregion

				debugString = null;
				negativeScroll = abstractDelta <= 0d;

				// Viewport constant values
				vpSize = _InternalState.vpSize;

				// Items variable values

				if (negativeScroll)
				{
					neg1_posMinus1 = 1;
					negStartEdge_posEndEdge = _InternalState.startEdge;
				}
				else
				{
					neg1_posMinus1 = -1;
					negStartEdge_posEndEdge = _InternalState.endEdge;
				}
				neg1_pos0 = (neg1_posMinus1 + 1) / 2;
				neg0_pos1 = 1 - neg1_pos0;

				thereWereVisibletems = _Adapter._VisibleItemsCount > 0;

				itemsCount = _ItemsDesc.itemsCount;

				// _InternalParams.itemsCount - 1, if negativeScroll
				// 0, else
				endItemIndexInView = neg1_pos0 * (itemsCount - 1);

				ctVrtInsetFromVPS = _InternalState.ctVirtualInsetFromVPS_Cached;
				negCTVrtInsetFromVPS_posCTVrtInsetFromVPE = negativeScroll ? ctVrtInsetFromVPS : (-_InternalState.ctVirtualSize + _InternalState.vpSize - ctVrtInsetFromVPS);

#if DEBUG_COMPUTE_VISIBILITY
			debugString += "\n : ctVirtualInsetFromVPS_Cached=" + ctVrtInsetFromVPS + 
				" negCTVrtInsetFromVPS_posCTVrtInsetFromVPE=" + negCTVrtInsetFromVPS_posCTVrtInsetFromVPE +
				", vpSize=" + vpSize;
#endif

				// _VisibleItemsCount is always 0 in the first call of this func after the list is modified.

				// IF _VisibleItemsCount == 0:
				//		-1, if negativeScroll
				//		_InternalParams.itemsCount, else
				// ELSE
				//		indexInView of last visible, if negativeScroll
				//		indexInView of first visible, else
				int currentLVItemIndexInView;

				// Get a list of items that are before(if neg)/after(if pos) viewport and move them from 
				// _VisibleItems to itemsOutsideViewport; they'll be candidates for recycling
				if (thereWereVisibletems)
					PrepareOutsideItemsForPotentialRecycleAndGetNextFirstVisibleIndexInView(out currentLVItemIndexInView);
				else
					//currentLVItemIndexInView = neg0_pos1 * (itemsCount - 1) - neg1_posMinus1;
					currentLVItemIndexInView = neg0_pos1 * (itemsCount - 1);

				if (itemsCount > 0)
				{
					// Optimization: saving a lot of computations (especially visible when fast-scrolling using SmoothScrollTo or dragging the scrollbar) by skipping 
					// GetItemVirtualOffsetFromParent[Start/End]UsingItemIndexInView() calls and instead, inferring the offset along the way after calling that method only for the first item
					// Optimization2: trying to estimate the new FIRST visible item by the current scroll position when jumping large distances
					bool inferredDifferentFirstVisibleVH = false;
					// TODO see if using double instead of int breaks anything, since avg should be double usually
					//int estimatedAVGVisibleItems = -1;
					double estimatedAVGVisibleItems = -1d;
					bool forceInferFirstLVIndexAndInset = !thereWereVisibletems; // always infer if there were no items, because there's not item to use as a base for usual inferring
					if ((forceInferFirstLVIndexAndInset
							|| Math.Abs(abstractDelta) > _InternalState.vpSize * OPTIMIZE_JUMP__MIN_DRAG_AMOUNT_AS_FACTOR_OF_VIEWPORT_SIZE // huge jumps need optimization
						)
						//&& (estimatedAVGVisibleItems = (int)Math.Round(Math.Min(_InternalState.vpSize / (_Params.DefaultItemSize + _InternalState.spacing), _Adapter._AVGVisibleItemsCount)))
						&& (estimatedAVGVisibleItems = Math.Min(_InternalState.vpSize / ((double)_Params.DefaultItemSize + _InternalState.spacing), _Adapter._AVGVisibleItemsCount))
							< itemsCount
					)
					{
						//if (thereWereVisibletems)
						//	Debug.Log(thereWereVisibletems + ", " + _Adapter.VisibleItemsCount + ", " + _Adapter.RecyclableItemsCount);
						inferredDifferentFirstVisibleVH = InferFirstVisibleVHIndexInViewAndInset(ref currentLVItemIndexInView, estimatedAVGVisibleItems);
					}

					// This check is the same as in the loop inside. there won't be no "next" item if the current LV is the last in view (first if positive scroll) 
					//if (currentLVItemIndexInView != endItemIndexInView + neg1_posMinus1)
					if (negativeScroll && currentLVItemIndexInView <= endItemIndexInView || !negativeScroll && currentLVItemIndexInView >= endItemIndexInView)
					{
						// Infinity means it was not set; if set, it means the position was inferred due to big jumps in dragging
						if (!inferredDifferentFirstVisibleVH)
						{
							if (negativeScroll)
								currentItemRealInset_negStart_posEnd = _InternalState.GetItemInferredRealInsetFromParentStart(currentLVItemIndexInView);
							else
								currentItemRealInset_negStart_posEnd = _InternalState.GetItemInferredRealInsetFromParentEnd(currentLVItemIndexInView);

#if DEBUG_COMPUTE_VISIBILITY
						debugString += "\n First: currentItemRealInset_negStart_posEnd=" + currentItemRealInset_negStart_posEnd;
#endif
						}

						// Searching for next item(s) that might get visible in order to update them: towards vpEnd on negativeScroll OR towards vpStart else
						do
						{
							bool b = FindCorrectNLVFromCurrent(currentLVItemIndexInView);
							//if (iterations > 100)
							//	Debug.Log(iterations + ", delta " + abstractDelta + ", found " + b + ", estVisible " + estimatedAVGVisibleItems + ", nlvIndexInView " + nlvIndexInView + ", endItemIndexInView " + endItemIndexInView + ", negScroll " + negativeScroll);
							if (!b)
								break;

							int nlvRealIndex = _ItemsDesc.GetItemRealIndexFromViewIndex(nlvIndexInView);

							// Search for a recyclable holder for current NLV
							// This block remains the same regardless of <negativeScroll> variable, because the items in <itemsOutsideViewport> were already added in an order dependent on <negativeScroll>
							// (they are always from <closest to [start]> to <closest to [end]>)
							nlvHolder = _Adapter.ExtractRecyclableViewsHolderOrCreateNew(nlvRealIndex, nlvSize);

							int vhIndex = neg1_pos0 * _Adapter._VisibleItemsCount;

#if DEBUG_COMPUTE_VISIBILITY
						debugString += "\n InsertVH at #" + vhIndex + " (itemIndex=" + nlvRealIndex + "): nlvSize=" + nlvSize + ", realInset_negStart_posEnd=" + currentItemRealInset_negStart_posEnd;
#endif

							_Adapter.AddViewsHolderAndMakeVisible(nlvHolder, vhIndex, nlvRealIndex, nlvIndexInView, currentItemRealInset_negStart_posEnd, negStartEdge_posEndEdge, nlvSize);

							currentLVItemIndexInView = nlvIndexInView + neg1_posMinus1;
							currentItemRealInset_negStart_posEnd += nlvSizePlusSpacing;
						}
						// Loop until:
						//		- negativeScroll vert/hor: there are no items below/to-the-right-of-the current LV that might need to be made visible
						//		- positive vert/hor: there are no items above/to-the-left-of-the current LV that might need to be made visible
						//while (currentLVItemIndexInView != endItemIndexInView + neg1_posMinus1);
						while (negativeScroll && currentLVItemIndexInView <= endItemIndexInView || !negativeScroll && currentLVItemIndexInView >= endItemIndexInView);

					}
					if (debugString != null)
						Debug.Log("OSA.ComputeVisibility(" + abstractDelta + "): " + debugString);
				}

				// Keep track of the <maximum number of items that were visible since last scroll view size change>, so we can optimize the object pooling process
				// by destroying objects in recycle bin only if the aforementioned number is less than <numVisibleItems + numItemsInRecycleBin>,
				// and of course, making sure at least 1 item is in the bin all the time
				if (_Adapter._VisibleItemsCount > _ItemsDesc.maxVisibleItemsSeenSinceLastScrollViewSizeChange)
					_ItemsDesc.maxVisibleItemsSeenSinceLastScrollViewSizeChange = _Adapter._VisibleItemsCount;

				PostComputeVisibilityCleanRecyclableItems();

				// Last result weighs 9x more than the current result in calculating the AVG to prevent "outliers"
				_Adapter._AVGVisibleItemsCount = _Adapter._AVGVisibleItemsCount * .9d + _Adapter._VisibleItemsCount * .1d;
			}

			//int iterations;
			bool FindCorrectNLVFromCurrent(int currentLVItemIndexInView)
			{
				//iterations = 0;

				nlvIndexInView = currentLVItemIndexInView;
				do
				{
					nlvSize = _ItemsDesc[nlvIndexInView];
					nlvSizePlusSpacing = nlvSize + _InternalState.spacing;
					currentRealInsetToUseForNLV_negFromCTS_posFromCTE = currentItemRealInset_negStart_posEnd;
					negNLVCandidateIsBeforeVP_posNLVCandidateIsAfterVP = currentRealInsetToUseForNLV_negFromCTS_posFromCTE <= -nlvSize;
					if (negNLVCandidateIsBeforeVP_posNLVCandidateIsAfterVP)
					{
						if (nlvIndexInView == endItemIndexInView) // all items are outside viewport => abort
							return false;
					}
					else
					{
						// Next item is after vp(if neg) or before vp (if pos) => no more items will become visible 
						// (this happens usually in the first iteration of this loop inner loop, i.e. negNLVCandidateBeforeVP_posNLVCandidateAfterVP never being true)
						if (currentRealInsetToUseForNLV_negFromCTS_posFromCTE > vpSize)
							return false;

						// At this point, we've found the real nlv: nlvIndex, nlvH and currentTopToUseForNLV(if negativeScroll)/currentBottomToUseForNLV(if upScroll) were correctly assigned
						return true;
					}
					currentItemRealInset_negStart_posEnd += nlvSizePlusSpacing;
					//++iterations;
					nlvIndexInView += neg1_posMinus1;
				}
				while (true);
			}

			void PrepareOutsideItemsForPotentialRecycleAndGetNextFirstVisibleIndexInView(out int startingVHIndexInView)
			{
				// startingLV means the item in _VisibleItems that's the closest to the next one that'll spawn

				int startingLVHolderIndex;

				// startingLVHolderIndex will be:
				// _VisibleItemsCount - 1, if negativeScroll
				// 0, if upScroll
				startingLVHolderIndex = neg1_pos0 * (_Adapter._VisibleItemsCount - 1);
				startingLVHolder = _Adapter._VisibleItems[startingLVHolderIndex];
				//startingLVRT = startingLVHolder.root;

				// Approach name(will be referenced below): (%%%)
				// currentStartToUseForNLV will be:
				// NLV top (= LV bottom - spacing), if negativeScroll
				// NLV bottom (= LV top + spacing), else
				//---
				// More in depth: <down0up1 - startingLVRT.pivot.y> will be
				// -startingLVRT.pivot.y, if negativeScroll
				// 1 - startingLVRT.pivot.y, else
				//---
				// And: 
				// ctSpacing will be subtracted from the value, if negativeScroll
				// added, if upScroll


				// Items variable values; initializing them to the current LV
				startingVHIndexInView = startingLVHolder.itemIndexInView + neg1_posMinus1;

#if DEBUG_COMPUTE_VISIBILITY
				debugString += "\n ThereAreVisibleItems: (starting)currentLVItemIndexInView=" + startingVHIndexInView;
#endif

				bool currentIsOutside;
				//RectTransform curRecCandidateRT;
				double curRecCandidateSizePlusSpacing;

				// vItemHolder is:
				// first in _VisibleItems, if negativeScroll
				// last in _VisibleItems, else
				int curRecCandidateVHIndex = neg0_pos1 * (_Adapter._VisibleItemsCount - 1);
				TItemViewsHolder curRecCandidateVH = _Adapter._VisibleItems[curRecCandidateVHIndex];
				double curVrtInsetFromParentEdge = negativeScroll ? _InternalState.GetItemVirtualInsetFromParentStartUsingItemIndexInView(curRecCandidateVH.itemIndexInView)
																: _InternalState.GetItemVirtualInsetFromParentEndUsingItemIndexInView(curRecCandidateVH.itemIndexInView);
				while (true)
				{
					//// vItemHolder is:
					//// first in _VisibleItems, if negativeScroll
					//// last in _VisibleItems, else
					//int curRecCandidateVHIndex = neg0_pos1 * (_VisibleItemsCount - 1);

					curRecCandidateSizePlusSpacing = _ItemsDesc[curRecCandidateVH.itemIndexInView] + _InternalState.spacing; // major bugfix: 18.12.2016 1:20: must use vItemHolder.ItemIndex INSTEAD of currentLVItemIndex

					// Commented: avoiding some potential loss in precision
					//currentIsOutside = negCTVrtInsetFromVPS_posCTVrtInsetFromVPE + (curVrtInsetFromParentEdge + curRecCandidateSizePlusSpacing) <= 0d;
					currentIsOutside = negCTVrtInsetFromVPS_posCTVrtInsetFromVPE <= -(curVrtInsetFromParentEdge + curRecCandidateSizePlusSpacing);

#if DEBUG_COMPUTE_VISIBILITY
					var realInsetFromParentEdge = negativeScroll ? _InternalState.GetItemInferredRealInsetFromParentStart(curRecCandidateVH.itemIndexInView)
																: _InternalState.GetItemInferredRealInsetFromParentEnd(curRecCandidateVH.itemIndexInView);
					debugString += "\n |---: curRecCandidateVHIndex=" + curRecCandidateVHIndex + 
						", itemIdxView=" + curRecCandidateVH.itemIndexInView + 
						", vrtIinsetFromPar=" + curVrtInsetFromParentEdge + 
						", realIinsetFromPar=" + realInsetFromParentEdge + 
						", outside=" + currentIsOutside;
#endif
					if (currentIsOutside)
					{
						_Adapter._RecyclableItems.Add(curRecCandidateVH);
						_Adapter._VisibleItems.RemoveAt(curRecCandidateVHIndex);
						--_Adapter._VisibleItemsCount;

						if (_Adapter._VisibleItemsCount == 0) // all items that were considered visible are now outside viewport => will need to seek even more below 
							break;
					}
					else
						break; // the current item is INside(not necessarily completely) the viewport

					// if negative, VIs will be removed from start, so the index of the "next" stays constantly at 0; 
					// if positive, the index of the "next" is decremented by one, because it starts at end and the list is always shortened by 1
					curRecCandidateVHIndex -= neg0_pos1;

					curVrtInsetFromParentEdge += curRecCandidateSizePlusSpacing;
					curRecCandidateVH = _Adapter._VisibleItems[curRecCandidateVHIndex];
				}
			}

			//bool InferFirstVisibleVHIndexInViewAndInset(ref int currentInferredFirstVisibleVHIndexInView, int estimatedAVGVisibleItems)
			bool InferFirstVisibleVHIndexInViewAndInset(ref int currentInferredFirstVisibleVHIndexInView, double estimatedAVGVisibleItems)
			{
				int initialEstimatedIndexInViewOfNewFirstVisible = (int)
					Math.Round(
						(1d - _InternalState.GetVirtualAbstractNormalizedScrollPosition()) * ((itemsCount - 1) - neg1_pos0 * estimatedAVGVisibleItems)
					);
				initialEstimatedIndexInViewOfNewFirstVisible = Math.Max(0, Math.Min(itemsCount - 1, initialEstimatedIndexInViewOfNewFirstVisible));

				int index = initialEstimatedIndexInViewOfNewFirstVisible;
				double itemSize = _ItemsDesc.GetItemSizeOrDefault(index);
				double negRealInsetStart_posRealInsetEnd =
					negativeScroll ?
						_InternalState.GetItemInferredRealInsetFromParentStart(index)
						: _InternalState.GetItemInferredRealInsetFromParentEnd(index);
				
				int firstOutsideBoundsIndex = itemsCount * neg0_pos1 - neg1_pos0; // -1 if neg, itemsCount if pos

				// Go down/right until a visible item is found
				while (negRealInsetStart_posRealInsetEnd <= -itemSize)
				{
					int nextPotentialIndex = index + neg1_posMinus1;
					if (nextPotentialIndex == firstOutsideBoundsIndex)
						break;

					index = nextPotentialIndex;
					itemSize = _ItemsDesc.GetItemSizeOrDefault(index);
					negRealInsetStart_posRealInsetEnd += itemSize + _InternalState.spacing;
				}

				// If the previous loop didnt' execute at all, it means there's a possibility that the searched item may be after the next item (next=to end if neg scrolling)
				if (index == initialEstimatedIndexInViewOfNewFirstVisible)
				{
					// Go up/left until the FIRST visible item is found (i.e. no one visible before it (after it, if positive scroll))
					do
					{
						int nextPotentialIndex = index - neg1_posMinus1; // next actually means before if neg, after if pos
						if (nextPotentialIndex == firstOutsideBoundsIndex)
							break;

						double nextPotentialItemSize = _ItemsDesc.GetItemSizeOrDefault(nextPotentialIndex);
						double nextPotential_negRealInsetStart_posRealInsetEnd = negRealInsetStart_posRealInsetEnd - (nextPotentialItemSize + _InternalState.spacing);

						if (nextPotential_negRealInsetStart_posRealInsetEnd <= -nextPotentialItemSize) // the next is outside VP => the current one is the first visible
							break;
						index = nextPotentialIndex;
						itemSize = nextPotentialItemSize;
						negRealInsetStart_posRealInsetEnd = nextPotential_negRealInsetStart_posRealInsetEnd;
					}
					while (true);

					if (index < 0)
						throw new UnityException("index " + index + ", currentInferredFirstVisibleVHIndexInView " + currentInferredFirstVisibleVHIndexInView);
					if (index >= itemsCount)
						throw new UnityException("index " + index + " >= itemsCount " + itemsCount + ", currentInferredFirstVisibleVHIndexInView " + currentInferredFirstVisibleVHIndexInView);
				}

				if (!thereWereVisibletems ||
					negativeScroll && index >= currentInferredFirstVisibleVHIndexInView || // the index should be bigger if going down/right. if the inferred one is <=, then startingLV.itemIndexInview is reliable 
					!negativeScroll && index <= currentInferredFirstVisibleVHIndexInView // analogous explanation for pos scroll
					// update: also using "=" to prevent caller from calculating the inset himself
				)
				{
					//Debug.Log("est=" + estimatedIndexInViewOfNewFirstVisible + ", def=" + currentLVItemIndexInView + ", actual=" + index);
#if DEBUG_COMPUTE_VISIBILITY
					debugString += "\nOptimizing big jump: currentInferred "+ currentInferredFirstVisibleVHIndexInView.ToString(DEBUG_FLOAT_FORMAT) + 
						" resolvedIndexItemBeforeNLV=" + index + ", initialEstimatedNLVIndex=" + initialEstimatedIndexInViewOfNewFirstVisible;
#endif

					currentInferredFirstVisibleVHIndexInView = index;
					currentItemRealInset_negStart_posEnd = negRealInsetStart_posRealInsetEnd;

					return true;
				}
#if DEBUG_COMPUTE_VISIBILITY
				debugString += "\nOptimizing big jump: already in bounds";
#endif
				return false;
			}

			void PostComputeVisibilityCleanRecyclableItems()
			{
				// Disable all recyclable views
				// Destroy remaining unused views, BUT keep one, so there's always a reserve, instead of creating/destroying very frequently
				// + keep <numVisibleItems + numItemsInRecycleBin> above <_InternalParams.maxVisibleItemsSeenSinceLastScrollViewSizeChange>
				// See GetNumExcessObjects()
				//GameObject go;
				TItemViewsHolder vh;
				for (int i = 0; i < _Adapter._RecyclableItems.Count;)
				{
					vh = _Adapter._RecyclableItems[i];
					if (_Adapter.IsViewsHolderEnabled(vh))
					{
						_Adapter.OnBeforeRecycleOrDisableViewsHolder(vh, -1); // -1 means it'll be disabled, not re-used ATM
						_Adapter.SetViewsHolderDisabled(vh);
					}

					if (_Adapter.ShouldDestroyRecyclableItem(vh, _Adapter.GetNumExcessObjects() > 0))
					{
						GameObject.Destroy(vh.root.gameObject);
						_Adapter._RecyclableItems.RemoveAt(i);
						++_ItemsDesc.destroyedItemsSinceLastScrollViewSizeChange;
					}
					else
						++i;
				}
			}
		}


		protected class NestingManager : IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
		{
			public bool SearchedParentAtLeastOnce { get { return _SearchedAtLeastOnce; } }
			public bool CurrentDragCapturedByParent { get { return _CurrentDragCapturedByParent; } }


			OSA<TParams, TItemViewsHolder> _Adapter;
			InternalState _InternalState;
			IInitializePotentialDragHandler initializePotentialDragHandler;
			IBeginDragHandler beginDragHandler;
			IDragHandler dragHandler;
			IEndDragHandler endDragHandler;
			bool _SearchedAtLeastOnce;
			bool _CurrentDragCapturedByParent;


			public NestingManager(OSA<TParams, TItemViewsHolder> adapter)
			{
				_Adapter = adapter;
				_InternalState = _Adapter._InternalState;
			}


			public bool FindAndStoreNestedParent()
			{
				initializePotentialDragHandler = null;
				beginDragHandler = null;
				dragHandler = null;
				endDragHandler = null;

				var tr = _Adapter.transform;
				// Find the first parent that implements all of the interfaces
				while ((tr = tr.parent) && initializePotentialDragHandler == null)
				{
					initializePotentialDragHandler = tr.GetComponent(typeof(IInitializePotentialDragHandler)) as IInitializePotentialDragHandler;
					if (initializePotentialDragHandler == null)
						continue;

					beginDragHandler = initializePotentialDragHandler as IBeginDragHandler;
					if (beginDragHandler == null)
					{
						initializePotentialDragHandler = null;
						continue;
					}

					dragHandler = initializePotentialDragHandler as IDragHandler;
					if (dragHandler == null)
					{
						initializePotentialDragHandler = null;
						beginDragHandler = null;
						continue;
					}

					endDragHandler = initializePotentialDragHandler as IEndDragHandler;
					if (endDragHandler == null)
					{
						initializePotentialDragHandler = null;
						beginDragHandler = null;
						dragHandler = null;
						continue;
					}
				}

				_SearchedAtLeastOnce = true;

				return initializePotentialDragHandler != null;
			}

			public void OnInitializePotentialDrag(PointerEventData eventData)
			{
				if (!_SearchedAtLeastOnce && !FindAndStoreNestedParent())
					return;

				if (initializePotentialDragHandler == null)
					return;

				initializePotentialDragHandler.OnInitializePotentialDrag(eventData);
			}

			public void OnBeginDrag(PointerEventData eventData)
			{
				if (initializePotentialDragHandler == null)
				{
					_CurrentDragCapturedByParent = false;
					return;
				}

				if (_Adapter.Parameters.DragEnabled)
				{
					var delta = eventData.delta;
					float dyExcess = Mathf.Abs(delta.y) - Mathf.Abs(delta.x);

					_CurrentDragCapturedByParent = _InternalState.hor1_vertMinus1 * dyExcess >= 0f; // parents have priority when dx == dy, since they are supposed to be more important
				}
				else
					// When the child ScrollView has its drag disabled, forward the drag to the parent no matter what
					_CurrentDragCapturedByParent = true;

				if (!_CurrentDragCapturedByParent)
					return;

				beginDragHandler.OnBeginDrag(eventData);
			}

			public void OnDrag(PointerEventData eventData)
			{
				if (initializePotentialDragHandler == null)
					return;

				dragHandler.OnDrag(eventData);
			}

			public void OnEndDrag(PointerEventData eventData)
			{
				if (initializePotentialDragHandler == null)
					return;

				endDragHandler.OnEndDrag(eventData);
				_CurrentDragCapturedByParent = false;
			}
		}


		enum AllowContentOutsideBoundsMode
		{
			DO_NOT_ALLOW,
			ALLOW_IF_OUTSIDE_AMOUNT_SHRINKS,
			ALLOW
		}
	}
}
