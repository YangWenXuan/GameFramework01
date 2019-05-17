using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Text;

public class Liortal53Extensions : MonoBehaviour
{

    public Text text;
    private string str01=null;


    void Start()
    {
        str01 =  StringLoggingExtensions.Colored("StirngMessage",LogColors.aqua);
        str01=StringLoggingExtensions.Sized(str01,60);
        Debug.Log(str01);
        text.text=str01;
    }
}
