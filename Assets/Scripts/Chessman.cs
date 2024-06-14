
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    public GameObject controller;
    public GameObject movePlate;

    private int xBoard = -1;
    private int yBoard = -1;
public string player;


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

    public void OnMouseUp()
    {
        if (!controller.GetComponent<Game>().IsGameOver())
        {
            if (controller.GetComponent<Game>().GetCurrentPlayer() == player)
            {
                DestroyMovePlates();

                InitiateMovePlates();
            }
        }
    }

    public void SetXBoard(int x)
    {
        xBoard = x;
    }

    public void SetYBoard(int y)
    {
        yBoard = y;
    }

    public int GetXBoard()
    {
        return xBoard;
    }

    public int GetYBoard()
    {
        return yBoard;
    }

    public void DestroyMovePlates()
    {
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]);
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

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard - 1, yBoard);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    public void PawnMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();

        if (sc.PositionOnBoard(x, y))
        {
            if (sc.GetPosition(x, y) == null)
            {
                MovePlateSpawn(x, y);
            }

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

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.675f;
        y *= 0.68f;

        x += -2.35f;
        y += -2.8f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.675f;
        y *= 0.68f;

        x += -2.35f;
        y += -2.8f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);
        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
}


// 미니맥스 AI 클래스
 class MinimaxAI
{
    private GameObject bestPiece;
    private int depthLimit;
    private Game gameController;

    public MinimaxAI(Game gameController)
    {
        depthLimit = 3; // 미니맥스 탐색 깊이 (조정 가능)
        this.gameController = gameController;
    }

        public int Minimax(GameObject[,] board, int depth, bool isMaximizingPlayer, int alpha, int beta)
    {
        Debug.Log("Minimax 함수 실행 - depth: " + depth + ", isMaximizingPlayer: " + isMaximizingPlayer);
        if (depth == 0 || IsGameOver(board))
        {
            Debug.Log("종료 조건 충족 - depth: " + depth + ", isMaximizingPlayer: " + isMaximizingPlayer);
            return EvaluateBoard(board);
        }

        if (isMaximizingPlayer) 
        {
            Debug.Log("흑의 입장 (최대화)");
            return Maximize(board, depth, alpha, beta);
       
        }
        else
        {
            Debug.Log("백의 입장 (최소화)");
           // return Minimize(board, depth, alpha, beta);
           return 2;
        }
    } 

    // 흑의 입장 (최대화)
      // 흑의 입장 (최대화)
    private int Maximize(GameObject[,] board, int depth, int alpha, int beta)
    {
        Debug.Log("Maximize 함수 실행 - depth: " + depth + ", alpha: " + alpha + ", beta: " + beta);

        // 1. 흑의 이동 가능한 기물 목록 가져오기
        List<GameObject> blackMovablePieces = GetBlackMovablePieces(board);
        Debug.Log("흑 기물 목록 - " + blackMovablePieces.Count + "개");



        // 2. 최대 점수 초기화
        int bestScore = int.MinValue; 

        //여기까진 성공


        // 3. 각 기물에 대한 최적 이동 계산
        foreach (GameObject piece in blackMovablePieces)
        {
            Debug.Log("흑 기물 검사 - " + piece.name);
           bestScore = EvaluateBestMoveForPiece(board, depth, alpha, beta, piece, bestScore);

/*
            // 4. 알파-베타 가지치기 적용
            if (beta <= alpha)
            {
                Debug.Log("알파-베타 가지치기 - " + beta + " <= " + alpha);
                break; // 알파-베타 가지치기
            }

 */           
        }

        
/*
        Debug.Log("Maximize 함수 종료 - 최대 점수: " + bestScore);
        return bestScore;
        */
         return 1;
    }

