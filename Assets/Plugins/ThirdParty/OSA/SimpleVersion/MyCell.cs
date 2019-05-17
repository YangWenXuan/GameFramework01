using Com.TheFallenGames.OSA.CustomAdapters.GridView;
using UnityEngine;
using UnityEngine.UI;

namespace OSA {
    public class MyCell : MonoBehaviour, ISimpleCellView {
        public Text titleText;
        public Image backgroundImage;


        public void UpdateCell(CellViewsHolder newOrRecycled, object data) {
            this.titleText.text = " Item " + newOrRecycled.ItemIndex;
            this.backgroundImage.color = (Color) data;
        }
    }
}