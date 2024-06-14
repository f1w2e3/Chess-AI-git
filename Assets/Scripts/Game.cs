using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Game : MonoBehaviour
{
public GameObject chesspiece; // 체스 기물

// 체스에서 흑과 백 각각 갖고 있는 기물의 총 개수는 16개
// 기물들의 위치를 저장
// 체스 보드판은 8*8 사이즈의 64개 칸으로 이루어져있음. 이를 2차원 배열로 저장함
private GameObject[,] boardPositions = new GameObject[8, 8];
private GameObject[] playerBlack = new GameObject[16];
private GameObject[] playerWhite = new GameObject[16];

private string currentPlayer = "white";

private bool gameOver = false;

// Start is called before the first frame update
void Start()
{
    /*
    GameObject testObj = Instantiate(chesspiece, new Vector3(0,0,-1), Quaternion.identity); 
      //chesspiece에게 위치를 주는 함수.
      //z값이 -1처럼 음수여야 체스 보드판 위로 기물이 올라온다
    */

    // 기물들의 초기위치 설정 밑  생성
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

    //Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity); // 가운데에 실험용 기물을 두는 코드
    /*chesspiece에게 위치를 주는 함수.
    z값이 -1처럼 음수여야 체스 보드판 위로 기물이 올라온다*/
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
        // 흑의 턴이 시작되면 컴퓨터가 이동
        StartCoroutine(MoveBlackPiece()); 
    }
    else
    {
        currentPlayer = "white";
    }
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

// 좌표가 범위 내에 있는지 확인
public bool PositionOnBoard(int x, int y)
{
    if (x < 0 || y < 0 || x >= boardPositions.GetLength(0) || y >= boardPositions.GetLength(1)) return false;
    return true;
}

// 흑 플레이어의 랜덤 이동 처리
private IEnumerator MoveBlackPiece()
{
yield return new WaitForSeconds(1); // 흑이 움직이기 전에 1초 지연

// 보드판 위에 남아있는 흑 기물만 선택하기 위해 리스트를 초기화
    List<GameObject> movableBlackPieces = new List<GameObject>();

    // 흑 기물 배열에서 각 기물을 순회하며 보드판에 존재하는지 확인
    foreach (GameObject piece in playerBlack)
    {
        if (piece != null) // 기물이 파괴되지 않았는지 확인
        {
            movableBlackPieces.Add(piece); 
        }
    }

    // 이동 가능한 흑 기물이 남아있는지 확인
    if (movableBlackPieces.Count > 0)
    {
        GameObject piece = movableBlackPieces[Random.Range(0, movableBlackPieces.Count)]; // 랜덤으로 선택

        // 체크 상태인 경우 킹을 살리는 움직임만 가능하도록 제한
        if (IsCheck("black"))
        {
            // 흑 기물을 순회하며 킹을 살리는 움직임이 있는지 확인
            bool hasValidMove = false;
            piece.GetComponent<Chessman>().DestroyMovePlates();
            piece.GetComponent<Chessman>().InitiateMovePlates();
            if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
            {
                hasValidMove = true; // 킹을 살리는 움직임이 존재
            }
            piece.GetComponent<Chessman>().DestroyMovePlates();

            if (!hasValidMove)
            {
                // 킹을 살리는 움직임이 없으면 다시 기물을 선택
                Debug.Log("No valid moves to escape check. Trying another piece.");
                StartCoroutine(MoveBlackPiece());
                yield break; 
            }
        }

        piece.GetComponent<Chessman>().OnMouseUp(); // 흑 기물 선택 후 이동 경로 표시

        yield return new WaitForSeconds(1); // 기물이 이동하기 전에 1초 지연

        if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
        {
            FindObjectOfType<MovePlate>().ExecuteRandomMove(); // 랜덤으로 이동 경로 선택 후 이동
        }
        else
        {
            // 만약 이동 가능한 위치가 없다면 다른 기물을 선택한다.
            Debug.Log("No valid moves for selected piece. Trying another piece.");
            StartCoroutine(MoveBlackPiece());
        }

        // 컴퓨터 플레이어 턴 종료 시 MovePlate 오브젝트 삭제
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
        }
    }
    else
    {
        // 만약 움직일 수 있는 기물이 없다면 턴을 건너뛴다.
        Debug.Log("No movable pieces for black. Skipping turn.");
        NextTurn();
    }

    // 체크메이트 여부 확인
    if (IsCheckmate("white")) 
    {
        Winner("Black");
        yield break;
    }
}

// 흑 플레이어의 이동 가능한 기물 중 랜덤으로 하나를 선택
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

 public void Update()
{
    if (gameOver == true && Input.GetMouseButtonDown(0))
    {
        gameOver = false;

        //Using UnityEngine.SceneManagement is needed here
        SceneManager.LoadScene("Game"); //Restarts the game by loading the scene over again
    }
}