    // 흑의 이동 가능한 기물 목록 가져오기
    private List<GameObject> GetBlackMovablePieces(GameObject[,] board)
    {
        Debug.Log("GetBlackMovablePieces 함수 실행");
        List<GameObject> movablePieces = new List<GameObject>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null && board[i, j].GetComponent<Chessman>().player == "black")
                {
                    // 기물의 이동 가능 여부 확인 (MovePlate 생성 여부 확인)
                    Chessman chessman = board[i, j].GetComponent<Chessman>();
                    chessman.DestroyMovePlates();
                    chessman.InitiateMovePlates();

                    if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
                    {
                        movablePieces.Add(board[i, j]);
                    }

                    chessman.DestroyMovePlates(); // MovePlate 제거
                }
            }
        }
        Debug.Log("GetBlackMovablePieces 함수 종료 - " + movablePieces.Count + "개 기물 반환");
        return movablePieces;
    }

   private int EvaluateBestMoveForPiece(GameObject[,] board, int depth, int alpha, int beta, GameObject piece, int bestScore)
{
    Debug.Log("EvaluateBestMoveForPiece 함수 실행 - " + piece.name);

    // 체스판 배열 출력
    Debug.Log("현재 체스판 상태:");
    for (int i = 0; i < 8; i++)
    {
        string row = "";
        for (int j = 0; j < 8; j++)
        {
            if (board[i, j] != null)
            {
                row += board[i, j].name + " ";
            }
            else
            {
                row += "null ";
            }
        }
        Debug.Log(row);
    }

    foreach (GameObject movePlate in GetMovePlates(piece))
    {
        Debug.Log("이동 가능 위치 검사 - " + movePlate.name);
        int score = Minimax(ApplyMove(board, piece, movePlate), depth - 1, false, alpha, beta);
//이 부분이 오류남

        // 체스판 배열 출력
        Debug.Log("이동 후 체스판 상태:");
        for (int i = 0; i < 8; i++)
        {
            string row = "";
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null)
                {
                    row += board[i, j].name + " ";
                }
                else
                {
                    row += "null ";
                }
            }
            Debug.Log(row);
        }

        /*
        Debug.Log("현재 이동 점수: " + score);
        if (score > bestScore)
        {
            Debug.Log("새로운 최대 점수 발견 - " + score);
            bestScore = score;
            alpha = Mathf.Max(alpha, bestScore);
            Debug.Log("alpha 업데이트 - " + alpha);
        }
        */
    }
    /*
    Debug.Log("EvaluateBestMoveForPiece 함수 종료 - 최대 점수: " + bestScore);
    return bestScore;
    */
    return 100;
}



    // 백의 입장 (최소화)
