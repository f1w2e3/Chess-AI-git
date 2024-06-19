using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class main : MonoBehaviour
{


    public void M_1()
    {
        ChessGameMode.GameMode2 = "m1";
        SceneManager.LoadScene("Game"); //Game 씬으로 이동
    }

     public void M_2()
    {
        ChessGameMode.GameMode2 = "m2";
        SceneManager.LoadScene("Game"); //Game 씬으로 이동
    }

     public void M_3()
    {ChessGameMode.GameMode2 = "m3";
        SceneManager.LoadScene("Game"); //Game 씬으로 이동
    }
     public void M_4()
    {ChessGameMode.GameMode2 = "m4";
        SceneManager.LoadScene("Game"); //Game 씬으로 이동
    }
    
}

