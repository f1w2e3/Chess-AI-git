using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 startPosition;
    private Vector3 offset;

    private void OnMouseDown()
    {
        // 마우스가 클릭된 위치로부터 기물의 위치까지의 거리를 계산하여 offset을 설정합니다.
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            // 마우스가 이동한 위치로 기물을 이동시킵니다.
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        // 기물이 마우스에서 놓인 위치에 대한 추가 처리를 할 수 있습니다.
    }
    // 밑으로는 각 기물들의 이동가능 위치를 계산하는 코드입니다.
    public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King };
    public PieceType pieceType;

    public Vector2[] GetValidMoves()
    {
        Vector2[] validMoves = null;

        switch (pieceType)
        {
            case PieceType.Pawn:
                // 폰의 이동 가능한 위치 계산
                validMoves = new Vector2[2];
                // (1, 0)과 (2, 0)으로 이동 가능
                validMoves[0] = new Vector2(transform.position.x + 1, transform.position.y);
                validMoves[1] = new Vector2(transform.position.x + 2, transform.position.y);
                break;

            case PieceType.Rook:
                // 룩의 이동 가능한 위치 계산
                // 세로와 가로로 이동 가능
                break;

            case PieceType.Knight:
                // 나이트의 이동 가능한 위치 계산
                // L자 모양으로 이동 가능
                break;

            case PieceType.Bishop:
                // 비숍의 이동 가능한 위치 계산 
                // 대각선으로 이동 가능
                break;

            case PieceType.Queen:
                // 퀸의 이동 가능한 위치 계산
                // 룩과 비숍의 이동을 합친 형태
                break;

            case PieceType.King:
                // 킹의 이동 가능한 위치 계산
                // 상하좌우, 대각선으로 한 칸씩 이동 가능
                break;
        }

        return validMoves;
    }
}
