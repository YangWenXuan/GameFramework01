using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;

namespace Com.TheFallenGames.OSA.Demos.Hierarchy
{
	/// <summary>Demonstrating a hierarchy view (aka Tree View) implemented with OSA</summary>
	public class HierarchyExample : OSA<MyParams, FileSystemEntryViewsHolder>
	{
		bool _BusyWithAnimation;


		#region OSA implementation
		/// <inheritdoc/>
		protected override FileSystemEntryViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new FileSystemEntryViewsHolder();
			instance.Init(_Params.itemPrefab, itemIndex);
			instance.foldoutButton.onClick.AddListener(() => OnDirectoryFoldOutClicked(instance));

			return instance;
		}

		/// <inheritdoc/>
		protected override void UpdateViewsHolder(FileSystemEntryViewsHolder newOrRecycled)
		{
			// Initialize the views from the associated model
			FileSystemEntryModel model = _Params.flattenedVisibleHierarchy[newOrRecycled.ItemIndex];
			newOrRecycled.UpdateViews(model);
		}
		#endregion

		#region UI
		public bool TryChangeCount(int newCount)
		{
			if (_BusyWithAnimation)
				return false;

			int uniqueID = 0;

			_Params.hierarchyRootNode = CreateRandomNodeModel(ref uniqueID, 0, true, 7);
			_Params.flattenedVisibleHierarchy = new List<FileSystemEntryModel>(_Params.hierarchyRootNode.children);
			ResetItems(_Params.flattenedVisibleHierarchy.Count);

			return true;
		}
		public void CollapseAll()
		{
			if (_BusyWithAnimation)
				return;

			for (int i = 0; i < _Params.flattenedVisibleHierarchy.Count;)
			{
				var m = _Params.flattenedVisibleHierarchy[i];
				m.expanded = false;
				if (m.depth > 1)
				{
					_Params.flattenedVisibleHierarchy.RemoveAt(i);
				}
				else
					++i;
			}
			ResetItems(_Params.flattenedVisibleHierarchy.Count);
		}
		public void ExpandAll()
		{
			if (_BusyWithAnimation)
				return;

			_Params.flattenedVisibleHierarchy = _Params.hierarchyRootNode.GetFlattenedHierarchyAndExpandAll();
			ResetItems(_Params.flattenedVisibleHierarchy.Count);
		}
		#endregion

		void OnDirectoryFoldOutClicked(FileSystemEntryViewsHolder vh)
		{
			if (_BusyWithAnimation)
				return;

			var model = _Params.flattenedVisibleHierarchy[vh.ItemIndex];
			int nextIndex = vh.ItemIndex + 1;
			bool wasExpanded = model.expanded;
			model.expanded = !wasExpanded;
			if (wasExpanded)
			{
				// Remove all following models with bigger depth, until a model with a less than- or equal depth is found
				int i = vh.ItemIndex + 1;
				int count = _Params.flattenedVisibleHierarchy.Count;
				for (; i < count;)
				{
					var m = _Params.flattenedVisibleHierarchy[i];
					if (m.depth > model.depth)
					{
						m.expanded = false;
						++i;
						continue;
					}

					break; // found with depth less than- or equal to the collapsed item
				}

				int countToRemove = i - nextIndex;
				if (countToRemove > 0)
				{
					if (_Params.animatedFoldOut)
						GradualRemove(nextIndex, countToRemove);
					else
					{
						_Params.flattenedVisibleHierarchy.RemoveRange(nextIndex, countToRemove);
						RemoveItems(nextIndex, countToRemove);
					}
				}
			}
			else
			{
				if (model.children.Length > 0)
				{
					if (_Params.animatedFoldOut)
						GradualAdd(nextIndex, model.children);
					else
					{
						_Params.flattenedVisibleHierarchy.InsertRange(nextIndex, model.children);
						InsertItems(nextIndex, model.children.Length);
					}
				}
			}

			// Starting with v4.0, only the newly visible items are updated to keep performance at max, 
			// so we need to update the directory viewsholder manually (most notably, its arrow will change)
			vh.UpdateViews(model);
		}

		void GradualAdd(int index, FileSystemEntryModel[] children) { StartCoroutine(GradualAddOrRemove(index, children.Length, children)); }

		void GradualRemove(int index, int countToRemove) { StartCoroutine(GradualAddOrRemove(index, countToRemove, null)); }

