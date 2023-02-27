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

    public Dictionary<decimal, int> empty_X = new Dictionary<decimal, int>();           // �� ���� x�� ��ġ�� ����, �ε��Ҽ���(float) ���� ������ �� ���׳��� decimal ���
    public List<GameObject> down_Cells = new List<GameObject>();                        // �� ������ �������� �ִ� ��

    Dictionary<GameObject, Vector3> target_Positions = new Dictionary<GameObject, Vector3>();       // ������ ������ ��ǥ ��ġ

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
        // ����ִ� ������ �ִٸ�
        if (empty_X.Count != 0)
        {
            FallingCell();
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

    
    // �� ������ ä�� �� ����
    public void FillEmptyBoard(Vector3 empty_Position)
    {
        decimal x = (decimal)empty_Position.x;
        Debug.Log(empty_X.ContainsKey(x));
        if (!empty_X.ContainsKey(x))
        {
            empty_X.Add(x, 1);
        }
        else
        {
            empty_X[x]++;
        }

        // �̻������ ���� ������ �� ���κ��� ä��
        if (disable_Cells.Count != 0)
        {
            GameObject show_Cell = disable_Cells[0];
            show_Cell.SetActive(true);
            show_Cell.transform.position = new Vector2((float)x, cell_CreateY + 0.5f * empty_X[x]);
        }
        // �̻������ ���� ������ ���� �����ؼ� �� ���κ��� ä��
        else
        {
            GameObject instantiate_Cell = Instantiate(cell);
            instantiate_Cell.transform.position = new Vector2((float)x, cell_CreateY + 0.5f * empty_X[x]);
        }

        SetFallingCell(empty_Position);
    }

    // �� ������ ä�� ���� ��ǥ ��ġ ����
    void SetFallingCell(Vector3 empty_Position)
    {
        decimal x = (decimal)empty_Position.x;
        float tmp = 0.5f * empty_X[x];

        for (int j = 0; j < enable_Cells.Count; j++)
        {
            if (enable_Cells[j].transform.position.y > empty_Position.y && enable_Cells[j].transform.position.x == empty_Position.x)
            {
                Debug.Log(tmp);
                // ������ �� ��Ͽ� ���ٸ� �߰�
                if (!target_Positions.ContainsKey(enable_Cells[j]))
                {
                    target_Positions.Add(enable_Cells[j], new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - tmp));
                }
                // ������ �� ��Ͽ� �ִٸ� ����
                else
                {
                    target_Positions[enable_Cells[j]] = new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - tmp);
                }
            }
        }
    }

    // �� �Ʒ��� �̵�
    void FallingCell()
    {
        for (int i = empty_X.Count - 1; i >= 0; i--)
        {
            decimal x = empty_X.Keys.ToList()[i];
            int check_Cell = 0;

            for (int j = target_Positions.Count - 1; j >= 0; j--)
            {
                GameObject obj = target_Positions.Keys.ToList()[j];
                if (x == (decimal)obj.transform.position.x)
                {
                    obj.transform.position = Vector2.MoveTowards(obj.transform.position, target_Positions[obj], 2.5f * Time.deltaTime);

                    if (obj.transform.position == target_Positions[obj])
                    {
                        target_Positions.Remove(obj);
                    }
                }
                else
                {
                    check_Cell++;
                }
            }

            if (check_Cell >= target_Positions.Count)
            {
                empty_X.Remove(x);
            }
        }
    }
}
