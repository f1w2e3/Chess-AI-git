using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
public GameObject controller;

GameObject reference = null;

<<<<<<< HEAD
<<<<<<< HEAD
// 이동 좌표
public int matrixX;
public int matrixY;

// false = 이동, true = 공격
public bool attack = false;
=======
=======
>>>>>>> parent of 5e7441a (나이트 대체 카멜, 폰의 최초 이동시 두칸이동 구현)
    //ü���� ������
    int matrixX;
    int matrixY;

    //false = �̵�, true = ����
    public bool attack = false;
>>>>>>> parent of 5e7441a (나이트 대체 카멜, 폰의 최초 이동시 두칸이동 구현)

public void Start()
{
    if (attack)
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }
}

public void OnMouseUp()
{
    controller = GameObject.FindGameObjectWithTag("GameController");

    if (attack)
    {
        GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

        Destroy(cp);

         if (cp.name == "white_king")
    {
        controller.GetComponent<Game>().Winner("Black"); // 흑 승리 처리
    }
    else if (cp.name == "black_king")
    {
        controller.GetComponent<Game>().Winner("White"); // 백 승리 처리
    }
    }

    controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Chessman>().GetXBoard(),
        reference.GetComponent<Chessman>().GetYBoard());

    reference.GetComponent<Chessman>().SetXBoard(matrixX);
    reference.GetComponent<Chessman>().SetYBoard(matrixY);
    reference.GetComponent<Chessman>().SetCoords();

    controller.GetComponent<Game>().SetPosition(reference);

    controller.GetComponent<Game>().NextTurn();

    reference.GetComponent<Chessman>().DestroyMovePlates();
}

public void SetCoords(int x, int y)
{
    matrixX = x;
    matrixY = y;
}

public void SetReference(GameObject obj)
{
    reference = obj;
}

public GameObject GetReference()
{
    return reference;
}

// 랜덤으로 이동 경로를 선택하여 이동
public void ExecuteRandomMove()
{
    GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
    if (movePlates.Length > 0)
    {
<<<<<<< HEAD
        int index = Random.Range(0, movePlates.Length);
        movePlates[index].GetComponent<MovePlate>().OnMouseUp();
=======
        controller = GameObject.FindGameObjectWithTag("GameController");

        if (attack)
        {
            GameObject cp = controller.GetComponent<Game>().GetPosition(matrixX, matrixY);

            Destroy(cp);
        }

        controller.GetComponent<Game>().SetPositionEmpty(reference.GetComponent<Chessman>().GetXBoard(),
            reference.GetComponent<Chessman>().GetYBoard());

        reference.GetComponent<Chessman>().SetXBoard(matrixX);
        reference.GetComponent<Chessman>().SetYBoard(matrixY);
        reference.GetComponent<Chessman>().SetCoords();

        controller.GetComponent<Game>().SetPosition(reference);

        reference.GetComponent<Chessman>().DestroyMovePlates();
    }

    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
>>>>>>> parent of 5e7441a (나이트 대체 카멜, 폰의 최초 이동시 두칸이동 구현)
    }
}
}