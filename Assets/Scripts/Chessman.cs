using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    //참조
    public GameObject controller;
    public GameObject movePlate; //기물이 움직일 수 있는 경로

// 위치
private int xBoard = -1; 
private int yBoard = -1;

//black player와 white player를 구분하기 위함. 체스에서 흰 말을 플레이하는 사람이 white player
private string player;


public Sprite black_queen, black_kinght, black_bishop, black_king, black_rook, black_pawn;
public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

}

