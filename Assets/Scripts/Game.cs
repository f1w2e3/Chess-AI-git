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
    public bool gameOver = false;

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
        (int value, Move move) result = Minimax(2, "black"); // 탐색 깊이를 2로 조정
        Move bestMove = result.move;

        Debug.Log($"Minimax evaluation: {result.value}, Move: ({bestMove.startX}, {bestMove.startY}) -> ({bestMove.endX}, {bestMove.endY})");

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

    private (int value, Move move) Minimax(int depth, string maximizingPlayer)
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
                if (piece == null) continue; 

                Chessman cm = piece.GetComponent<Chessman>();
                cm.DestroyMovePlates();
                cm.InitiateMovePlates();

                GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
                foreach (GameObject mp in movePlates)
                {
                    MovePlate mpScript = mp.GetComponent<MovePlate>();
                    int endX = mpScript.matrixX;
                    int endY = mpScript.matrixY;

                    int startX = cm.GetXBoard();
                    int startY = cm.GetYBoard();
                    GameObject capturedPiece = GetPosition(endX, endY);
                    SetPositionEmpty(startX, startY);
                    cm.SetXBoard(endX);
                    cm.SetYBoard(endY);
                    SetPosition(piece);

                    int eval = Minimax(depth - 1, "white").value;

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
                }
                cm.DestroyMovePlates();
            }
            return (maxEval, bestMove);
        }
        else // minimizingPlayer "white"
        {
            int minEval = int.MaxValue;
            foreach (GameObject piece in playerWhite)
            {
                if (piece == null) continue;

                Chessman cm = piece.GetComponent<Chessman>();
                cm.DestroyMovePlates();
                cm.InitiateMovePlates();

                GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
                foreach (GameObject mp in movePlates)
                {
                    MovePlate mpScript = mp.GetComponent<MovePlate>();
                    int endX = mpScript.matrixX;
                    int endY = mpScript.matrixY;

                    int startX = cm.GetXBoard();
                    int startY = cm.GetYBoard();
                    GameObject capturedPiece = GetPosition(endX, endY);
                    SetPositionEmpty(startX, startY);
                    cm.SetXBoard(endX);
                    cm.SetYBoard(endY);
                    SetPosition(piece);

                    int eval = Minimax(depth - 1, "black").value;

                    SetPositionEmpty(endX, endY);
                    cm.SetXBoard(startX);
                    cm.SetYBoard(startY);
                    SetPosition(piece);
                    if (capturedPiece != null)
                    {
                        SetPosition(capturedPiece);
                    }

                    if (eval < minEval)
                    {
                        minEval = eval;
                        bestMove = new Move(startX, startY, endX, endY);
                    }
                }
                cm.DestroyMovePlates();
            }
            return (minEval, bestMove);
        }
    }


    // 체스 보드 상태를 평가하는 함수
    private int EvaluateBoard()
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
                    int pieceValue = GetPieceValue(cm.name);

                    // 기물의 색깔에 따라 점수를 더하거나 뺍니다.
                    score += cm.player == "black" ? pieceValue : -pieceValue;

                    // 기물 종류에 따라 위치 및 활동성 점수 추가
                    switch (cm.name)
                    {
                        case "black_pawn":
                        case "white_pawn":
                            score += cm.player == "black" ? PawnPositionBonus(y) : -PawnPositionBonus(7 - y);
                            break;
                        case "black_knight":
                        case "white_knight":
                            score += cm.player == "black" ? KnightPositionBonus(x, y) : -KnightPositionBonus(7 - x, 7 - y);
                            break;
                        case "black_bishop":
                        case "white_bishop":
                            score += cm.player == "black" ? BishopPositionBonus(x, y) : -BishopPositionBonus(7 - x, 7 - y);
                            break;
                        case "black_rook":
                        case "white_rook":
                            score += cm.player == "black" ? RookPositionBonus(x, y) : -RookPositionBonus(7 - x, 7 - y);
                            break;
                        case "black_queen":
                        case "white_queen":
                            score += cm.player == "black" ? QueenPositionBonus(x, y) : -QueenPositionBonus(7 - x, 7 - y);
                            break;
                        case "black_king":
                        case "white_king":
                            score += cm.player == "black" ? KingPositionBonus(x, y, IsEndGame()) : -KingPositionBonus(7 - x, 7 - y, IsEndGame());
                            break;
                    }
                }
            }
        }

        return score;
    }

    // 폰 위치 보너스 (진출할수록 높은 점수)
    private int PawnPositionBonus(int y)
    {
        return y;
    }

    // 나이트 위치 보너스 (중앙 위치일수록 높은 점수)
    private int KnightPositionBonus(int x, int y)
    {
        // TODO: 나이트 전초기지 보너스
        return -Mathf.Abs(x - 3) - Mathf.Abs(y - 3);
    }

    // 비숍 위치 보너스 (대각선으로 움직일 수 있는 칸 수에 따라 점수 부여)
    private int BishopPositionBonus(int x, int y)
    {
        int bonus = 0;
        bonus += CountFreeDiagonalSquares(x, y, 1, 1);
        bonus += CountFreeDiagonalSquares(x, y, 1, -1);
        bonus += CountFreeDiagonalSquares(x, y, -1, 1);
        bonus += CountFreeDiagonalSquares(x, y, -1, -1);

        // TODO: 비숍 페어 보너스 추가
        return bonus;
    }

    // 룩 위치 보너스 (열린 줄에 있으면 보너스)
    private int RookPositionBonus(int x, int y)
    {
        int bonus = 0;

        // 열린 줄 확인
        bool openFile = true;
        for (int i = 0; i < 8; i++)
        {
            if (i != y && GetPosition(x, i) != null)
            {
                openFile = false;
                break;
            }
        }
        if (openFile) bonus += 5;

        // TODO: 룩 연결 보너스 추가
        // TODO: 7랭크 보너스 추가
        return bonus;
    }

    // 퀸 위치 보너스 (중앙 위치일수록 높은 점수)
    private int QueenPositionBonus(int x, int y)
    {
        // TODO: 퀸 활동성 고려
        return (8 - Mathf.Abs(x - 3) - Mathf.Abs(y - 3));
    }

    // 킹 위치 보너스 (엔드게임에서는 중앙, 아니면 안전한 구석)
    private int KingPositionBonus(int x, int y, bool endGame)
    {
        if (endGame)
        {
            return (8 - Mathf.Abs(x - 3) - Mathf.Abs(y - 3)); // 엔드게임: 중앙 보너스
        }
        else
        {
            // TODO: 킹 안전 고려 (주변에 아군 기물 많으면 보너스)
            return (7 - y);
        }
    }

    // 주어진 방향으로 비숍이 움직일 수 있는 빈 칸 수 계산
    private int CountFreeDiagonalSquares(int x, int y, int dx, int dy)
    {
        int count = 0;
        while (PositionOnBoard(x + dx, y + dy) && GetPosition(x + dx, y + dy) == null)
        {
            count++;
            x += dx;
            y += dy;
        }
        return count;
    }

    // 엔드게임인지 판단 (단순화된 예시: 퀸이 없으면 엔드게임)
    private bool IsEndGame()
    {
        // TODO: 더 정확한 엔드게임 판단 로직 필요
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject piece = GetPosition(x, y);
                if (piece != null && (piece.name == "black_queen" || piece.name == "white_queen"))
                {
                    return false;
                }
            }
        }
        return true;
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
            case "black_king":
            case "white_king":
                return 100; // 킹의 가치는 게임의 승패와 직결되므로 매우 크게 설정
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