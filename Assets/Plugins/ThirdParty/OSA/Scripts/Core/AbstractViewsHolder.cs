﻿using UnityEngine;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.Core
{
    /// <summary>
    /// Class representing the concept of a Views Holder, i.e. a class that references some views and the id of the data displayed by those views. 
    /// Usually, the root and its child views, once created, don't change, but <see cref="ItemIndex"/> does, after which the views will change their data.
    /// </summary>
    public class AbstractViewsHolder
    {
        /// <summary>The root of the view instance (which contains the actual views)</summary>
        public RectTransform root;

        /// <summary> The index of the data model from which this viewsholder's views take their display information </summary>
        public virtual int ItemIndex { get; set; }
		

        /// <summary> Calls <see cref="Init(GameObject, int, bool, bool)"/> </summary>
        public void Init(RectTransform rootPrefab, int itemIndex, bool activateRootGameObject = true, bool callCollectViews = true)
        { Init(rootPrefab.gameObject, itemIndex, activateRootGameObject, callCollectViews); }

        /// <summary>
		/// Instantiates <paramref name="rootPrefabGO"/>, assigns it to root and sets its itemIndex to <paramref name="itemIndex"/>. 
		/// Activates the new instance if <paramref name="activateRootGameObject"/> is true. Also calls CollectViews if <paramref name="callCollectViews"/> is true
		/// </summary>
        public virtual void Init(GameObject rootPrefabGO, int itemIndex, bool activateRootGameObject = true, bool callCollectViews = true)
        {
            root = (GameObject.Instantiate(rootPrefabGO) as GameObject).transform as RectTransform;
            if (activateRootGameObject)
                root.gameObject.SetActive(true);
            this.ItemIndex = itemIndex;

            if (callCollectViews)
                CollectViews();
        }

        /// <summary>If instead of calling <see cref="Init(GameObject, int, bool, bool)"/>, the initializaton is done manually, this should be called lastly as part of the initialization phase</summary>
        public virtual void CollectViews()
        { }

		/// <summary>
		/// Make sure to override this when you have children layouts (for example, a [Vertical/Horizontal/Grid]LayoutGroup) and call <see cref="LayoutRebuilder.MarkLayoutForRebuild(RectTransform)"/> for them. Base's implementation should still be called!
		/// </summary>
		public virtual void MarkForRebuild() { if (root) LayoutRebuilder.MarkLayoutForRebuild(root); }

		/// <summary>
		/// This is only called when an item is being shifted due to an <see cref="OSA{TParams, TItemViewsHolder}.InsertItems(int, int, bool, bool)"/> or 
		/// <see cref="OSA{TParams, TItemViewsHolder}.RemoveItems(int, int, bool, bool)"/> call, but since its data remains the same (the models are shifted 
		/// to make room for the others, but they don't change), there's no need to call <see cref="OSA{TParams, TItemViewsHolder}.UpdateViewsHolder(TItemViewsHolder)"/>
		/// <para>Don't forget to call the base implementation first if you override this method!</para>
		/// <para>Don't forget to call the base implementation first if you override this method!</para>
		/// </summary>
		/// <param name="shift"></param>
		/// <param name="modulo">
		/// This is the items count, but its purpose is to limit the new value of the itemIndex to be no greater than it. 
		/// If it's greater, it should be rotated. This occurs in some cases when looping is enabled.
		/// You don't need to care about this as here it's the only place it's used</param>
		public virtual void ShiftIndex(int shift, int modulo)
		{
			ItemIndex = ShiftIntWithOverflowCheck(ItemIndex, shift, modulo);
		}

		/// <summary>Internal utility for adding a <paramref name="shift"/> to <paramref name="value"/> and keeping it within range [0, <paramref name="modulo"/>), and also preventing integer overflow</summary>
		protected int ShiftIntWithOverflowCheck(int value, int shift, int modulo)
		{ return (int)(((long)value + shift + modulo) % modulo); }
    }
}
