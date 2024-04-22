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
        // ���콺�� Ŭ���� ��ġ�κ��� �⹰�� ��ġ������ �Ÿ��� ����Ͽ� offset�� �����մϴ�.
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            // ���콺�� �̵��� ��ġ�� �⹰�� �̵���ŵ�ϴ�.
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        // �⹰�� ���콺���� ���� ��ġ�� ���� �߰� ó���� �� �� �ֽ��ϴ�.
    }
    // �����δ� �� �⹰���� �̵����� ��ġ�� ����ϴ� �ڵ��Դϴ�.
    public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King };
    public PieceType pieceType;

    public Vector2[] GetValidMoves()
    {
        Vector2[] validMoves = null;

        switch (pieceType)
        {
            case PieceType.Pawn:
                // ���� �̵� ������ ��ġ ���
                validMoves = new Vector2[2];
                // (1, 0)�� (2, 0)���� �̵� ����
                validMoves[0] = new Vector2(transform.position.x + 1, transform.position.y);
                validMoves[1] = new Vector2(transform.position.x + 2, transform.position.y);
                break;

            case PieceType.Rook:
                // ���� �̵� ������ ��ġ ���
                // ���ο� ���η� �̵� ����
                break;

            case PieceType.Knight:
                // ����Ʈ�� �̵� ������ ��ġ ���
                // L�� ������� �̵� ����
                break;

            case PieceType.Bishop:
                // ����� �̵� ������ ��ġ ��� 
                // �밢������ �̵� ����
                break;

            case PieceType.Queen:
                // ���� �̵� ������ ��ġ ���
                // ��� ����� �̵��� ��ģ ����
                break;

            case PieceType.King:
                // ŷ�� �̵� ������ ��ġ ���
                // �����¿�, �밢������ �� ĭ�� �̵� ����
                break;
        }

        return validMoves;
    }
}
