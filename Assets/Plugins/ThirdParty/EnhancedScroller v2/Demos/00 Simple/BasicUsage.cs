using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

public class BasicUsage : MonoBehaviour {
    private int[] _datas;

    public SimpleScroller Scroller;
    public SimpleScroller Scroller2;

    //public SimpleScroller Scroller3;

    private void Start() {
        this._datas = new int[100];
        for (int i = 0; i < 100; i++) {
            this._datas[i] = i;
        }
        this.Scroller.SetDatas(this._datas);
        this.Scroller2.SetDatas(this._datas);
        
        //this.Scroller3.SetDatas(this._datas);
    }

}