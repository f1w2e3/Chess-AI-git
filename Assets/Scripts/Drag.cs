using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;

    private void OnMouseDown()
    {
        // 마우스가 기물 위에서 클릭되었을 때
        offset = gameObject.transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        // 마우스가 기물 위에서 드래그되고 있을 때
        if (isDragging)
        {
            Vector3 newPosition = GetMouseWorldPos() + offset;
            transform.position = newPosition;
        }
    }

    private void OnMouseUp()
    {
        // 마우스 버튼을 놓았을 때
        isDragging = false;
    }

    private Vector3 GetMouseWorldPos()
    {
        // 마우스 위치를 화면 상의 좌표로 변환
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void SnapToGrid()
    {
        // 각기 정해진 위치로 기물을 이동시킵니다
        transform.position = new Vector3(Mathf.Round(transform.position.x),
                                         Mathf.Round(transform.position.y),
                                         Mathf.Round(transform.position.z));
    }
}
