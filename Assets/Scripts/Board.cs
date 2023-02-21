using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;       // �̱���

    public GameObject cell;         // ��
    public float cell_CreateX;      // �� ó�� X�� ��ġ
    public float cell_CreateY;      // �� ó�� Y�� ��ġ
    public float cell_NextX;        // ������� �� X�� ��ġ
    public float cell_NextY;        // ������� �� Y�� ��ġ
    public int board_X;             // ���� x�� ����
    public int board_Y;             // ���� y�� ����

    public List<GameObject> enable_Cells = new List<GameObject>();      // ������� ��
    public List<GameObject> disable_Cells = new List<GameObject>();     // �̻������ ��

    public Vector3 empty_Position;                                      // �� ���� ��ġ
    public List<GameObject> down_Cells = new List<GameObject>();        // �� ������ �������� �ִ� ��

    bool isEmpty = false;


    void Awake()
    {
        instance = this;

        cell_NextX = cell_CreateX;
        cell_NextY = cell_CreateY;
    }

    void Start()
    {
        InstantiateCellBoard();
    }

    void FixedUpdate()
    {
        if (isEmpty)
        {
            // ��� ���� ���� ������ �� ĭ�� ������
            for (int i = 0; i < enable_Cells.Count; i++)
            {
                if (enable_Cells[i].transform.position.y > empty_Position.y && enable_Cells[i].transform.position.x == empty_Position.x)
                {
                    enable_Cells[i].transform.position = Vector2.MoveTowards(enable_Cells[i].transform.position, new Vector2(empty_Position.x, cell_CreateY), -2.5f * Time.deltaTime);

                    // ���� �� �����Դٸ� ������
                    if (enable_Cells[i].transform.position.y == empty_Position.y && enable_Cells[i].transform.position.x == empty_Position.x)
                    {
                        isEmpty = false;
                        break;
                    }
                }
            }
        }
    }

    // ���� ����
    void InstantiateCellBoard()
    {
        for (int i = 0; i < board_Y; i++)
        {
            for (int j = 0; j < board_X; j++)
            {
                GameObject current_Cell = Instantiate(cell);
                current_Cell.transform.position = new Vector2(cell_NextX, cell_NextY);
                cell_NextX += 0.5f;
            }
            cell_NextX = cell_CreateX;
            cell_NextY -= 0.5f;
        }
    }

    
    // �� ���� ä���
    public void FillEmptyBoard(GameObject disappear_Cell)
    {
        isEmpty = true;
        empty_Position = disappear_Cell.transform.position;
        /*
        // ��� ���� ���� ������ �� ĭ�� ������
        for (int i = 0; i < enable_Cells.Count; i++)
        {
            if (enable_Cells[i].transform.position.y > disappear_Cell.transform.position.y && enable_Cells[i].transform.position.x == disappear_Cell.transform.position.x)
            {
                enable_Cells[i].transform.position = new Vector2(enable_Cells[i].transform.position.x, enable_Cells[i].transform.position.y - 0.5f);
            }
        }

        // �̻������ ���� ������ �� ���κ��� ä��
        if (disable_Cells.Count != 0)
        {
            GameObject show_Cell = disable_Cells[0];
            show_Cell.SetActive(true);
            show_Cell.transform.position = new Vector2(disappear_Cell.transform.position.x, cell_CreateY);
        }
        // �̻������ ���� ������ ���� �����ؼ� �� ���κ��� ä��
        else
        {
            GameObject instantiate_Cell = Instantiate(cell);
            instantiate_Cell.transform.position = new Vector2(disappear_Cell.transform.position.x, cell_CreateY);
        }
        */
        disappear_Cell.SetActive(false);
    }

    // �� �Ʒ��� �̵�
    void FillEmptyCell(GameObject own_Cell, Vector3 empty_Position)
    {
        own_Cell.transform.position = Vector2.MoveTowards(own_Cell.transform.position, new Vector2(empty_Position.x, cell_CreateY), 2.5f * Time.deltaTime);
    }
}
