using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject chesspiece; // 체스 기물
    public GameObject movePlate2; // 새로운 MovePlate 프리팹

    public GameObject[,] boardPositions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    private string currentPlayer = "white";
    private bool gameOver = false;
    private GameObject currentBlackPiece = null; // 현재 흑 기물을 저장하기 위한 변수

    void Start()
    {
        playerWhite = new GameObject[]
        {
            Create("white_rook",0,0), Create("white_knight",1,0), Create("white_bishop",2,0),
            Create("white_queen",3,0), Create("white_king",4,0), Create("white_bishop",5,0),
            Create("white_knight",6,0), Create("white_rook",7,0),
            Create("white_pawn",0,1), Create("white_pawn",1,1), Create("white_pawn",2,1),
            Create("white_pawn",3,1), Create("white_pawn",4,1), Create("white_pawn",5,1),
            Create("white_pawn",6,1), Create("white_pawn",7,1)
        };
        playerBlack = new GameObject[]
        {
            Create("black_rook",0,7), Create("black_knight",1,7), Create("black_bishop",2,7),
            Create("black_queen",3,7), Create("black_king",4,7), Create("black_bishop",5,7),
            Create("black_knight",6,7), Create("black_rook",7,7),
            Create("black_pawn",0,6), Create("black_pawn",1,6), Create("black_pawn",2,6),
            Create("black_pawn",3,6), Create("black_pawn",4,6), Create("black_pawn",5,6),
            Create("black_pawn",6,6), Create("black_pawn",7,6)
        };

        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
            StartCoroutine(MoveBlackPiece());
        }
        else
        {
            currentPlayer = "white";
        }
    }

    private IEnumerator MoveBlackPiece()
    {
        // 흑 기물을 랜덤으로 선택하고 이동 가능한 위치 표시
        currentBlackPiece = GetRandomBlackPieceWithMoves();

        // 1초 후에 movePlate2를 생성
        yield return new WaitForSeconds(1);
        SpawnMovePlate2(currentBlackPiece); 

        // 1초 후에 movePlate2를 제거
        yield return new WaitForSeconds(1);
        Destroy(GameObject.FindGameObjectWithTag("MovePlate2"));

        // 흑 기물이 이동 가능한 위치가 있으면 랜덤으로 이동
        if (GameObject.FindGameObjectsWithTag("MovePlate2").Length > 0)
        {
            FindObjectOfType<MovePlate>().ExecuteRandomMove();
        }
        else
        {
            // 만약 이동 가능한 위치가 없다면 다른 기물을 선택한다.
            Debug.Log("No valid moves for selected piece. Trying another piece.");
            StartCoroutine(MoveBlackPiece());
        }
    }

    // movePlate2를 생성하는 함수
    private void SpawnMovePlate2(GameObject blackPiece)
    {
        // 흑 기물이 이동 가능한 위치를 표시
        blackPiece.GetComponent<Chessman>().InitiateMovePlates();

        // "MovePlate2" 태그를 가진 모든 객체를 파괴
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate2");
        foreach (GameObject movePlate in movePlates)
        {
            Destroy(movePlate);
        }

        // 이동 가능한 위치에 "MovePlate2" 생성
        movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        foreach (GameObject movePlate in movePlates)
        {
            MovePlate movePlateScript = movePlate.GetComponent<MovePlate>();
            Vector3 position = movePlate.transform.position;
            GameObject newMovePlate = Instantiate(movePlate2, position, Quaternion.identity);
            newMovePlate.tag = "MovePlate2";
            MovePlate newMovePlateScript = newMovePlate.GetComponent<MovePlate>();
            newMovePlateScript.SetReference(blackPiece); // 기물 참조 설정
            newMovePlateScript.SetCoords(movePlateScript.matrixX, movePlateScript.matrixY); // 좌표 설정
            newMovePlateScript.attack = movePlateScript.attack; // 공격 여부 설정
        }

        // 이동 가능한 위치 표시를 제거
        blackPiece.GetComponent<Chessman>().DestroyMovePlates();
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        boardPositions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        boardPositions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return boardPositions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= boardPositions.GetLength(0) || y >= boardPositions.GetLength(1)) return false;
        return true;
    }

    public GameObject GetRandomBlackPieceWithMoves()
    {
        List<GameObject> movablePieces = new List<GameObject>();

        foreach (GameObject piece in playerBlack)
        {
            Chessman cm = piece.GetComponent<Chessman>();
            cm.DestroyMovePlates();
            cm.InitiateMovePlates();

            if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
            {
                movablePieces.Add(piece);
            }
            cm.DestroyMovePlates();
        }

        if (movablePieces.Count > 0)
        {
            int index = Random.Range(0, movablePieces.Count);
            return movablePieces[index];
        }

        return null;
    }
}