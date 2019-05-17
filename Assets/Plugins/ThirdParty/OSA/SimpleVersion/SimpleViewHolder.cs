using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using UnityEngine;
using UnityEngine.UI;

namespace OSA {
//不想被CellViewHolder中的结构规则约束，这里重写掉
    public class SimpleViewHolder : CellViewsHolder {
        protected override RectTransform GetViews() {
            return this.root;
        }

        public override void CollectViews() {
            this.views = GetViews();
            this.rootLayoutElement = this.root.GetComponent<LayoutElement>();
        }
    }
}