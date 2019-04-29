using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnhancedUI.EnhancedScroller {
    /// <summary>
    /// 这是一个简化的复用列表，适用于只有一个固定的cell
    /// (由于可能拷贝数据，并且进行装箱，效率不如原始用法)
    /// </summary>
    public class SimpleScroller : EnhancedScroller, IEnhancedScrollerDelegate {
        public SimpleCellView Template;

        private float _size;

        public IList Datas = new List<object>();

        protected override void Awake() {
            base.Awake();
            this.Template.gameObject.SetActive(false);
            var rect = this.Template.GetComponent<RectTransform>().rect;
            this._size = this.scrollDirection == ScrollDirectionEnum.Vertical ? rect.height : rect.width;
            this.Delegate = this;
        } 
        
        public void SetDatas(IList datas, float scrollPositionFactor = 0) {
            this.Datas = datas;
            this.ReloadData(scrollPositionFactor);
        }


        public int GetNumberOfCells(EnhancedScroller scroller) {
            return this.Datas.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex) {
            return this._size;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex) {
            var cellView = scroller.GetCellView(this.Template) as SimpleCellView;
            cellView.gameObject.SetActive(true);
            cellView.SetData(this.Datas[dataIndex],dataIndex,cellIndex);
            return cellView;
        }
    }
}