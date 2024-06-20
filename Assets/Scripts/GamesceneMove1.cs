using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GamesceneMove1 : MonoBehaviour
{
    void Awake() 
    {
    Screen.SetResolution(800, 600, false);
    }
    
    public void GameSceneCtrl_1()
    {
        ChessGameMode.GameMode = "default";
        SceneManager.LoadScene("ai_select"); //Game 씬으로 이동
    }

     public void GameSceneCtrl_2()
    {
        ChessGameMode.GameMode = "mono";
        SceneManager.LoadScene("ai_select"); //Game 씬으로 이동
    }

     public void GameSceneCtrl_3()
    {
        SceneManager.LoadScene("ai_select"); //Game 씬으로 이동
    }
    
}

public static class ChessGameMode
{
        public static string GameMode = ""; //mode 변수를 지정하여 mode에 따라 게임 규칙이 달라지도록 함
        public static string GameMode2 = "";
}