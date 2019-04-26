using UnityEngine;
using UnityEngine.UI;
using XUI;

public class ActivePanelDialog : UIBehaviour {

    public static void Show(int arg1,string arg2,GameObject arg3) {
        var stack = FindObjectOfType<UIStackTest>();
        stack.Overlay(new[] {stack.Black, stack.Dialog},arg1,arg2,arg3);
    }

	void Awake() {
		this.GetComponentInChildren<Text>().text = "id:" + Random.Range(0, 100);//通过给一个id，测试被覆盖的界面恢复时状态仍然正确
	}
    
    public void OnUIShowArgs(int arg1,string arg2,GameObject arg3) {
        Debug.LogWarning(" 活动对话框显示了 参数是 "+arg1+arg2+arg3);
    }

    public override void OnUIPause(bool cover) {
        base.OnUIPause(cover);
        if (cover) {
          Debug.LogWarning("活动界面被覆盖了");
        } else {
            Debug.LogWarning("活动界面被叠加了");
        }
    }
}