using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
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
    
    //체스모드에 따른 기물 움직임을 위해 저장

    //폰의 최초이동시 두칸이나 캐슬링을 위해 이 기물이 이동했는지를 저장
    bool isMoved = false;

    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn, black_camel;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn, white_camel;

    public void Activate()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");

        SetCoords();

        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black"; break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black"; break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black"; break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;
            case "black_camel": this.GetComponent<SpriteRenderer>().sprite = black_camel; player = "black"; break;


            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
            case "white_camel": this.GetComponent<SpriteRenderer>().sprite = white_camel; player = "white"; break;
        }
    }

    public void SetCoords()
    {
        float x = xBoard;
        float y = yBoard;

        x *= 0.675f;
        y *= 0.68f;

        x += -2.35f;
        y += -2.8f;

        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    public bool GetIsMoved()
    {
        return isMoved;
    }
    public void SetIsMoved(bool tmp)
    {
        isMoved = tmp;
    }
    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }

    private void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player)
        {
            //이전 이동 경로 제거
            DestroyMovePlates();

            //이동 경로 생성
            InitiateMovePlates();
        }
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                if(ChessGameMode.GameMode == "default")
                {
                    LineMovePlate(1, 0);
                    LineMovePlate(0, 1);
                    LineMovePlate(-1, 0);
                    LineMovePlate(0, -1);
                }
                else if(ChessGameMode.GameMode == "mono")
                {
                    LineMoveMonoPlate(1,0);
                    LineMoveMonoPlate(0,1);
                    LineMoveMonoPlate(-1,0);
                    LineMoveMonoPlate(0,-1);
                }
                LineMovePlate(1, 1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
                break;
            case "black_camel":
            case "white_camel":
                LCamelMovePlate();
                break;
            case "black_bishop":
            case "white_bishop":
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;
            case "black_king":
            case "white_king": 
                if(ChessGameMode.GameMode == "default")
                    SurroundMovePlate();
                else if(ChessGameMode.GameMode == "mono")
                    SurroundMoveMonoPlate();
                break;
            case "black_rook":
            case "white_rook":
                if(ChessGameMode.GameMode == "default")
                {
                    LineMovePlate(1, 0);
                    LineMovePlate(0, 1);
                    LineMovePlate(-1, 0);
                    LineMovePlate(0, -1);
                }
                else if(ChessGameMode.GameMode == "mono")
                {
                    LineMoveMonoPlate(1,0);
                    LineMoveMonoPlate(0,1);
                    LineMoveMonoPlate(-1,0);
                    LineMoveMonoPlate(0,-1);
                }
                break;
            case "black_pawn":
                if(ChessGameMode.GameMode == "default")
                    PawnMovePlate(xBoard, yBoard - 1);
                if(isMoved == false)
                    PawnMovePlate(xBoard, yBoard - 2);
                break;
            case "white_pawn":
                if(ChessGameMode.GameMode == "default")
                    PawnMovePlate(xBoard, yBoard + 1);
                if(isMoved == false)
                    PawnMovePlate(xBoard, yBoard + 2);
                break;
        }
    }

    //직선 이동 경로 생성 (퀸, 룩, 비숍)
    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        //해당 위치가 비어있을 때까지 이동 경로 생성
        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        //이동 경로에 상대 기물이 있을 경우 공격 경로 생성
        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    //모노체스의 직선 이동 경로 생성(퀸,룩)
        public void LineMoveMonoPlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;
        
        bool samecolor = false;

        //해당 위치가 비어있을 때까지 이동 경로 생성
        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            if(samecolor == true)
            {
                MovePlateSpawn(x, y);
                samecolor = false;
            }
            else
                samecolor = true;
            x += xIncrement;
            y += yIncrement;
        }

        //이동 경로에 상대 기물이 있을 경우 공격 경로 생성
        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            if(samecolor == true)
                MovePlateAttackSpawn(x, y);
        }
    }

    //L자 이동 경로 생성 (나이트)
    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    //L자 이동경로(카멜)
        public void LCamelMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 3);
        PointMovePlate(xBoard - 1, yBoard + 3);
        PointMovePlate(xBoard + 3, yBoard + 1);
        PointMovePlate(xBoard + 3, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 3);
        PointMovePlate(xBoard - 1, yBoard - 3);
        PointMovePlate(xBoard - 3, yBoard + 1);
        PointMovePlate(xBoard - 3, yBoard - 1);
    }

    //주변 이동 경로 생성 (킹)
    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 0);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard + 0);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    //단섹 체스 주변 이동 경로 생성(킹)
    public void SurroundMoveMonoPlate()
    {
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    //특정 좌표에 이동 경로 생성
    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            //해당 위치가 비어있을 경우 이동 경로 생성
            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            //이동 경로에 상대 기물이 있을 경우 공격 경로 생성
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    //폰의 이동 경로 생성
    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            // 앞칸이 비어있을 경우 이동 경로 생성
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
            }
            // 대각선에 상대 기물이 있을 경우 공격 경로 생성
            if (sc.PositionOnBoard(x + 1, y) && sc.GetPosition(x + 1, y) != null && 
                sc.GetPosition(x + 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x + 1, y);
            }

            if (sc.PositionOnBoard(x - 1, y) && sc.GetPosition(x - 1, y) != null && 
                sc.GetPosition(x - 1, y).GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x - 1, y);
            }
        }
    }

    // 이동 경로 생성
    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.675f;
        y *= 0.68f;

        x += -2.35f;
        y += -2.82f;

        // 이동 경로 오브젝트 생성
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    // 공격 경로 생성
    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.675f;
        y *= 0.68f;

        x += -2.35f;
        y += -2.82f;

        // 공격 경로 오브젝트 생성
        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
}

