// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{

    public static Leaderboard instance;
    //相关应排行榜id,可根据自己需求定义。
    public string endless_bestkill = "CgkIjerK29sZEAIQAQ";
    public string endless_hilevel = "CgkIjerK29sZEAIQBQ";
    public string warfog_bestkill = "CgkIjerK29sZEAIQAg";
    public string warfog_hilevel = "CgkIjerK29sZEAIQBg";
    public string warfog_survival = "CgkIjerK29sZEAIQBA";
    public string hiarea_bestkill = "CgkIjerK29sZEAIQAw";
    public string hiarea_hilevel = "CgkIjerK29sZEAIQBw";

    protected string leaderboardIdToUse;

    protected bool isLogged;

    void Awake()
    {
        //清空引用操作.
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // instance = this;
        // PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //     // requests an ID token be generated.  This OAuth token can be used to
        //     //  identify the player to other services such as Firebase.
        //     .RequestIdToken()
        //     .Build();
        // PlayGamesPlatform.InitializeInstance(config);
        // // 查看Google服务输出信息 recommended for debugging:
        // PlayGamesPlatform.DebugLogEnabled = true;
        // // Activate the Google Play Games platform
        // //激活谷歌服务 
        // PlayGamesPlatform.Activate();
        // //判断用户是否登录
        // SignIn();
        // Debug.LogError("google play servers sign in");
    }

    void SignIn()
    {
        // 认证用户，判定用户是否登录:
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                isLogged = true;
            }
            else
            {
                isLogged = false;
            }
        });
    }

    //取本地的HighScore与对应排行榜线上的分数作比较时，使用此方法，googlePlayLeaderboardID为排行榜ID
    public void reportScore(long HighScore, string googlePlayLeaderboardID)
    {
        if (!isLogged)
        {
            Debug.LogError("isLogged or not");
            return;
        }
        // PlayGamesPlatform.Instance.LoadScores(
        //     googlePlayLeaderboardID,
        //     LeaderboardStart.PlayerCentered,
        //     1,
        //     LeaderboardCollection.Public,
        //     LeaderboardTimeSpan.AllTime,
        //     (data) =>
        //     {
        //         if (data.Valid)
        //         {
        //             int score = (int)data.PlayerScore.value;
        //             Debug.LogError("score from inside leaderboard: " + data.PlayerScore.value);
        //             Debug.LogError("formated score from inside leaderboard: " + data.PlayerScore.formattedValue);
        //             if (score < HighScore)
        //             {
        //                 Social.ReportScore(HighScore, googlePlayLeaderboardID, (bool success) =>
        //                 {

        //                 });
        //             }
        //         }
        //         else
        //         {
        //             Debug.LogError("data invalid in leaderboard");
        //         }
        //     });
    }


    
    //在本地已经做了排序处理，只提交自己需要提交的相关分数时 使用此方法，leaderboardIdToUse为对应排行榜ID
    public void reportScore(long score)
    {      
        try
        {
            Social.ReportScore(score, leaderboardIdToUse, (bool success) =>
            {

            });
        }
        catch
        {

        }
    }


    //显示排行榜
    public void showLeaderboard()
    {

        //显示对应ID排行榜
        //PlayGamesPlatform.Instance.ShowLeaderboardUI(googlePlayLeaderboardID);
        //显示所有排行榜
        Social.ShowLeaderboardUI();
    }

    //解锁一个成就 对应地方引用该方法即可解锁该成就
    public static void UnlockAnAchievement(string _achievementID)
    {


        Social.ReportProgress(_achievementID, 100.0f, (bool success) =>
        {

        });
    }


    //显示成就页面
    public static void ShowAchievementUI()
    {
        Social.ShowAchievementsUI();
    }

}