		IEnumerator GradualAddOrRemove(int index, int count, FileSystemEntryModel[] childrenIfAdd)
		{
			_BusyWithAnimation = true;
			int curIndexInChildren = 0;
			int remainingLen = count;
			int divider = Mathf.Min(7, count);
			int maxChunkSize = count / divider;
			int curChunkSize;
			float toWait = .01f;
			var toWaitYieldInstr = new WaitForSeconds(toWait);

			if (childrenIfAdd == null)
			{
				index = index + count - 1;
				while (remainingLen > 0)
				{
					curChunkSize = Math.Min(remainingLen, maxChunkSize);

					int curStartIndex = index - curChunkSize + 1;
					for (int i = index; i >= curStartIndex; --i, --index)
						_Params.flattenedVisibleHierarchy.RemoveAt(i);
					RemoveItems(curStartIndex, curChunkSize);
					remainingLen -= curChunkSize;

					yield return toWaitYieldInstr;
				}
			}
			else
			{
				while (remainingLen > 0)
				{
					curChunkSize = Math.Min(remainingLen, maxChunkSize);

					int curStartIndex = index;
					for (int i = 0; i < curChunkSize; ++i, ++index, ++curIndexInChildren)
						_Params.flattenedVisibleHierarchy.Insert(index, childrenIfAdd[curIndexInChildren]);

					InsertItems(curStartIndex, curChunkSize);
					remainingLen -= curChunkSize;

					yield return toWaitYieldInstr;
				}
			}
			_BusyWithAnimation = false;
		}

		// Just a utility for generating random models. in a real-case scenario, you'd be creating the models from your data source
		FileSystemEntryModel CreateRandomNodeModel(ref int uniqueID, int depth, bool forceDirectory, int numChildren)
		{
			if (forceDirectory || depth + 1 < _Params.maxHierarchyDepth && UnityEngine.Random.Range(0, 2) == 0)
			{
				var m = CreateNewModel(ref uniqueID, depth, true);
				m.children = new FileSystemEntryModel[numChildren];
				bool depth1 = depth == 1;
				for (int i = 0; i < numChildren; ++i)
					m.children[i] = CreateRandomNodeModel(ref uniqueID, depth + 1, depth1, UnityEngine.Random.Range(1, 7));

				return m;
			}

			return CreateNewModel(ref uniqueID, depth, false);
		}

		FileSystemEntryModel CreateNewModel(ref int itemIndex, int depth, bool isDirectory)
		{
			return new FileSystemEntryModel()
			{
				title = (isDirectory ? "Directory " : "File ") + (itemIndex++),
				depth = depth
			};
		}
	}


	// This in almost all cases will contain the prefab and your list of models
	[Serializable] // serializable, so it can be shown in inspector
	public class MyParams : BaseParamsWithPrefab
	{
		[Range(0, 10)]
		public int maxHierarchyDepth;
		public bool animatedFoldOut = true;
		public FileSystemEntryModel hierarchyRootNode;
		public List<FileSystemEntryModel> flattenedVisibleHierarchy; // doesn't include descendants of non-expanded folders
	}


	// File system entry (file or directory)
	public class FileSystemEntryModel
	{
		public FileSystemEntryModel[] children;
		public int depth;
		public string title;
		public bool expanded; // only needed for directories

		public bool IsDirectory { get { return children != null; } }


		public List<FileSystemEntryModel> GetFlattenedHierarchyAndExpandAll()
		{
			var res = new List<FileSystemEntryModel>();
			for (int i = 0; i < children.Length; i++)
			{
				var c = children[i];
				res.Add(c);
				c.expanded = true;
				if (c.IsDirectory)
				{
					res.AddRange(c.GetFlattenedHierarchyAndExpandAll());
				}
			}

			return res;
		}
	}


	public class FileSystemEntryViewsHolder : BaseItemViewsHolder
	{
		public Text titleText;
		public Image foldoutArrowImage;
		public Button foldoutButton;
		Image _FileIconImage, _DirectoryIconImage;
		RectTransform _PanelRT;
		HorizontalLayoutGroup _RootLayoutGroup;


		/// <inheritdoc/>
		public override void CollectViews()
		{
			base.CollectViews();

			_RootLayoutGroup = root.GetComponent<HorizontalLayoutGroup>();
			_PanelRT = root.GetChild(0) as RectTransform;
			_PanelRT.GetComponentAtPath("TitleText", out titleText);
			_PanelRT.GetComponentAtPath("FoldOutButton", out foldoutButton);
			_PanelRT.GetComponentAtPath("DirectoryIconImage", out _DirectoryIconImage);
			_PanelRT.GetComponentAtPath("FileIconImage", out _FileIconImage);
			foldoutButton.transform.GetComponentAtPath("FoldOutArrowImage", out foldoutArrowImage);
		}

		public override void MarkForRebuild()
		{
			base.MarkForRebuild();
			LayoutRebuilder.MarkLayoutForRebuild(_PanelRT);
		}

		public void UpdateViews(FileSystemEntryModel model)
		{
			titleText.text = model.title;
			bool isDir = model.IsDirectory;
			foldoutButton.interactable = isDir;
			_DirectoryIconImage.gameObject.SetActive(isDir);
			_FileIconImage.gameObject.SetActive(!isDir);
			foldoutArrowImage.gameObject.SetActive(isDir);
			if (isDir)
				foldoutArrowImage.rectTransform.localRotation = Quaternion.Euler(0, 0, model.expanded ? -90 : 0);

			_RootLayoutGroup.padding.left = 25 * model.depth; // the higher the depth, the higher the padding
		}
	}
}
