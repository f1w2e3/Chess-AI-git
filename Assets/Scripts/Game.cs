using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject chesspiece; // 체스 기물

    public GameObject[,] boardPositions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    private string currentPlayer = "white";
    private bool gameOver = false;

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
        yield return new WaitForSeconds(1); // 흑이 움직이기 전에 1초 지연

        // 미니맥스 알고리즘을 사용하여 최선의 수 계산
    (int value, Move move) result = Minimax(10, int.MinValue, int.MaxValue, "black");
    Move bestMove = result.move;

    Debug.Log($"Minimax evaluation: {result.value}, Move: ({bestMove.startX}, {bestMove.startY}) -> ({bestMove.endX}, {bestMove.endY})"); // 디버그 메시지 추가

        if (bestMove != null)
        {
            GameObject pieceToMove = boardPositions[bestMove.startX, bestMove.startY];
            pieceToMove.GetComponent<Chessman>().OnMouseUp();

            yield return new WaitForSeconds(1); // 기물이 이동하기 전에 1초 지연

            GameObject movePlateToClick = null;
            GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
            foreach (GameObject mp in movePlates)
            {
                MovePlate mpScript = mp.GetComponent<MovePlate>();
                if (mpScript.matrixX == bestMove.endX && mpScript.matrixY == bestMove.endY)
                {
                    movePlateToClick = mp;
                    break;
                }
            }

            if (movePlateToClick != null)
            {
                movePlateToClick.GetComponent<MovePlate>().OnMouseUp();
            }
            else
            {
                Debug.LogError("Error: Could not find move plate for calculated best move.");
            }
        }
        else
        {
            // 만약 움직일 수 있는 기물이 없다면 턴을 건너뛴다.
            Debug.Log("No movable pieces for black. Skipping turn.");
            NextTurn();
        }
    }

    private (int value, Move move) Minimax(int depth, int alpha, int beta, string maximizingPlayer)
    {
        if (depth == 0 || IsGameOver())
        {
            return (EvaluateBoard(), null);
        }

        Move bestMove = null;
        if (maximizingPlayer == "black")
        {
            int maxEval = int.MinValue;
            foreach (GameObject piece in playerBlack)
            {
                Chessman cm = piece.GetComponent<Chessman>();
                cm.DestroyMovePlates();
                cm.InitiateMovePlates();

                GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
                foreach (GameObject mp in movePlates)
                {
                    MovePlate mpScript = mp.GetComponent<MovePlate>();
                    int endX = mpScript.matrixX;
                    int endY = mpScript.matrixY;

                    // 임시 이동
                    int startX = cm.GetXBoard();
                    int startY = cm.GetYBoard();
                    GameObject capturedPiece = GetPosition(endX, endY);
                    SetPositionEmpty(startX, startY);
                    cm.SetXBoard(endX);
                    cm.SetYBoard(endY);
                    SetPosition(piece);

                    // 재귀 호출
                    int eval = Minimax(depth - 1, alpha, beta, "white").value;

                    // 되돌리기
                    SetPositionEmpty(endX, endY);
                    cm.SetXBoard(startX);
                    cm.SetYBoard(startY);
                    SetPosition(piece);
                    if (capturedPiece != null)
                    {
                        SetPosition(capturedPiece);
                    }

                    if (eval > maxEval)
                    {
                        maxEval = eval;
                        bestMove = new Move(startX, startY, endX, endY);
                    }

                    alpha = Mathf.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        cm.DestroyMovePlates();
                        return (maxEval, bestMove); // Beta cut-off
                    }
                }
                cm.DestroyMovePlates();
            }
            return (maxEval, bestMove);
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (GameObject piece in playerWhite)
            {
                // ... (위와 동일한 로직, 단지 최소화하는 플레이어이므로 minEval, beta 사용)
            }
            return (minEval, bestMove);
        }
    }

    // 체스 보드 상태를 평가하는 함수 (단순화된 예시)
    /*private int EvaluateBoard()
    {
        int score = 0;
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject piece = GetPosition(x, y);
                if (piece != null)
                {
                    Chessman cm = piece.GetComponent<Chessman>();
                    if (cm.player == "black")
                    {
                        score += GetPieceValue(cm.name);
                    }
                    else
                    {
                        score -= GetPieceValue(cm.name);
                    }
                }
            }
        }
        return score;
    }*/

    private int EvaluateBoard() {
    int score = 0;
    for (int y = 0; y < 8; y++) {
        for (int x = 0; x < 8; x++) {
            GameObject piece = GetPosition(x, y);
            if (piece != null) {
                Chessman cm = piece.GetComponent<Chessman>();
                int pieceValue = GetPieceValue(cm.name);

                // 기물의 색깔에 따라 점수를 더하거나 뺍니다.
                if (cm.player == "black") {
                    score += pieceValue;
                } else {
                    score -= pieceValue;
                }

                // 룩 활동성 보너스
                if (cm.name == "black_rook" || cm.name == "white_rook") {
                    // 룩이 있는 줄에 다른 기물이 없는 경우 보너스 점수를 부여합니다.
                    bool openFile = true;
                    for (int i = 0; i < 8; i++) {
                        if (i != y && GetPosition(x, i) != null) {
                            openFile = false;
                            break;
                        }
                    }
                    if (openFile) {
                        score += cm.player == "black" ? 2 : -2;
                    }
                }

                // // ... 다른 기물에 대한 위치 평가 로직 추가 ...
            }
        }
    }

    // 킹의 안전 평가 (예: 킹 주변에 기물이 적을수록 안전)
    // ...

    return score;
}

    // 체스 기물의 가치를 반환하는 함수 (단순화된 예시)
    private int GetPieceValue(string pieceName)
    {
        switch (pieceName)
        {
            case "black_pawn":
            case "white_pawn":
                return 1;
            case "black_knight":
            case "white_knight":
            case "black_bishop":
            case "white_bishop":
                return 3;
            case "black_rook":
            case "white_rook":
                return 5;
            case "black_queen":
            case "white_queen":
                return 9;
            default:
                return 0;
        }
    }

    // 이동을 나타내는 클래스
    private class Move
    {
        public int startX;
        public int startY;
        public int endX;
        public int endY;

        public Move(int startX, int startY, int endX, int endY)
        {
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
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
