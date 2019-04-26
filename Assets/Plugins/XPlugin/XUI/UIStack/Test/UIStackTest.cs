using UnityEngine;

namespace XUI {
    public class UIStackTest : UIStack {
        public GameObject TopPanel;
        public GameObject MainBoard;
        public GameObject Black;
        public GameObject Dialog;
        public GameObject OtherBoard;

        public UIGroup MainBoardGroup;

        private int _a = 10;

        void OnGUI() {
            this._a = this._a + 10;
            if (GUILayout.Button("主界面")) {
                this.MainBoardGroup = this.Cover(new[] {this.TopPanel, this.MainBoard},false, "this is args",12,45f,this);
            }
            if (GUILayout.Button("叠加对话框并传参")) {
                ActivePanelDialog.Show(123, "args", this.gameObject);
            }
            if (GUILayout.Button("覆盖新界面")) {
                this.Cover(new[] {this.TopPanel, this.OtherBoard}, false);
            }
            if (GUILayout.Button("回到上一层界面")) {
                this.Back();
            }
            if (GUILayout.Button("回到主界面")) {
                this.BackTo(this.MainBoardGroup);
            }
        }
    }
}