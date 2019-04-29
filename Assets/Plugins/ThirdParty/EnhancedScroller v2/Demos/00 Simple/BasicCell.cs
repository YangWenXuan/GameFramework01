using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

public class BasicCell : SimpleCellView {
    public override void SetData(object data, int dataIndex, int cellIndex) {
        base.SetData(data, dataIndex, cellIndex);
        this.GetComponentInChildren<Text>().text = data.ToString();
    }
}