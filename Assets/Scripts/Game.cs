using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject chesspiece; //체스 기물


    // Start is called before the first frame update
    void Start()
    {
      Instantiate(chesspiece, new Vector3(0,0,-1), Quaternion.identity); 
        /*chesspiece에게 위치를 주는 함수.
        z값이 -1처럼 음수여야 체스 보드판 위로 기물이 올라온다*/
    }

    
}
