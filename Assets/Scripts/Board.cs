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

    public List<Vector3> empty_Positions = new List<Vector3>();         // �� ���� ��ġ
    public List<GameObject> down_Cells = new List<GameObject>();        // �� ������ �������� �ִ� ��

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
        if (empty_Positions.Count != 0)
        {
            Debug.Log("����ִ� ���� ����");
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

    
    // �� ���� ä��� (���� �̸� ���� �ʿ�, �ڵ嵵 2���� �������� ��)
    public void FillEmptyBoard(GameObject disappear_Cell)
    {
        // �̻������ ���� ������ �� ���κ��� ä��
        if (disable_Cells.Count != 0)
        {
            GameObject show_Cell = disable_Cells[0];
            show_Cell.SetActive(true);
            show_Cell.transform.position = new Vector2(disappear_Cell.transform.position.x, cell_CreateY + 0.5f * empty_Positions.Count);
        }
        // �̻������ ���� ������ ���� �����ؼ� �� ���κ��� ä��
        else
        {
            GameObject instantiate_Cell = Instantiate(cell);
            instantiate_Cell.transform.position = new Vector2(disappear_Cell.transform.position.x, cell_CreateY + 0.5f * empty_Positions.Count);
        }
        
        
        disappear_Cell.SetActive(false);
        empty_Positions.Add(disappear_Cell.transform.position);

        // -------------------------------------------------------------------------------
        /*
        // ������ �� ���� ��ġ�� ���鸸 �ڽ� ��ġ�� �ٷ� �Ʒ� ���� ��ġ�� ��� ��ųʸ��� ����
        for (int i = 0; i < empty_Positions.Count; i++)
        {
            for (int j = 0; j < enable_Cells.Count; j++)
            {
                if (enable_Cells[j].transform.position.y > empty_Positions[i].y && enable_Cells[j].transform.position.x == empty_Positions[i].x)
                {
                    Debug.Log("1");
                    // ������ �� ��Ͽ� ���ٸ� �߰�
                    if (!target_Positions.ContainsKey(enable_Cells[j]))
                    {
                        Debug.Log("����");
                        target_Positions.Add(enable_Cells[j], new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - 0.5f * empty_Positions.Count));
                    }
                    // ������ �� ��Ͽ� �ִٸ� ����
                    else
                    {
                        target_Positions[enable_Cells[j]] = new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - 0.5f * empty_Positions.Count);
                    }
                }
            }
        }
        */

        SetFallingCell(disappear_Cell);
    }

    // ������ �� ���� ��ġ�� ���鸸 �ڽ� ��ġ�� �ٷ� �Ʒ� ���� ��ġ�� ��� ��ųʸ��� ����
    void SetFallingCell(GameObject disappear_Cell)
    {
        for (int j = 0; j < enable_Cells.Count; j++)
        {
            if (enable_Cells[j].transform.position.y > disappear_Cell.transform.position.y && enable_Cells[j].transform.position.x == disappear_Cell.transform.position.x)
            {
                Debug.Log("1");
                // ������ �� ��Ͽ� ���ٸ� �߰�
                if (!target_Positions.ContainsKey(enable_Cells[j]))
                {
                    Debug.Log("����");
                    target_Positions.Add(enable_Cells[j], new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - 0.5f * empty_Positions.Count));
                }
                // ������ �� ��Ͽ� �ִٸ� ����
                else
                {
                    target_Positions[enable_Cells[j]] = new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - 0.5f * empty_Positions.Count);
                }
            }
        }
    }

    // �� �Ʒ��� �̵�
    void FallingCell()
    {
        for (int i = 0; i < empty_Positions.Count; i++)
        {
            int check_Cell = 0;
            for (int j = 0; j < target_Positions.Count; j++)
            {
                GameObject obj = target_Positions.Keys.ToList()[j];
                Debug.Log(empty_Positions[i].x == obj.transform.position.x && empty_Positions[i].y < obj.transform.position.y);
                if (empty_Positions[i].x == obj.transform.position.x && empty_Positions[i].y < obj.transform.position.y)
                {
                    obj.transform.position = Vector2.MoveTowards(obj.transform.position, empty_Positions[i], 2.5f * Time.deltaTime);

                    if (obj.transform.position == target_Positions.Values.ToList()[j])
                    {
                        Debug.Log("��ǥ ����");
                        target_Positions.Remove(obj);
                        j--;
                    }
                }
                else
                {
                    check_Cell++;
                }
            }

            if (check_Cell >= target_Positions.Count)
            {
                empty_Positions.Remove(empty_Positions[i]);
                i--;
            }
        }
    }
}
