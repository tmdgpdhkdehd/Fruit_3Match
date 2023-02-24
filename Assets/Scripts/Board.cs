using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;       // 싱글톤

    public GameObject cell;         // 셀
    public float cell_CreateX;      // 셀 처음 X축 위치
    public float cell_CreateY;      // 셀 처음 Y축 위치
    public float cell_NextX;        // 만들어질 셀 X축 위치
    public float cell_NextY;        // 만들어질 셀 Y축 위치
    public int board_X;             // 보드 x축 길이
    public int board_Y;             // 보드 y축 길이

    public List<GameObject> enable_Cells = new List<GameObject>();      // 사용중인 셀
    public List<GameObject> disable_Cells = new List<GameObject>();     // 미사용중인 셀

    public List<Vector3> empty_Positions = new List<Vector3>();         // 빈 공간 위치
    public List<GameObject> down_Cells = new List<GameObject>();        // 빈 공간에 내려오고 있는 셀

    Dictionary<GameObject, Vector3> target_Positions = new Dictionary<GameObject, Vector3>();       // 내려올 셀들의 목표 위치

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
        // 비어있는 공간이 있다면
        if (empty_Positions.Count != 0)
        {
            Debug.Log("비어있는 공간 있음");
            FallingCell();
        }
    }

    // 보드 생성
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

    
    // 빈 공간 채우기 (추후 이름 변경 필요, 코드도 2개로 나눠야할 듯)
    public void FillEmptyBoard(GameObject disappear_Cell)
    {
        // 미사용중인 셀이 있으면 맨 윗부분을 채움
        if (disable_Cells.Count != 0)
        {
            GameObject show_Cell = disable_Cells[0];
            show_Cell.SetActive(true);
            show_Cell.transform.position = new Vector2(disappear_Cell.transform.position.x, cell_CreateY + 0.5f * empty_Positions.Count);
        }
        // 미사용중인 셀이 없으면 셀을 생성해서 맨 윗부분을 채움
        else
        {
            GameObject instantiate_Cell = Instantiate(cell);
            instantiate_Cell.transform.position = new Vector2(disappear_Cell.transform.position.x, cell_CreateY + 0.5f * empty_Positions.Count);
        }
        
        
        disappear_Cell.SetActive(false);
        empty_Positions.Add(disappear_Cell.transform.position);

        // -------------------------------------------------------------------------------
        /*
        // 없어진 셀 위에 위치한 셀들만 자신 위치의 바로 아래 셀로 위치를 잡고 딕셔너리에 저장
        for (int i = 0; i < empty_Positions.Count; i++)
        {
            for (int j = 0; j < enable_Cells.Count; j++)
            {
                if (enable_Cells[j].transform.position.y > empty_Positions[i].y && enable_Cells[j].transform.position.x == empty_Positions[i].x)
                {
                    Debug.Log("1");
                    // 내려올 셀 목록에 없다면 추가
                    if (!target_Positions.ContainsKey(enable_Cells[j]))
                    {
                        Debug.Log("생성");
                        target_Positions.Add(enable_Cells[j], new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - 0.5f * empty_Positions.Count));
                    }
                    // 내려올 셀 목록에 있다면 변경
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

    // 없어진 셀 위에 위치한 셀들만 자신 위치의 바로 아래 셀로 위치를 잡고 딕셔너리에 저장
    void SetFallingCell(GameObject disappear_Cell)
    {
        for (int j = 0; j < enable_Cells.Count; j++)
        {
            if (enable_Cells[j].transform.position.y > disappear_Cell.transform.position.y && enable_Cells[j].transform.position.x == disappear_Cell.transform.position.x)
            {
                Debug.Log("1");
                // 내려올 셀 목록에 없다면 추가
                if (!target_Positions.ContainsKey(enable_Cells[j]))
                {
                    Debug.Log("생성");
                    target_Positions.Add(enable_Cells[j], new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - 0.5f * empty_Positions.Count));
                }
                // 내려올 셀 목록에 있다면 변경
                else
                {
                    target_Positions[enable_Cells[j]] = new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - 0.5f * empty_Positions.Count);
                }
            }
        }
    }

    // 셀 아래로 이동
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
                        Debug.Log("목표 도달");
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
