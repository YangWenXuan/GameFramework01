using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.MultipleCellTypesDemo
{

    // public delegate void SelectedDelegate(EnhancedScrollerCellView cellView);

    

    /// <summary>
    /// This is the view for the rows
    /// </summary>
    public class CellViewRow : CellView
    {
        /// <summary>
        /// An internal reference to the row data. We could have just
        /// used the base CellView's class member _data, but that would
        /// require us to cast it each time a row data field is needed.
        /// By referencing the row data, we can save some time accessing
        /// the fields.
        /// </summary>
        private RowData _rowData;

        /// <summary>
        /// Links to the UI fields
        /// </summary>
        public Text userNameText;
        public Image userAvatarImage;
        public Text userHighScoreText;
        public Image panel;

        //Test Add 
        public Color selectedColor;
        public Color unSelectedColor;
        public SelectedDelegate selected;
        void OnDestroy()
        {
            if(_data!=null)
            {
                _data.selectedChanged-=SelectedChanged;
            }
        }




        /// <summary>
        /// Override of the base class's SetData function. This links the data
        /// and updates the UI
        /// </summary>
        /// <param name="data"></param>
        public override void SetData(Data data)
        {
            // call the base SetData to link to the underlying _data
            base.SetData(data);


            //Test Add
            if(_data!=null)
            {
                _data.selectedChanged-=SelectedChanged;
            }

            // cast the data as rowData and store the reference
            _rowData = data as RowData;

            // update the UI with the data fields
            userNameText.text = _rowData.userName;
            userAvatarImage.sprite = Resources.Load<Sprite>(_rowData.userAvatarSpritePath);
            // userAvatarImage.sprite = Resources.Load<Sprite>("01/avatar_male");
            userHighScoreText.text = string.Format("{0:n0}", _rowData.userHighScore);


            //Test Add
            _data.selectedChanged -= SelectedChanged;
            _data.selectedChanged += SelectedChanged;

            SelectedChanged(data.IsSelected);
        }



        //Test Add
        private void SelectedChanged(bool selected)
        {
            panel.color=(selected?selectedColor:unSelectedColor);
        }

        public void OnSelected()
        {
            if(selected!=null)
            {
                selected(this);
            }

            // Debug.Log("-============================测试Button是否管用？？");
        }
    }
}