using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class AlphaBG_Board : MonoBehaviour
{
    
    void Start()
    {
        UIEventTriggerListener.Get(this.gameObject).onClick=OnAlphaBgClick;
    }

    public void OnAlphaBgClick(GameObject go)
    {
        if(go==this.gameObject)
        {
            ModMenu.Ins.Back();
        } 
    }
}
