using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    //참조
    public GameObject controller;
    public GameObject movePlate; //기물이 움직일 수 있는 경로

    // 위치 변수
    private int xBoard = -1;
    private int yBoard = -1;

    // black player와 white player를 구분하기 위함. 체스에서 흰 말을 플레이하는 사람이 white player
    private string player;


    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

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
            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
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
             controller.GetComponent<Game>().NextTurn(); // 턴 변경
        }
    }

    public void InitiateMovePlates()
    {
        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;
            case "black_knight":
            case "white_knight":
                LMovePlate();
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
                SurroundMovePlate();
                break;
            case "black_rook":
            case "white_rook":
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;
            case "black_pawn":
                PawnMovePlate(xBoard, yBoard - 1);
                break;
            case "white_pawn":
                PawnMovePlate(xBoard, yBoard + 1);
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


     public List<Game.Move> GetPossibleMoves(int x, int y)
    {
        List<Game.Move> moves = new List<Game.Move>();

        switch (this.name)
        {
            case "white_queen":
            case "black_queen":
                // 퀸의 이동 규칙 (직선, 대각선)
                moves.AddRange(LineMove(x, y, 1, 0)); // 오른쪽
                moves.AddRange(LineMove(x, y, 0, 1)); // 위쪽
                moves.AddRange(LineMove(x, y, -1, 0)); // 왼쪽
                moves.AddRange(LineMove(x, y, 0, -1)); // 아래쪽
                moves.AddRange(LineMove(x, y, 1, 1)); // 오른쪽 위
                moves.AddRange(LineMove(x, y, 1, -1)); // 오른쪽 아래
                moves.AddRange(LineMove(x, y, -1, 1)); // 왼쪽 위
                moves.AddRange(LineMove(x, y, -1, -1)); // 왼쪽 아래
                break;
            case "white_rook":
            case "black_rook":
                // 룩의 이동 규칙 (직선)
                moves.AddRange(LineMove(x, y, 1, 0)); // 오른쪽
                moves.AddRange(LineMove(x, y, 0, 1)); // 위쪽
                moves.AddRange(LineMove(x, y, -1, 0)); // 왼쪽
                moves.AddRange(LineMove(x, y, 0, -1)); // 아래쪽
                break;
            case "white_knight":
            case "black_knight":
                // 나이트의 이동 규칙 (L자 이동)
                moves.Add(new Game.Move(x, y, x + 1, y + 2,0)); // 오른쪽 위 2칸
                moves.Add(new Game.Move(x, y, x + 2, y + 1,0)); // 오른쪽 2칸 위
                moves.Add(new Game.Move(x, y, x + 2, y - 1,0)); // 오른쪽 2칸 아래
                moves.Add(new Game.Move(x, y, x + 1, y - 2,0)); // 오른쪽 아래 2칸
                moves.Add(new Game.Move(x, y, x - 1, y + 2,0)); // 왼쪽 위 2칸
                moves.Add(new Game.Move(x, y, x - 2, y + 1,0)); // 왼쪽 2칸 위
                moves.Add(new Game.Move(x, y, x - 2, y - 1,0)); // 왼쪽 2칸 아래
                moves.Add(new Game.Move(x, y, x - 1, y - 2,0)); // 왼쪽 아래 2칸
                break;
            case "white_bishop":
            case "black_bishop":
                // 비숍의 이동 규칙 (대각선)
                moves.AddRange(LineMove(x, y, 1, 1)); // 오른쪽 위
                moves.AddRange(LineMove(x, y, 1, -1)); // 오른쪽 아래
                moves.AddRange(LineMove(x, y, -1, 1)); // 왼쪽 위
                moves.AddRange(LineMove(x, y, -1, -1)); // 왼쪽 아래
                break;
            case "white_king":
            case "black_king":
                // 킹의 이동 규칙 (주변 1칸)
                moves.Add(new Game.Move(x, y, x + 1, y,0)); // 오른쪽
                moves.Add(new Game.Move(x, y, x, y + 1,0)); // 위쪽
                moves.Add(new Game.Move(x, y, x - 1, y,0)); // 왼쪽
                moves.Add(new Game.Move(x, y, x, y - 1,0)); // 아래쪽
                moves.Add(new Game.Move(x, y, x + 1, y + 1,0)); // 오른쪽 위
                moves.Add(new Game.Move(x, y, x + 1, y - 1,0)); // 오른쪽 아래
                moves.Add(new Game.Move(x, y, x - 1, y + 1,0)); // 왼쪽 위
                moves.Add(new Game.Move(x, y, x - 1, y - 1,0)); // 왼쪽 아래
                break;
            case "white_pawn":
                // 폰의 이동 규칙 (앞으로 한 칸 또는 두 칸, 대각선 공격)
                if (y < 7)
                {
                    // 앞으로 한 칸 이동 가능
                    if (controller.GetComponent<Game>().GetPosition(x, y + 1) == null)
                    {
                        moves.Add(new Game.Move(x, y, x, y + 1,0));

                        // 시작 위치에서 두 칸 이동 가능
                        if (y == 1 && controller.GetComponent<Game>().GetPosition(x, y + 2) == null)
                        {
                            moves.Add(new Game.Move(x, y, x, y + 2,0));
                        }
                    }

                    // 대각선 공격 가능
                    if (x < 7 && controller.GetComponent<Game>().GetPosition(x + 1, y + 1) != null &&
                        controller.GetComponent<Game>().GetPosition(x + 1, y + 1).GetComponent<Chessman>().player == "black")
                    {
                        moves.Add(new Game.Move(x, y, x + 1, y + 1,0));
                    }
                    if (x > 0 && controller.GetComponent<Game>().GetPosition(x - 1, y + 1) != null &&
                        controller.GetComponent<Game>().GetPosition(x - 1, y + 1).GetComponent<Chessman>().player == "black")
                    {
                        moves.Add(new Game.Move(x, y, x - 1, y + 1,0));
                    }
                }
                break;
            case "black_pawn":
                // 폰의 이동 규칙 (앞으로 한 칸 또는 두 칸, 대각선 공격)
                if (y > 0)
                {
                    // 앞으로 한 칸 이동 가능
                    if (controller.GetComponent<Game>().GetPosition(x, y - 1) == null)
                    {
                        moves.Add(new Game.Move(x, y, x, y - 1,0));

                        // 시작 위치에서 두 칸 이동 가능
                        if (y == 6 && controller.GetComponent<Game>().GetPosition(x, y - 2) == null)
                        {
                            moves.Add(new Game.Move(x, y, x, y - 2,0));
                        }
                    }

                    // 대각선 공격 가능
                    if (x < 7 && controller.GetComponent<Game>().GetPosition(x + 1, y - 1) != null &&
                        controller.GetComponent<Game>().GetPosition(x + 1, y - 1).GetComponent<Chessman>().player == "white")
                    {
                        moves.Add(new Game.Move(x, y, x + 1, y - 1,0));
                    }
                    if (x > 0 && controller.GetComponent<Game>().GetPosition(x - 1, y - 1) != null &&
                        controller.GetComponent<Game>().GetPosition(x - 1, y - 1).GetComponent<Chessman>().player == "white")
                    {
                        moves.Add(new Game.Move(x, y, x - 1, y - 1,0));
                    }
                }
                break;
        }

        return moves;
    }

     private List<Game.Move> LineMove(int x, int y, int xIncrement, int yIncrement)
    {
        List<Game.Move> moves = new List<Game.Move>();
        int newX = x + xIncrement;
        int newY = y + yIncrement;

        // 체스판 범위 내에 있는지 확인
        while (controller.GetComponent<Game>().PositionOnBoard(newX, newY))
        {
            // 해당 칸이 비어있으면 이동 가능
            if (controller.GetComponent<Game>().GetPosition(newX, newY) == null)
            {
                moves.Add(new Game.Move(x, y, newX, newY,0));
            }
            // 해당 칸에 상대 기물이 있으면 공격 가능
            else if (controller.GetComponent<Game>().GetPosition(newX, newY).GetComponent<Chessman>().player != player)
            {
                moves.Add(new Game.Move(x, y, newX, newY,0));
                break; // 상대 기물을 지나서 이동 불가능
            }
            else
            {
                break; // 같은 편 기물을 지나서 이동 불가능
            }

            newX += xIncrement;
            newY += yIncrement;
        }

        return moves;
    }

       public string GetPlayer() 
    {
        return player; 
    }

}