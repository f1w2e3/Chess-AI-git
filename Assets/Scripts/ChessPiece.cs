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
                //룩의 이동 가능한 위치 계산
                // 수평이동
                List<Vector2> rookMoves = new List<Vector2>();
                for (int i = -7; i <= 7; i++)
                {
                    if (i != 0)
                    {
                        rookMoves.Add(new Vector2(transform.position.x + i, transform.position.y));
                        rookMoves.Add(new Vector2(transform.position.x, transform.position.y + i));
                    }
                }
                validMoves = rookMoves.ToArray();
                break;

            case PieceType.Knight:
                //나이트의 이동 가능 위치 계싼
                List<Vector2> knightMoves = new List<Vector2>();
                knightMoves.Add(new Vector2(transform.position.x + 2, transform.position.y + 1));
                knightMoves.Add(new Vector2(transform.position.x + 2, transform.position.y - 1));
                knightMoves.Add(new Vector2(transform.position.x - 2, transform.position.y + 1));
                knightMoves.Add(new Vector2(transform.position.x - 2, transform.position.y - 1));
                knightMoves.Add(new Vector2(transform.position.x + 1, transform.position.y + 2));
                knightMoves.Add(new Vector2(transform.position.x + 1, transform.position.y - 2));
                knightMoves.Add(new Vector2(transform.position.x - 1, transform.position.y + 2));
                knightMoves.Add(new Vector2(transform.position.x - 1, transform.position.y - 2));
                validMoves = knightMoves.ToArray();
                break;
        

            case PieceType.Bishop:
                // 비숍 이동 가능 위치 계산
                List<Vector2> bishopMoves = new List<Vector2>();
                for (int i = -7; i <= 7; i++)
                {
                    if (i != 0)
                    {
                        bishopMoves.Add(new Vector2(transform.position.x + i, transform.position.y + i));
                        bishopMoves.Add(new Vector2(transform.position.x - i, transform.position.y + i));
                    }
                }
                validMoves = bishopMoves.ToArray();
                break;

            case PieceType.Queen:
                // 퀸의 이동 가능한 위치 계산
                // 퀸의 이동
                List<Vector2> queenMoves = new List<Vector2>();
                for (int i = -7; i <= 7; i++)
                {
                    if (i != 0)
                    {
                        queenMoves.Add(new Vector2(transform.position.x + i, transform.position.y + i));
                        queenMoves.Add(new Vector2(transform.position.x - i, transform.position.y + i));
                    }
                }
                validMoves = queenMoves.ToArray();
                break;

            case PieceType.King:
                //킹의 이동 가능위치들
                List<Vector2> kingMoves = new List<Vector2>();
                // 상하좌우 이동
                kingMoves.Add(new Vector2(transform.position.x + 1, transform.position.y));
                kingMoves.Add(new Vector2(transform.position.x - 1, transform.position.y));
                kingMoves.Add(new Vector2(transform.position.x, transform.position.y + 1));
                kingMoves.Add(new Vector2(transform.position.x, transform.position.y - 1));
                // 대각선 이동
                kingMoves.Add(new Vector2(transform.position.x + 1, transform.position.y + 1));
                kingMoves.Add(new Vector2(transform.position.x - 1, transform.position.y + 1));
                kingMoves.Add(new Vector2(transform.position.x + 1, transform.position.y - 1));
                kingMoves.Add(new Vector2(transform.position.x - 1, transform.position.y - 1));
                validMoves = kingMoves.ToArray();
                break;
        }

        return validMoves;
    }






}
