using UnityEngine;
using UnityEngine.UI;
public class Test : MonoBehaviour {
    //定义一个int 变量用来接收java传过来的参数
    int index = 0;
    string angle;
    public Text mtext;
    public Text testForReporter;
    //一个旋转物体
    public GameObject cube;
    public void OnClick1()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        /* jo.call | java对象的函数名字
         *   <>    | 函数返回值类型
         *  ("")   | 调用的方法名    */
        index = jo.Call<int>("makePauseUnity");
        angle = jo.Call<string>("makeRotateUnity");
        mtext.text = index.ToString();
        RotateCube(angle);

        Debug.Log("======TestForReporter01======");
        testForReporter.text="======TestForReporter01======";
    }
    public void OnClick2()
    {
        print("OnClick1被执行到了");
        /* com.android.unityToandroid : Android工程定义的包名
         * UnityPlayerActivity :java方法所在的类名 */
        AndroidJavaClass jc = new AndroidJavaClass("com.android.unityToandroid.UnityPlayerActivity");
        //m_instance : UnityPlayerActivity类中自己声明 
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("m_instance");
        //ShowMessage : java对应的方法 
        //Unity : 向这的代码传一个参数
        jo.Call("ShowMessage", "Unity");

        Debug.Log("======TestForReporter02======");
    }
    public void RotateCube(string angle)
    {
        cube.transform.Rotate(Vector3.up, float.Parse(angle));
    }
}


