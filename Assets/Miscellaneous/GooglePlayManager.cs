using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
//using GooglePlayGames;
using UnityEngine.SocialPlatforms;
 
/// <summary>
/// Google play manager.
/// create by liufeng on 18-01-11
/// </summary>
public static class GooglePlayManager {
 
    public delegate void GPDelegate(bool success,string uname);
 
    static GPDelegate authenticatingCallback = null;
 
	/// <summary>
    /// 初始化SDK
    /// </summary>
	public static void Init () {
        //PlayGamesPlatform.Activate();
	}
 
    /// <summary>
    /// 是否登录
    /// </summary>
    /// <returns>The authenticated.</returns>
    public static bool Authenticated(){
        return Social.localUser.authenticated;
    }
	
    /// <summary>
    /// 登录
    /// </summary>
    /// <returns>The authenticating.</returns>
    /// <param name="cb">回调</param>
    public static void Authenticating(GPDelegate cb = null){
        authenticatingCallback = cb;
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("Authentication success : " + Social.localUser.userName);
            }
            else
            {
                Debug.Log("Authentication failed");
            }
            if (cb != null) cb(success, Social.localUser.userName);
        });
    }
 
    /// <summary>
    /// 注销
    /// </summary>
    public static void SignOut(){
        //((PlayGamesPlatform)Social.Active).SignOut();
    }
 
 
    /// <summary>
    /// 上传分数
    /// </summary>
    /// <param name="scores">分数.</param>
    /// <param name="lbid">排行榜id.</param>
    public static void PostScore(int scores,string lbid){
        if(!Authenticated()){
            Debug.Log("没有登录");
            return;
        }
 
        Social.ReportScore(scores,lbid,  (bool success) => {
            // handle success or failure
            Debug.Log("post score : " + success);
        });
 
    }
 
    /// <summary>
    /// 显示排行榜
    /// </summary>
    /// <param name="lbid">排行榜id，传空字符串则显示全部排行榜</param>
    public static void ShowLeaderboard(string lbid = ""){
        if (!Authenticated())
        {
            Debug.Log("没有登录");
            return;
        }
        if(lbid == ""){
            Social.ShowLeaderboardUI(); 
        }else{
            //PlayGamesPlatform.Instance.ShowLeaderboardUI(lbid);
        }
 
    }
}