using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class Game : MonoBehaviour
{
    public GameObject chesspiece; //체스 기물

    //체스에서 흑과 백 각각 갖고 있는 기물의 총 개수는 16개
    //기물들의 위치를 저장
    //체스 보드판은 8*8 사이즈의 64개 칸으로 이루어져있음. 이를 2차원 배열로 저장함
    public GameObject[,] boardPositions = new GameObject[8, 8];
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

        //기물들의 초기위치 설정 밑  생성
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
                 AITurn(); 
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

    //좌표가 범위 내에 있는지 확인
    public bool PositionOnBoard(int x, int y)
    {
        if (x < 0 || y < 0 || x >= boardPositions.GetLength(0) || y >= boardPositions.GetLength(1)) return false;
        return true;
    }



//AI 개발

  public void AITurn()
    {
        // 미니맥스 알고리즘으로 최적의 이동을 계산
        Move bestMove = Minimax(5, true, float.MinValue, float.MaxValue);

        // 계산된 최적의 이동을 실행
        MovePiece(bestMove.xFrom, bestMove.yFrom, bestMove.xTo, bestMove.yTo);

        // 턴 변경
        NextTurn();
    }

    // 미니맥스 알고리즘
    private Move Minimax(int depth, bool isMaximizingPlayer, float alpha, float beta)
    {
        // 게임 종료 조건 (체크메이트, 스테일메이트, 깊이 제한)
        if (IsGameOver() || depth == 0)
        {
            return new Move(0, 0, 0, 0, EvaluateBoard());
        }

        // 최적의 이동을 저장할 변수
        Move bestMove = new Move(0, 0, 0, 0, isMaximizingPlayer ? float.MinValue : float.MaxValue);

        // 현재 플레이어의 모든 가능한 이동을 확인
        IEnumerable<Move> possibleMoves = GetPossibleMoves(currentPlayer);

        // 각 이동에 대해 미니맥스 알고리즘을 재귀적으로 호출
        foreach (Move move in possibleMoves)
        {
            // 이동을 가상으로 실행
            MovePiece(move.xFrom, move.yFrom, move.xTo, move.yTo);

            // 다음 깊이에서 미니맥스 알고리즘 실행
            float score = Minimax(depth - 1, !isMaximizingPlayer, alpha, beta).score;

            // 가상 이동 취소
            UndoMove(move.xFrom, move.yFrom, move.xTo, move.yTo);

            // 최적의 이동 업데이트
            if (isMaximizingPlayer)
            {
                // 최대화 플레이어 (AI)
                if (score > bestMove.score)
                {
                    bestMove = new Move(move.xFrom, move.yFrom, move.xTo, move.yTo, score);
                    alpha = Mathf.Max(alpha, score);
                }
            }
            else
            {
                // 최소화 플레이어 (사람)
                if (score < bestMove.score)
                {
                    bestMove = new Move(move.xFrom, move.yFrom, move.xTo, move.yTo, score);
                    beta = Mathf.Min(beta, score);
                }
            }

            // 알파-베타 가지치기
            if (beta <= alpha)
            {
                break;
            }
        }

        return bestMove;
    }

    // 체스판 평가 함수
    private float EvaluateBoard()
    {
        float score = 0;

        // 각 기물의 가치를 점수에 반영
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject piece = GetPosition(i, j);
                if (piece != null)
                {
                    if (piece.GetComponent<Chessman>().GetPlayer() == "white")
                    {
                        // 흰색 기물: 점수 증가
                        score += GetPieceValue(piece);
                    }
                    else
                    {
                        // 검은색 기물: 점수 감소
                        score -= GetPieceValue(piece);
                    }
                }
            }
        }

        // 체크 상태일 경우 점수 조정
        if (IsCheck(currentPlayer))
        {
            score -= 100;
        }

        // AI 입장에서 유리한 상태일수록 높은 점수를 반환
        return score;
    }

    // 기물 가치를 반환하는 함수
    private float GetPieceValue(GameObject piece)
    {
        switch (piece.name)
        {
            case "white_queen":
            case "black_queen":
                return 9;
            case "white_rook":
            case "black_rook":
                return 5;
            case "white_knight":
            case "black_knight":
            case "white_bishop":
            case "black_bishop":
                return 3;
            case "white_king":
            case "black_king":
                return 1000;
            case "white_pawn":
            case "black_pawn":
                return 1;
            default:
                return 0;
        }
    }

    // 체크 여부를 판별하는 함수
    private bool IsCheck(string player)
    {
        // 현재 턴 플레이어의 왕 위치 찾기
        int kingX = -1;
        int kingY = -1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (GetPosition(i, j) != null &&
                    GetPosition(i, j).GetComponent<Chessman>().name == (player == "white" ? "white_king" : "black_king"))
                {
                    kingX = i;
                    kingY = j;
                    break;
                }
            }
            if (kingX != -1)
            {
                break;
            }
        }

        // 왕이 체크 상태인지 확인
        if (kingX != -1 && kingY != -1)
        {
            // 상대방 기물의 모든 가능한 이동을 확인
            IEnumerable<Move> possibleMoves = GetPossibleMoves(player == "white" ? "black" : "white");
            foreach (Move move in possibleMoves)
            {
                if (move.xTo == kingX && move.yTo == kingY)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // 가능한 이동 목록을 반환하는 함수
    private IEnumerable<Move> GetPossibleMoves(string player)
    {
        List<Move> possibleMoves = new List<Move>();

        // 체스판을 순회하며 각 기물의 가능한 이동을 확인
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject piece = GetPosition(i, j);
                if (piece != null && piece.GetComponent<Chessman>().GetPlayer() == player)
                {
                    // 기물의 가능한 이동 목록을 생성
                    List<Move> pieceMoves = piece.GetComponent<Chessman>().GetPossibleMoves(i, j);
                    foreach (Move move in pieceMoves)
                    {
                        // 체크 상황에서 이동 가능한지 확인
                        if (IsCheck(player, move.xTo, move.yTo))
                        {
                            continue;
                        }

                        // 가능한 이동 목록에 추가
                        possibleMoves.Add(new Move(i, j, move.xTo, move.yTo, 0));
                    }
                }
            }
        }

        return possibleMoves;
    }

    // 체크 상황에서 이동 가능한지 확인하는 함수
    private bool IsCheck(string player, int xTo, int yTo)
    {
        // 현재 턴 플레이어의 왕 위치 찾기
        int kingX = -1;
        int kingY = -1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (GetPosition(i, j) != null &&
                    GetPosition(i, j).GetComponent<Chessman>().name == (player == "white" ? "white_king" : "black_king"))
                {
                    kingX = i;
                    kingY = j;
                    break;
                }
            }
            if (kingX != -1)
            {
                break;
            }
        }

        // 이동 후 왕이 체크 상태인지 확인
        if (kingX != -1 && kingY != -1)
        {
            // 이동을 가상으로 실행
            MovePiece(xTo, yTo, kingX, kingY);
            if (IsCheck(player == "white" ? "black" : "white"))
            {
                // 이동 취소
                UndoMove(xTo, yTo, kingX, kingY);
                return true;
            }
            // 이동 취소
            UndoMove(xTo, yTo, kingX, kingY);
        }

        return false;
    }

    // 기물 이동 함수
    private void MovePiece(int xFrom, int yFrom, int xTo, int yTo)
    {
        // 체스판 배열 업데이트
        GameObject piece = GetPosition(xFrom, yFrom);
        SetPositionEmpty(xFrom, yFrom);
        SetPosition(piece, xTo, yTo);

        // 실제 기물의 위치를 변경
        piece.GetComponent<Chessman>().SetXBoard(xTo);
        piece.GetComponent<Chessman>().SetYBoard(yTo);
        piece.GetComponent<Chessman>().SetCoords();
    }

    // 이동 취소 함수
    private void UndoMove(int xFrom, int yFrom, int xTo, int yTo)
    {
        // 체스판 배열 복구
        GameObject piece = GetPosition(xTo, yTo);
        SetPositionEmpty(xTo, yTo);
        SetPosition(piece, xFrom, yFrom);

        // 실제 기물의 위치를 복구
        piece.GetComponent<Chessman>().SetXBoard(xFrom);
        piece.GetComponent<Chessman>().SetYBoard(yFrom);
        piece.GetComponent<Chessman>().SetCoords();
    }

    // 기물을 체스판 배열에 추가하는 함수 (기존 함수 개선)
    public void SetPosition(GameObject obj, int x, int y)
    {
        boardPositions[x, y] = obj;
    }

    public struct Move
    {
        public int xFrom;
        public int yFrom;
        public int xTo;
        public int yTo;
        public float score;

        public Move(int xFrom, int yFrom, int xTo, int yTo, float score)
        {
            this.xFrom = xFrom;
            this.yFrom = yFrom;
            this.xTo = xTo;
            this.yTo = yTo;
            this.score = score;
        }
    }



}