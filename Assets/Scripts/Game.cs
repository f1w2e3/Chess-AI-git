using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

   
  if(ChessGameMode.GameMode2=="m4")
  {
    StartCoroutine(MoveWhitePiece());
  }
  
}


private class Move
{
public Position From;
public Position To;
public float Score;

public Move(Position from, Position to, float score)
    {
        From = from;
        To = to;
        Score = score;
    }
}

// 위치 정보 클래스
private class Position
{
    public int X;
    public int Y;

    public Position(int x, int y)
    {
        X = x;
        Y = y;
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
        // 흑의 턴이 시작되면 컴퓨터가 이동
        StartCoroutine(MoveBlackPiece()); 

    }
    else
    {
        
        if(ChessGameMode.GameMode2=="m4")
        {
             currentPlayer = "white";
            StartCoroutine(MoveWhitePiece()); // 백도 랜덤으로 움직이도록 코루틴 실행
        }
        else
        {
            currentPlayer = "white";
        }
    }
}

private IEnumerator MoveWhitePiece()
    {
        yield return new WaitForSeconds(1); // 백이 움직이기 전에 1초 지연

        GameObject piece = GetRandomWhitePieceWithMoves();
        if (piece != null)
        {
            piece.GetComponent<Chessman>().OnMouseUp();

            yield return new WaitForSeconds(1); // 기물이 이동하기 전에 1초 지연

            if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
            {
                FindObjectOfType<MovePlate>().ExecuteRandomMove();
            }
            else
            {
                // 만약 이동 가능한 위치가 없다면 다른 기물을 선택한다.
                Debug.Log("No valid moves for selected piece. Trying another piece.");
                StartCoroutine(MoveWhitePiece());
            }
        }
        else
        {
            // 만약 움직일 수 있는 기물이 없다면 턴을 건너뛴다.
            Debug.Log("No movable pieces for white. Skipping turn.");
            NextTurn();
        }
    }

    public GameObject GetRandomWhitePieceWithMoves()
    {
        List<GameObject> movablePieces = new List<GameObject>();

        foreach (GameObject piece in playerWhite)
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

     if(ChessGameMode.GameMode2=="m2")
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
     else  if(ChessGameMode.GameMode2=="m4")
     {
 yield return new WaitForSeconds(1); // 흑이 움직이기 전에 1초 지연

        GameObject piece = GetRandomBlackPieceWithMoves();
        if (piece != null)
        {
            piece.GetComponent<Chessman>().OnMouseUp();

            yield return new WaitForSeconds(1); // 기물이 이동하기 전에 1초 지연

            if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
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
        else
        {
            // 만약 움직일 수 있는 기물이 없다면 턴을 건너뛴다.
            Debug.Log("No movable pieces for black. Skipping turn.");
            NextTurn();
        }
     }
     else if(ChessGameMode.GameMode2=="m3")
     {
        yield return new WaitForSeconds(1); // 흑색 플레이어 턴 시작 전 1초 지연

    // 가상 체스판 생성 및 미니맥스 알고리즘 실행
    ChessBoard virtualBoard = new ChessBoard(); // 가상 체스판 생성
    Move bestMove = virtualBoard.Minimax(3, true); // 미니맥스 알고리즘 실행 (깊이 3)
    Debug.Log($"최선의 수: {bestMove.From.X}, {bestMove.From.Y} -> {bestMove.To.X}, {bestMove.To.Y}");

//애플
//여기서 애플
    // 이동 가능한 흑색 기물을 찾습니다.
    GameObject piece = GetRandomBlackPieceWithMoves();
        if (piece != null)
        {
            piece.GetComponent<Chessman>().OnMouseUp();

            yield return new WaitForSeconds(1); // 기물이 이동하기 전에 1초 지연

            if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
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
        else
        {
            // 만약 움직일 수 있는 기물이 없다면 턴을 건너뛴다.
            Debug.Log("No movable pieces for black. Skipping turn.");
            NextTurn();
        }
     }
}

//미니맥스 병합
// 체스판에서 특정 위치의 기물을 가져오는 함수
private GameObject GetPieceAt(int x, int y)
{
if (x < 0 || x >= 8 || y < 0 || y >= 8)
{
return null;
}

return boardPositions[x, y];
}

// 체스판 출력 함수
private void PrintBoard()
{
    for (int y = 7; y >= 0; y--)
    {
        string row = "";
        for (int x = 0; x < 8; x++)
        {
            if (boardPositions[x, y] != null)
            {
                Chessman cm = boardPositions[x, y].GetComponent<Chessman>();
                switch (cm.name)
                {
                    case "black_king": row += "K "; break;
                    case "black_queen": row += "Q "; break;
                    case "black_rook": row += "R "; break;
                    case "black_bishop": row += "B "; break;
                    case "black_knight": row += "N "; break;
                    case "black_pawn": row += "P "; break;
                    case "white_king": row += "k "; break;
                    case "white_queen": row += "q "; break;
                    case "white_rook": row += "r "; break;
                    case "white_bishop": row += "b "; break;
                    case "white_knight": row += "n "; break;
                    case "white_pawn": row += "p "; break;
                }
            }
            else
            {
                row += "- ";
            }
        }
        Debug.Log(row);
    }
}

// 가상 체스판 클래스
private class ChessBoard
{
    private float totalScore = 0; // 총 점수
    // 가상 체스판 (각 칸은 기물 이름 문자열로 표현)
    private string[,] board = new string[8, 8];

    // 초기 상태로 가상 체스판 초기화
    public ChessBoard()
    {
        // 흑색 기물 초기화
        board[0, 0] = "R";
        board[1, 0] = "N";
        board[2, 0] = "B";
        board[3, 0] = "Q";
        board[4, 0] = "K";
        board[5, 0] = "B";
        board[6, 0] = "N";
        board[7, 0] = "R";
        for (int i = 0; i < 8; i++)
        {
            board[i, 1] = "P";
        }

        // 백색 기물 초기화
        board[0, 7] = "r";
        board[1, 7] = "n";
        board[2, 7] = "b";
        board[3, 7] = "q";
        board[4, 7] = "k";
        board[5, 7] = "b";
        board[6, 7] = "n";
        board[7, 7] = "r";
        for (int i = 0; i < 8; i++)
        {
            board[i, 6] = "p";
        }

        
    }

    // 체스판 출력 함수
    public void PrintBoard()
    {
        for (int y = 7; y >= 0; y--)
        {
            string row = "";
            for (int x = 0; x < 8; x++)
            {
                if (board[x, y] != null)
                {
                    row += board[x, y] + " ";
                }
                else
                {
                    row += "- ";
                }
            }
            Debug.Log(row);
        }
    }

    

    // 미니맥스 알고리즘 (탐색 과정 출력)
    public Move Minimax(int depth, bool isMaximizingPlayer)
    {
        Debug.Log($"Depth: {depth}, Maximizing: {isMaximizingPlayer}");
        PrintBoard();

        // 기저 사례: 깊이가 0이거나 게임 종료
        if (depth == 0 || IsGameOver())
        {
            Debug.Log($"Evaluating board: {EvaluateBoard()}");
            return new Move(null, null, EvaluateBoard());
        }

        // 최대화 플레이어 (흑)
            if (isMaximizingPlayer)
            {
                Move bestMove = new Move(new Position(-1, -1), new Position(-1, -1), float.MinValue);
                foreach (Move move in GetPossibleMoves(isMaximizingPlayer ? "black" : "white"))
                {
                    Debug.Log($"Trying move: {move.From.X}, {move.From.Y} -> {move.To.X}, {move.To.Y}");
                    ApplyMove(move);
                    float score = Minimax(depth - 1, false).Score;
                    UndoMove(move);

                    Debug.Log($"Current Score: {totalScore}"); // 현재 점수 출력

                    if (score > bestMove.Score)
                    {
                        bestMove = new Move(move.From, move.To, score);
                    }
                }
                Debug.Log($"Best move for black: {bestMove.From.X}, {bestMove.From.Y} -> {bestMove.To.X}, {bestMove.To.Y}, Score: {bestMove.Score}");
                return bestMove;
            }
            // 최소화 플레이어 (백)
            else
            {
                Move bestMove = new Move(new Position(-1, -1), new Position(-1, -1), float.MaxValue);
                foreach (Move move in GetPossibleMoves(isMaximizingPlayer ? "black" : "white"))
                {
                    Debug.Log($"Trying move: {move.From.X}, {move.From.Y} -> {move.To.X}, {move.To.Y}");
                    ApplyMove(move);
                    float score = Minimax(depth - 1, true).Score;
                    UndoMove(move);

                    Debug.Log($"Current Score: {totalScore}"); // 현재 점수 출력

                    if (score < bestMove.Score)
                    {
                        bestMove = new Move(move.From, move.To, score);
                    }
                }
                Debug.Log($"Best move for white: {bestMove.From.X}, {bestMove.From.Y} -> {bestMove.To.X}, {bestMove.To.Y}, Score: {bestMove.Score}");
                return bestMove;
        }
    }

    // 체스판 평가 함수 (임의로 구현)
   

    // 체스판 평가 함수 (임의로 구현)
    private float EvaluateBoard()
    {
        // 기물의 가치에 따라 평가
        float score = 0;
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (board[x, y] != null)
                {
                    if (char.IsUpper(board[x, y][0])) // 흑색 기물
                    {
                        score += GetPieceValue(board[x, y]);
                    }
                    else // 백색 기물
                    {
                        score -= GetPieceValue(board[x, y]);
                    }
                }
            }
        }
        return score;
    }

    // 기물 가치 반환
    // 기물 가치 반환
    private float GetPieceValue(string pieceName)
    {
        switch (pieceName)
        {
            case "Q":
            case "q":
                return 9;
            case "R":
            case "r":
                return 5;
            case "N":
            case "n":
            case "B":
            case "b":
                return 3;
            case "P":
            case "p":
                return 1;
            case "K":
            case "k":
                return 1000; // 왕은 특별한 가치 부여
            default:
                return 0;
        }
    }

    // 가능한 이동 목록 반환 (가상 체스판 기반)
         private List<Move> GetPossibleMoves(string player)
    {
        List<Move> possibleMoves = new List<Move>();

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (board[x, y] != null && (player == "black" && char.IsUpper(board[x, y][0]) || player == "white" && char.IsLower(board[x, y][0])))
                {
                    switch (board[x, y])
                    {
                        case "P": // 흑색 폰
                            // 한 칸 앞으로 이동
                            if (y < 7 && board[x, y + 1] == null)
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x, y + 1), 0));
                            }
                            // 두 칸 앞으로 이동 (시작 위치에서만 가능)
                            if (y == 1 && board[x, y + 1] == null && board[x, y + 2] == null)
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x, y + 2), 0));
                            }
                            // 대각선으로 포획
                            if (x < 7 && y < 7 && board[x + 1, y + 1] != null && char.IsLower(board[x + 1, y + 1][0]))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y + 1), 0));
                            }
                            if (x > 0 && y < 7 && board[x - 1, y + 1] != null && char.IsLower(board[x - 1, y + 1][0]))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y + 1), 0));
                            }
                            break;
                        case "p": // 백색 폰
                            // 한 칸 앞으로 이동
                            if (y > 0 && board[x, y - 1] == null)
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x, y - 1), 0));
                            }
                            // 두 칸 앞으로 이동 (시작 위치에서만 가능)
                            if (y == 6 && board[x, y - 1] == null && board[x, y - 2] == null)
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x, y - 2), 0));
                            }
                            // 대각선으로 포획
                            if (x < 7 && y > 0 && board[x + 1, y - 1] != null && char.IsUpper(board[x + 1, y - 1][0]))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y - 1), 0));
                            }
                            if (x > 0 && y > 0 && board[x - 1, y - 1] != null && char.IsUpper(board[x - 1, y - 1][0]))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y - 1), 0));
                            }
                            break;
                        case "R": // 흑색 룩
                            // 수직 이동
                            for (int i = y + 1; i < 8; i++)
                            {
                                if (board[x, i] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                }
                                else if (char.IsLower(board[x, i][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = y - 1; i >= 0; i--)
                            {
                                if (board[x, i] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                }
                                else if (char.IsLower(board[x, i][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            // 수평 이동
                            for (int i = x + 1; i < 8; i++)
                            {
                                if (board[i, y] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                }
                                else if (char.IsLower(board[i, y][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = x - 1; i >= 0; i--)
                            {
                                if (board[i, y] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                }
                                else if (char.IsLower(board[i, y][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case "r": // 백색 룩
                            // 수직 이동
                            for (int i = y + 1; i < 8; i++)
                            {
                                if (board[x, i] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                }
                                else if (char.IsUpper(board[x, i][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = y - 1; i >= 0; i--)
                            {
                                if (board[x, i] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                }
                                else if (char.IsUpper(board[x, i][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            // 수평 이동
                            for (int i = x + 1; i < 8; i++)
                            {
                                if (board[i, y] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                }
                                else if (char.IsUpper(board[i, y][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = x - 1; i >= 0; i--)
                            {
                                if (board[i, y] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                }
                                else if (char.IsUpper(board[i, y][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case "N": // 흑색 나이트
                            // L 자 이동
                            if (x + 2 < 8 && y + 1 < 8)
                            {
                                if (board[x + 2, y + 1] == null || char.IsLower(board[x + 2, y + 1][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x + 2, y + 1), 0));
                                }
                            }
                            if (x + 2 < 8 && y - 1 >= 0)
                            {
                                if (board[x + 2, y - 1] == null || char.IsLower(board[x + 2, y - 1][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x + 2, y - 1), 0));
                                }
                            }
                            if (x - 2 >= 0 && y + 1 < 8)
                            {
                                if (board[x - 2, y + 1] == null || char.IsLower(board[x - 2, y + 1][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x - 2, y + 1), 0));
                                }
                            }
                            if (x - 2 >= 0 && y - 1 >= 0)
                            {
                                if (board[x - 2, y - 1] == null || char.IsLower(board[x - 2, y - 1][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x - 2, y - 1), 0));
                                }
                            }
                            if (x + 1 < 8 && y + 2 < 8)
                            {
                                if (board[x + 1, y + 2] == null || char.IsLower(board[x + 1, y + 2][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y + 2), 0));
                                }
                            }
                            if (x + 1 < 8 && y - 2 >= 0)
                            {
                                if (board[x + 1, y - 2] == null || char.IsLower(board[x + 1, y - 2][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y - 2), 0));
                                }
                            }
                            if (x - 1 >= 0 && y + 2 < 8)
                            {
                                if (board[x - 1, y + 2] == null || char.IsLower(board[x - 1, y + 2][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y + 2), 0));
                                }
                            }
                            if (x - 1 >= 0 && y - 2 >= 0)
                            {
                                if (board[x - 1, y - 2] == null || char.IsLower(board[x - 1, y - 2][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y - 2), 0));
                                }
                            }
                            break;
                        case "n": // 백색 나이트
                            // L 자 이동
                            if (x + 2 < 8 && y + 1 < 8)
                            {
                                if (board[x + 2, y + 1] == null || char.IsUpper(board[x + 2, y + 1][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x + 2, y + 1), 0));
                                }
                            }
                            if (x + 2 < 8 && y - 1 >= 0)
                            {
                                if (board[x + 2, y - 1] == null || char.IsUpper(board[x + 2, y - 1][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x + 2, y - 1), 0));
                                }
                            }
                            if (x - 2 >= 0 && y + 1 < 8)
                            {
                                if (board[x - 2, y + 1] == null || char.IsUpper(board[x - 2, y + 1][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x - 2, y + 1), 0));
                                }
                            }
                            if (x - 2 >= 0 && y - 1 >= 0)
                            {
                                if (board[x - 2, y - 1] == null || char.IsUpper(board[x - 2, y - 1][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x - 2, y - 1), 0));
                                }
                            }
                            if (x + 1 < 8 && y + 2 < 8)
                            {
                                if (board[x + 1, y + 2] == null || char.IsUpper(board[x + 1, y + 2][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y + 2), 0));
                                }
                            }
                            if (x + 1 < 8 && y - 2 >= 0)
                            {
                                if (board[x + 1, y - 2] == null || char.IsUpper(board[x + 1, y - 2][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y - 2), 0));
                                }
                            }
                            if (x - 1 >= 0 && y + 2 < 8)
                            {
                                if (board[x - 1, y + 2] == null || char.IsUpper(board[x - 1, y + 2][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y + 2), 0));
                                }
                            }
                            if (x - 1 >= 0 && y - 2 >= 0)
                            {
                                if (board[x - 1, y - 2] == null || char.IsUpper(board[x - 1, y - 2][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y - 2), 0));
                                }
                            }
                            break;
                        case "B": // 흑색 비숍
                            // 대각선 이동
                            for (int i = 1; i < 8; i++)
                            {
                                if (x + i < 8 && y + i < 8)
                                {
                                    if (board[x + i, y + i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y + i), 0));
                                    }
                                    else if (char.IsLower(board[x + i, y + i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y + i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x - i >= 0 && y + i < 8)
                                {
                                    if (board[x - i, y + i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y + i), 0));
                                    }
                                    else if (char.IsLower(board[x - i, y + i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y + i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x + i < 8 && y - i >= 0)
                                {
                                    if (board[x + i, y - i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y - i), 0));
                                    }
                                    else if (char.IsLower(board[x + i, y - i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y - i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x - i >= 0 && y - i >= 0)
                                {
                                    if (board[x - i, y - i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y - i), 0));
                                    }
                                    else if (char.IsLower(board[x - i, y - i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y - i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            break;
                        case "b": // 백색 비숍
                            // 대각선 이동
                            for (int i = 1; i < 8; i++)
                            {
                                if (x + i < 8 && y + i < 8)
                                {
                                    if (board[x + i, y + i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y + i), 0));
                                    }
                                    else if (char.IsUpper(board[x + i, y + i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y + i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x - i >= 0 && y + i < 8)
                                {
                                    if (board[x - i, y + i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y + i), 0));
                                    }
                                    else if (char.IsUpper(board[x - i, y + i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y + i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x + i < 8 && y - i >= 0)
                                {
                                    if (board[x + i, y - i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y - i), 0));
                                    }
                                    else if (char.IsUpper(board[x + i, y - i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y - i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x - i >= 0 && y - i >= 0)
                                {
                                    if (board[x - i, y - i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y - i), 0));
                                    }
                                    else if (char.IsUpper(board[x - i, y - i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y - i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            break;
                        case "Q": // 흑색 퀸
                            // 룩 이동 (수직, 수평)
                            for (int i = y + 1; i < 8; i++)
                            {
                                if (board[x, i] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                }
                                else if (char.IsLower(board[x, i][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = y - 1; i >= 0; i--)
                            {
                                if (board[x, i] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                }
                                else if (char.IsLower(board[x, i][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = x + 1; i < 8; i++)
                            {
                                if (board[i, y] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                }
                                else if (char.IsLower(board[i, y][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = x - 1; i >= 0; i--)
                            {
                                if (board[i, y] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                }
                                else if (char.IsLower(board[i, y][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            // 비숍 이동 (대각선)
                            for (int i = 1; i < 8; i++)
                            {
                                if (x + i < 8 && y + i < 8)
                                {
                                    if (board[x + i, y + i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y + i), 0));
                                    }
                                    else if (char.IsLower(board[x + i, y + i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y + i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x - i >= 0 && y + i < 8)
                                {
                                    if (board[x - i, y + i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y + i), 0));
                                    }
                                    else if (char.IsLower(board[x - i, y + i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y + i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x + i < 8 && y - i >= 0)
                                {
                                    if (board[x + i, y - i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y - i), 0));
                                    }
                                    else if (char.IsLower(board[x + i, y - i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y - i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x - i >= 0 && y - i >= 0)
                                {
                                    if (board[x - i, y - i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y - i), 0));
                                    }
                                    else if (char.IsLower(board[x - i, y - i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y - i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            break;
                        case "q": // 백색 퀸
                            // 룩 이동 (수직, 수평)
                            for (int i = y + 1; i < 8; i++)
                            {
                                if (board[x, i] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                }
                                else if (char.IsUpper(board[x, i][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = y - 1; i >= 0; i--)
                            {
                                if (board[x, i] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                }
                                else if (char.IsUpper(board[x, i][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(x, i), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = x + 1; i < 8; i++)
                            {
                                if (board[i, y] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                }
                                else if (char.IsUpper(board[i, y][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            for (int i = x - 1; i >= 0; i--)
                            {
                                if (board[i, y] == null)
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                }
                                else if (char.IsUpper(board[i, y][0]))
                                {
                                    possibleMoves.Add(new Move(new Position(x, y), new Position(i, y), 0));
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            // 비숍 이동 (대각선)
                            for (int i = 1; i < 8; i++)
                            {
                                if (x + i < 8 && y + i < 8)
                                {
                                    if (board[x + i, y + i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y + i), 0));
                                    }
                                    else if (char.IsUpper(board[x + i, y + i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y + i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x - i >= 0 && y + i < 8)
                                {
                                    if (board[x - i, y + i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y + i), 0));
                                    }
                                    else if (char.IsUpper(board[x - i, y + i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y + i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x + i < 8 && y - i >= 0)
                                {
                                    if (board[x + i, y - i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y - i), 0));
                                      }
                                    else if (char.IsUpper(board[x + i, y - i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x + i, y - i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            for (int i = 1; i < 8; i++)
                            {
                                if (x - i >= 0 && y - i >= 0)
                                {
                                    if (board[x - i, y - i] == null)
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y - i), 0));
                                    }
                                    else if (char.IsUpper(board[x - i, y - i][0]))
                                    {
                                        possibleMoves.Add(new Move(new Position(x, y), new Position(x - i, y - i), 0));
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            break;
                        case "K": // 흑색 킹
                            // 상하좌우 이동
                            if (x + 1 < 8 && (board[x + 1, y] == null || char.IsLower(board[x + 1, y][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y), 0));
                            }
                            if (x - 1 >= 0 && (board[x - 1, y] == null || char.IsLower(board[x - 1, y][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y), 0));
                            }
                            if (y + 1 < 8 && (board[x, y + 1] == null || char.IsLower(board[x, y + 1][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x, y + 1), 0));
                            }
                            // y - 1이 배열 범위 내에 있는지 확인
                            if (y - 1 >= 0 && (board[x, y - 1] == null || char.IsLower(board[x, y - 1][0]))) 
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x, y - 1), 0));
                            }
                            // 대각선 이동
                            if (x + 1 < 8 && y + 1 < 8 && (board[x + 1, y + 1] == null || char.IsLower(board[x + 1, y + 1][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y + 1), 0));
                            }
                            if (x - 1 >= 0 && y + 1 < 8 && (board[x - 1, y + 1] == null || char.IsLower(board[x - 1, y + 1][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y + 1), 0));
                            }
                            // y - 1이 배열 범위 내에 있는지 확인
                            if (x + 1 < 8 && y - 1 >= 0 && (board[x + 1, y - 1] == null || char.IsLower(board[x + 1, y - 1][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y - 1), 0));
                            }
                            if (x - 1 >= 0 && y - 1 >= 0 && (board[x - 1, y - 1] == null || char.IsLower(board[x - 1, y - 1][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y - 1), 0));
                            }
                            break;
                        case "k": // 백색 킹
                            // 상하좌우 이동
                            if (x + 1 < 8 && (board[x + 1, y] == null || char.IsUpper(board[x + 1, y][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y), 0));
                            }
                            if (x - 1 >= 0 && (board[x - 1, y] == null || char.IsUpper(board[x - 1, y][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y), 0));
                            }
                            // y + 1이 배열 범위 내에 있는지 확인
                            if (y + 1 < 8 && (board[x, y + 1] == null || char.IsUpper(board[x, y + 1][0]))) 
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x, y + 1), 0));
                            }
                            if (y - 1 >= 0 && (board[x, y - 1] == null || char.IsUpper(board[x, y - 1][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x, y - 1), 0));
                            }
                            // 대각선 이동
                            if (x + 1 < 8 && y + 1 < 8 && (board[x + 1, y + 1] == null || char.IsUpper(board[x + 1, y + 1][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y + 1), 0));
                            }
                            if (x - 1 >= 0 && y + 1 < 8 && (board[x - 1, y + 1] == null || char.IsUpper(board[x - 1, y + 1][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y + 1), 0));
                            }
                            // y - 1이 배열 범위 내에 있는지 확인
                            if (x + 1 < 8 && y - 1 >= 0 && (board[x + 1, y - 1] == null || char.IsUpper(board[x + 1, y - 1][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x + 1, y - 1), 0));
                            }
                            if (x - 1 >= 0 && y - 1 >= 0 && (board[x - 1, y - 1] == null || char.IsUpper(board[x - 1, y - 1][0])))
                            {
                                possibleMoves.Add(new Move(new Position(x, y), new Position(x - 1, y - 1), 0));
                            }
                            break;
                    }
                }
            }
        }

        return possibleMoves;
    }

    // 이동 적용 (가상 체스판)
    private void ApplyMove(Move move)
{
    if (move.From != null && move.To != null)
    {
        // 이동 대상 위치에 기물이 있다면 제거
        if (board[move.To.X, move.To.Y] != null)
        {
            // 상대 기물 파괴 시 점수 추가 또는 감소
            if (char.IsUpper(board[move.To.X, move.To.Y][0])) // 흑색 기물 파괴
            {
                totalScore += GetPieceValue(board[move.To.X, move.To.Y]); 
            }
            else // 백색 기물 파괴
            {
                totalScore -= GetPieceValue(board[move.To.X, move.To.Y]); 
            }
        }

        board[move.To.X, move.To.Y] = board[move.From.X, move.From.Y];
        board[move.From.X, move.From.Y] = null;
    }
}

    // 이동 취소 (가상 체스판)
    private void UndoMove(Move move)
    {
        if (move.From != null && move.To != null)
        {
            board[move.From.X, move.From.Y] = board[move.To.X, move.To.Y];
            board[move.To.X, move.To.Y] = null;
        }
    }

    private bool IsGameOver()
    {
        // 게임 종료 조건 추가 (예: 체크메이트, 스테일메이트 등)
        return false;
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