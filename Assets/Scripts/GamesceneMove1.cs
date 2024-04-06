using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GamesceneMove1 : MonoBehaviour
{
    public int mode=0; //mode 변수를 지정하여 mode에 따라 게임 규칙이 달라지도록 함

    public void GameSceneCtrl_1()
    {
        SceneManager.LoadScene("Game"); //Game 씬으로 이동
        mode=1; 
    }

     public void GameSceneCtrl_2()
    {
        SceneManager.LoadScene("Game"); //Game 씬으로 이동
        mode=2;
    }

     public void GameSceneCtrl_3()
    {
        SceneManager.LoadScene("Game"); //Game 씬으로 이동
        mode=3;
    }
    
}
