using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class MinimaxAI : MonoBehaviour
{
    // 체스 보드판
    public ChessBoard board;

    // 최대 탐색 깊이 4
    public int maxDepth = 4;

    // 미니맥스 알고리즘
    public Move GetBestMove()
    {
        // 현재 턴의 플레이어 (흑팀)
        var currentPlayer = board.CurrentPlayer;

        // 최대 깊이까지 탐색하여 최적의 수를 찾습니다.
        var bestMove = Minimax(currentPlayer, maxDepth, int.MinValue, int.MaxValue);

        // 최적의 수를 반환합니다.
        return bestMove;
    }

    // 미니맥스 알고리즘
    private Move Minimax(Player player, int depth, int alpha, int beta)
    {
        // 기저 조건: 최대 깊이에 도달하거나 게임이 끝났을 때
        if (depth == 0 || board.IsGameOver())
        {
            // 게임이 끝났으면 결과에 따라 점수를 반환합니다.
            if (board.IsGameOver())
            {
                return new Move(null, board.GetResult(player));
            }
            // 게임이 끝나지 않았으면 평가 함수를 사용하여 점수를 반환합니다.
            else
            {
                return new Move(null, evaluate(player)); 
                // 평가함수 evaluate()를 통해서 현재 얼마나 유리한지 불리한지 판별함. 
            }
        }

        // 현재 턴의 플레이어가 흑팀인지 백팀인지에 따라 최대 또는 최소 값을 찾습니다.
        var bestMove = (player == Player.Black) ? new Move(null, int.MinValue) : new Move(null, int.MaxValue);

        // 모든 가능한 수를 탐색합니다.
        foreach (var move in board.GetValidMoves(player))
        {
            // 해당 수를 실행합니다.
            board.MakeMove(move);

            // 다음 턴의 플레이어로 재귀 호출합니다.
            var score = Minimax(board.CurrentPlayer, depth - 1, alpha, beta);

            // 해당 수를 취소합니다. 가상으로 미리 기물을 움직여본 뒤 다시 원래대로 되돌리는 작업.
            board.UndoMove(move);

            // 현재 턴의 플레이어가 흑팀인 경우 최대 값을 찾습니다.
            if (player == Player.Black && score.Score > bestMove.Score)
            {
                bestMove = new Move(move, score.Score);
                alpha = System.Math.Max(alpha, score.Score);
            }
            // 현재 턴의 플레이어가 백팀인 경우 최소 값을 찾습니다.
            else if (player == Player.White && score.Score < bestMove.Score)
            {
                bestMove = new Move(move, score.Score);
                beta = System.Math.Min(beta, score.Score);
            }

            // 알파-베타 가지치기
            if (beta <= alpha)
            {
                break;
            }
        }

        // 최적의 수를 반환합니다.
        return bestMove;
    }

    // 체스 판의 상태를 평가하는 함수 (예: 백팀의 기물 개수가 많을수록 높은 점수)
    private int evaluate(Player player)
    {
        // 평가 함수를 구현합니다.
        /* 예: 
        1.백팀의 기물 개수가 많을수록 높은 점수를 반환합니다.
        2.킹, 퀸 등 중요 기물이 얼마나 보드판 위에서 안전한 위치에 놓여있는지에 따라 가중치를 부여할수도 있음.
        */
        var whitePieces = board.GetPieces(Player.White).Count;
        var blackPieces = board.GetPieces(Player.Black).Count;
        return whitePieces - blackPieces;
    }
}