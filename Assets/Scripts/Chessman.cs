using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    //참조
    public GameObject controller;
    public GameObject movePlate; //기물이 움직일 수 있는 경로

// 위치 변수
private int xBoard = -1; 
private int yBoard = -1;

//black player와 white player를 구분하기 위함. 체스에서 흰 말을 플레이하는 사람이 white player
private string player;


public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;


//기물과 관련된 초기 설정
public void Active()
{
    controller = GameObject.FindGameObjectWithTag("GameController");

    SetCords();

    switch(this.name)
    {
        case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; break;
        case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; break;
        case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; break;
        case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; break;
        case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; break;
        case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; break;

        case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; break;
        case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; break;
        case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; break;
        case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; break;
        case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; break;
        case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; break;
    }
}

//기물의 위치를 조정해주는 함수
public void SetCords()
    {
        float x = xBoard;
        float y = yBoard;

        x *= 0.675f; x += -2.35f;
        y *= 0.68f; y += -2.8f;

        this.transform.position = new Vector3(x,y,-1.0f);
    }   


    public int GetXBoard() {return xBoard;} 
    public int GetYBoard() { return yBoard;}
    public void SetXBoard(int x){ xBoard = x;}
    public void SetYBoard(int y){ yBoard = y;}
}






