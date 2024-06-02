using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;

    private void OnMouseDown()
    {
        // ���콺�� �⹰ ������ Ŭ���Ǿ��� ��
        offset = gameObject.transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        // ���콺�� �⹰ ������ �巡�׵ǰ� ���� ��
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPos() + offset;
            transform.position = newPosition;
        }
    }

    private void OnMouseUp()
    {
        // ���콺 ��ư�� ������ ��
        isDragging = false;
    }

    private Vector3 GetMouseWorldPos()
    {
        // ���콺 ��ġ�� ȭ�� ���� ��ǥ�� ��ȯ
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void SnapToGrid()
    {
        // ���� ������ ��ġ�� �⹰�� �̵���ŵ�ϴ�
        transform.position = new Vector3(Mathf.Round(transform.position.x),
                                         Mathf.Round(transform.position.y),
                                         Mathf.Round(transform.position.z));
    }
}


