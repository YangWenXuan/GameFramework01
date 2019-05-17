using System.Collections;
using UnityEngine.UI;
using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using frame8.Logic.Misc.Other.Extensions;
using UnityEngine;

namespace OSA {
    public class SimpleScroller : GridAdapter<GridParams, SimpleViewHolder> {
        public bool __AUTO_ITEM_SIZE = true;
        public IList Data { get; private set; }

        private void Awake() {
            var template = this._Params.grid.cellPrefab.gameObject;
            var templateRect = this._Params.grid.cellPrefab;
            template.SetActive(false);
            //自动添加layout组件，并且自动计算尺寸(包含缩放)
            var layoutElement = template.GetComponent<LayoutElement>();
            if (layoutElement == null) {
                layoutElement = template.AddComponent<LayoutElement>();
            }
            if (this.__AUTO_ITEM_SIZE) {
                layoutElement.preferredWidth = templateRect.sizeDelta.x * templateRect.localScale.x;
                layoutElement.preferredHeight = templateRect.sizeDelta.y * templateRect.localScale.y;
            }

            //自动添加viewport,content
            if (this._Params.Viewport == null) {
                var rect = this._Params.Viewport = new GameObject("ViewPort", typeof(RectTransform)).GetComponent<RectTransform>();
                rect.SetParent(this.transform,false);
                rect.MatchParentSize(false);
            }
            if (this._Params.Content == null) {
                var rect = this._Params.Content = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
                rect.SetParent(this._Params.Viewport,false);
            }
            Init();
        }

        protected override void Start() {
            //在awake里init过了，这里不要在base.Start了
        }

        public void ClearActive() {
            this.Data.Clear();
            this.ResetItems(0);
        }

        public void SetDatas(IList datas) {
            this.Data = datas;
            this.ResetItems(this.Data.Count);
        }

        protected override void UpdateCellViewsHolder(SimpleViewHolder holder) {
            holder.root.GetComponent<ISimpleCellView>().UpdateCell(holder, Data[holder.ItemIndex]);
        }
    }
}