public void Winner(string playerWinner)
{
    gameOver = true;

    //Using UnityEngine.UI is needed here
    GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
    GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

    GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
}




  // 체크 상태 여부를 확인하는 함수
public bool IsCheck(string player)
{
    // 킹의 위치 찾기
    int kingX = -1;
    int kingY = -1;
    for (int x = 0; x < 8; x++)
    {
        for (int y = 0; y < 8; y++)
        {
            if (boardPositions[x, y] != null && boardPositions[x, y].GetComponent<Chessman>().player == player && boardPositions[x, y].name.Contains("king"))
            {
                kingX = x;
                kingY = y;
                break;
            }
        }
        if (kingX != -1)
        {
            break;
        }
    }

    // 킹이 공격받고 있음을 확인
    if (kingX != -1 && kingY != -1)
    {
        // 킹이 있는 위치를 기준으로 모든 기물의 이동 경로를 생성
        foreach (GameObject piece in playerWhite)
        {
            if (piece != null && piece.GetComponent<Chessman>().player != player) // 상대편 기물만 확인
            {
                piece.GetComponent<Chessman>().DestroyMovePlates();
                piece.GetComponent<Chessman>().InitiateMovePlates();
                if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
                {
                    foreach (GameObject movePlate in GameObject.FindGameObjectsWithTag("MovePlate"))
                    {
                        // 킹이 있는 위치에 공격 경로가 존재하는 경우 체크
                        if (movePlate.GetComponent<MovePlate>().matrixX == kingX && movePlate.GetComponent<MovePlate>().matrixY == kingY && movePlate.GetComponent<MovePlate>().attack)
                        {
                            return true;
                        }
                    }
                }
                piece.GetComponent<Chessman>().DestroyMovePlates();
            }
        }
        foreach (GameObject piece in playerBlack)
        {
            if (piece != null && piece.GetComponent<Chessman>().player != player) // 상대편 기물만 확인
            {
                piece.GetComponent<Chessman>().DestroyMovePlates();
                piece.GetComponent<Chessman>().InitiateMovePlates();
                if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
                {
                    foreach (GameObject movePlate in GameObject.FindGameObjectsWithTag("MovePlate"))
                    {
                        if (movePlate.GetComponent<MovePlate>().matrixX == kingX && movePlate.GetComponent<MovePlate>().matrixY == kingY && movePlate.GetComponent<MovePlate>().attack)
                        {
                            return true;
                        }
                    }
                }
                piece.GetComponent<Chessman>().DestroyMovePlates();
            }
        }
    }
    return false;
}

// 체크메이트 여부를 확인하는 함수
public bool IsCheckmate(string player)
{
    // 킹의 위치 찾기
    int kingX = -1;
    int kingY = -1;
    for (int x = 0; x < 8; x++)
    {
        for (int y = 0; y < 8; y++)
        {
            if (boardPositions[x, y] != null && boardPositions[x, y].GetComponent<Chessman>().player == player && boardPositions[x, y].name.Contains("king"))
            {
                kingX = x;
                kingY = y;
                break;
            }
        }
        if (kingX != -1)
        {
            break;
        }
    }

    // 킹이 공격받고 있음을 확인
    if (kingX != -1 && kingY != -1)
    {
        // 킹이 있는 위치를 기준으로 모든 기물의 이동 경로를 생성
        foreach (GameObject piece in playerWhite)
        {
            if (piece != null && piece.GetComponent<Chessman>().player == player)
            {
                piece.GetComponent<Chessman>().DestroyMovePlates();
                piece.GetComponent<Chessman>().InitiateMovePlates();
                if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
                {
                    foreach (GameObject movePlate in GameObject.FindGameObjectsWithTag("MovePlate"))
                    {
                        // 킹이 있는 위치에 공격 경로가 존재하는 경우 체크메이트
                        if (movePlate.GetComponent<MovePlate>().matrixX == kingX && movePlate.GetComponent<MovePlate>().matrixY == kingY && movePlate.GetComponent<MovePlate>().attack)
                        {
                            return true;
                        }
                    }
                }
                piece.GetComponent<Chessman>().DestroyMovePlates();
            }
        }
        foreach (GameObject piece in playerBlack)
        {
            if (piece != null && piece.GetComponent<Chessman>().player == player)
            {
                piece.GetComponent<Chessman>().DestroyMovePlates();
                piece.GetComponent<Chessman>().InitiateMovePlates();
                if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
                {
                    foreach (GameObject movePlate in GameObject.FindGameObjectsWithTag("MovePlate"))
                    {
                        if (movePlate.GetComponent<MovePlate>().matrixX == kingX && movePlate.GetComponent<MovePlate>().matrixY == kingY && movePlate.GetComponent<MovePlate>().attack)
                        {
                            return true;
                        }
                    }
                }
                piece.GetComponent<Chessman>().DestroyMovePlates();
            }
        }
    }
    return false;
}
}