/*    private int Minimize(GameObject[,] board, int depth, int alpha, int beta)
    {
        Debug.Log("Minimize 함수 실행 - depth: " + depth + ", alpha: " + alpha + ", beta: " + beta);
        int bestScore = int.MaxValue;
        foreach (GameObject piece in GetMovablePieces(board, "white"))
        {
            Debug.Log("백 기물 검사 - " + piece.name);
            foreach (GameObject movePlate in GetMovePlates(piece))
            {
                Debug.Log("이동 가능 위치 검사 - " + movePlate.name);
                int score = Minimax(ApplyMove(board, piece, movePlate), depth - 1, true, alpha, beta);

                Debug.Log("현재 이동 점수: " + score);
                if (score < bestScore)
                {
                    Debug.Log("새로운 최소 점수 발견 - " + score);
                    bestScore = score;
                    beta = Mathf.Min(beta, bestScore);
                    Debug.Log("beta 업데이트 - " + beta);
                }

                if (beta <= alpha)
                {
                    Debug.Log("알파-베타 가지치기 - " + beta + " <= " + alpha);
                    break; // 알파-베타 가지치기
                }
            }
        }
        Debug.Log("Minimize 함수 종료 - 최소 점수: " + bestScore);
        return bestScore;
    }  */

    // 게임 종료 여부 확인 (게임 종료 조건에 따라 구현)
    
    private bool IsGameOver(GameObject[,] board)
    {
        // 1. 킹이 체크메이트 되었는지 확인
        if (IsCheckmate(board, "black"))
        {
            return true;
        }
        if (IsCheckmate(board, "white"))
        {
            return true;
        }

        // 2. 무승부 조건 확인
        if (IsStalemate(board, "black"))
        {
            return true;
        }
        if (IsStalemate(board, "white"))
        {
            return true;
        }

        // 3. 다른 무승부 조건 (예: 50수 규칙, 패터닝 등)
        // ... (필요한 경우 추가) ...

        return false; // 게임 종료 조건에 해당하지 않으면 false 반환
    }

    // 체크메이트 여부 확인
    private bool IsCheckmate(GameObject[,] board, string player)
    {
        // 1. 상대방 킹의 위치 찾기
        int kingX = -1, kingY = -1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null && board[i, j].GetComponent<Chessman>().name == player + "_king")
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

        // 2. 킹이 체크 상태인지 확인
        if (!IsChecked(board, kingX, kingY, player))
        {
            return false; // 체크 상태가 아니면 체크메이트 아님
        }

        // 3. 킹을 이동하거나 방어할 수 있는지 확인
        foreach (GameObject piece in GetMovablePieces(board, player))
        {
            foreach (GameObject movePlate in GetMovePlates(piece))
            {
                GameObject[,] newBoard = ApplyMove(board, piece, movePlate);
                if (!IsChecked(newBoard, kingX, kingY, player))
                {
                    return false; // 킹을 이동하거나 방어할 수 있으면 체크메이트 아님
                }
            }
        }

        return true; // 킹을 이동하거나 방어할 수 없으면 체크메이트
    }

    // 스테일메이트 여부 확인
    private bool IsStalemate(GameObject[,] board, string player)
    {
        // 1. 킹이 체크 상태가 아닌지 확인
        int kingX = -1, kingY = -1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null && board[i, j].GetComponent<Chessman>().name == player + "_king")
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

        if (IsChecked(board, kingX, kingY, player))
        {
            return false; // 체크 상태면 스테일메이트 아님
        }

        // 2. 이동 가능한 기물이 없는지 확인
        foreach (GameObject piece in GetMovablePieces(board, player))
        {
            foreach (GameObject movePlate in GetMovePlates(piece))
            {
                return false; // 이동 가능한 기물이 있으면 스테일메이트 아님
            }
        }

        return true; // 이동 가능한 기물이 없으면 스테일메이트
    }

    // 체크 여부 확인 (특정 위치의 기물이 체크 상태인지 확인)
    private bool IsChecked(GameObject[,] board, int x, int y, string player)
    {
        // 상대방 기물을 순회하며 킹을 공격할 수 있는지 확인
        // ... (체크 확인 로직 구현) ...
        return false; // 체크 상태가 아니면 false 반환
    }
    // 체스판 평가 함수 (흑에게 유리할수록 점수 높게 설정)
     // 체스판 평가 함수 (흑에게 유리할수록 점수 높게 설정)
    private int EvaluateBoard(GameObject[,] board)
    {
        int blackScore = 0;
        int whiteScore = 0;

        // 1. 기물 가치 계산
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null)
                {
                    string pieceName = board[i, j].GetComponent<Chessman>().name;
                    switch (pieceName)
                    {
                        case "black_queen": blackScore += 9; break;
                        case "black_rook": blackScore += 5; break;
                        case "black_bishop": blackScore += 3; break;
                        case "black_knight": blackScore += 3; break;
                        case "black_pawn": blackScore += 1; break;
                        case "white_queen": whiteScore += 9; break;
                        case "white_rook": whiteScore += 5; break;
                        case "white_bishop": whiteScore += 3; break;
                        case "white_knight": whiteScore += 3; break;
                        case "white_pawn": whiteScore += 1; break;
                    }
                }
            }
        }

        // 2. 킹의 안전성 (현재 위치에서 체크를 받는지 확인)
        int blackKingSafety = 0;
        int whiteKingSafety = 0;

        // 흑 킹의 안전성 확인
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null && board[i, j].GetComponent<Chessman>().name == "black_king")
                {
                    if (IsChecked(board, i, j, "black"))
                    {
                        blackKingSafety -= 5; // 체크를 받으면 감점
                    }
                }
            }
        }

        // 백 킹의 안전성 확인
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null && board[i, j].GetComponent<Chessman>().name == "white_king")
                {
                    if (IsChecked(board, i, j, "white"))
                    {
                        whiteKingSafety -= 5; // 체크를 받으면 감점
                    }
                }
            }
        }

        // 3. 기물의 위치 가치 (센터에 가까울수록 높은 점수 부여)
        int blackPositionValue = 0;
        int whitePositionValue = 0;

        // ... (기물 위치 가치 계산 로직 구현) ...

        // 4. 기물의 위협 (상대방 기물을 위협하는 경우 높은 점수 부여)
        int blackThreatValue = 0;
        int whiteThreatValue = 0;

        // ... (기물 위협 계산 로직 구현) ...

        // 흑과 백의 점수 합계 계산
        int blackTotalScore = blackScore + blackKingSafety + blackPositionValue + blackThreatValue;
        int whiteTotalScore = whiteScore + whiteKingSafety + whitePositionValue + whiteThreatValue;

        // 흑에게 유리할수록 높은 점수를 반환 (흑의 점수 - 백의 점수)
        return blackTotalScore - whiteTotalScore;
    }


    // 이동 가능한 기물 목록 가져오기
     // 이동 가능한 기물 목록 가져오기
    private List<GameObject> GetMovablePieces(GameObject[,] board, string player)
    {
        List<GameObject> movablePieces = new List<GameObject>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] != null && board[i, j].GetComponent<Chessman>().player == player)
                {
                    // 기물의 이동 가능 여부 확인 (MovePlate 생성 여부 확인)
                    Chessman chessman = board[i, j].GetComponent<Chessman>();
                    chessman.DestroyMovePlates();
                    chessman.InitiateMovePlates();

                    if (GameObject.FindGameObjectsWithTag("MovePlate").Length > 0)
                    {
                        movablePieces.Add(board[i, j]);
                    }

                    chessman.DestroyMovePlates(); // MovePlate 제거
                }
            }
        }
        return movablePieces;
    }

    // 기물의 MovePlate 목록 가져오기
    private List<GameObject> GetMovePlates(GameObject piece)
    {
        List<GameObject> movePlates = new List<GameObject>();
        foreach (GameObject movePlate in GameObject.FindGameObjectsWithTag("MovePlate"))
        {
            if (movePlate.GetComponent<MovePlate>().GetReference() == piece)
            {
                movePlates.Add(movePlate);
            }
        }
        return movePlates;
    }


    // 기물 이동 적용 (체스판 상태 업데이트)
     private GameObject[,] ApplyMove(GameObject[,] board, GameObject piece, GameObject movePlate)
    {

 /*       
        // 1. MovePlate 정보를 이용하여 이동할 위치 계산
      int targetX = movePlate.GetComponent<MovePlate>().matrixX;
        int targetY = movePlate.GetComponent<MovePlate>().matrixY;

        // 2. 기물의 현재 위치를 비워줍니다.
        int currentX = piece.GetComponent<Chessman>().GetXBoard();
        int currentY = piece.GetComponent<Chessman>().GetYBoard();
        board[currentX, currentY] = null; 

        // 3. 이동할 위치에 기물을 배치합니다.
        board[targetX, targetY] = piece; 

        // 4. 기물의 위치 정보 업데이트
        piece.GetComponent<Chessman>().SetXBoard(targetX);
        piece.GetComponent<Chessman>().SetYBoard(targetY);

        // 5. MovePlate가 공격용인 경우, 공격 대상 기물 제거
        if (movePlate.GetComponent<MovePlate>().attack)
        {
            // 공격 대상 기물은 이미 board에서 제거되었으므로 
            // Destroy(movePlate.GetComponent<MovePlate>().GetReference()); 는 불필요합니다.
        }
*/
 Debug.Log("여기까지 도착");
        // 6. 업데이트된 체스판 배열 반환
        return board;
    }

    // 미니맥스 알고리즘에서 최고 점수를 가진 기물 반환
    public GameObject GetBestPiece()
    {
        return bestPiece;
    }
}






