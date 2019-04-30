using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatchMessageObj : MonoBehaviour
{

    public Text text01;
    public Text text02;
    public Text text03;

    int index1=0;
    int index2=0;
    int index3=0;

    public void Method1forShow(string value)
    {
        index1++;
        text01.text=value+index1.ToString();
    }


    public void Method2forPause(string value)
    {
        index2++;
        text02.text=value+index2.ToString();;
    }


    public void Method3forRotate(string value)
    {
        index3++;
        text03.text=value+index3.ToString();
    }

}
