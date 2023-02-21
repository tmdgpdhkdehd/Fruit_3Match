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

    public Vector3 empty_Position;                                      // 빈 공간 위치
    public List<GameObject> down_Cells = new List<GameObject>();        // 빈 공간에 내려오고 있는 셀

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
            // 사용 중인 셀이 있으면 한 칸씩 내려옴
            for (int i = 0; i < enable_Cells.Count; i++)
            {
                if (enable_Cells[i].transform.position.y > empty_Position.y && enable_Cells[i].transform.position.x == empty_Position.x)
                {
                    enable_Cells[i].transform.position = Vector2.MoveTowards(enable_Cells[i].transform.position, new Vector2(empty_Position.x, cell_CreateY), -2.5f * Time.deltaTime);

                    // 셀이 다 내려왔다면 끝내기
                    if (enable_Cells[i].transform.position.y == empty_Position.y && enable_Cells[i].transform.position.x == empty_Position.x)
                    {
                        isEmpty = false;
                        break;
                    }
                }
            }
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

    
    // 빈 공간 채우기
    public void FillEmptyBoard(GameObject disappear_Cell)
    {
        isEmpty = true;
        empty_Position = disappear_Cell.transform.position;
        /*
        // 사용 중인 셀이 있으면 한 칸씩 내려옴
        for (int i = 0; i < enable_Cells.Count; i++)
        {
            if (enable_Cells[i].transform.position.y > disappear_Cell.transform.position.y && enable_Cells[i].transform.position.x == disappear_Cell.transform.position.x)
            {
                enable_Cells[i].transform.position = new Vector2(enable_Cells[i].transform.position.x, enable_Cells[i].transform.position.y - 0.5f);
            }
        }

        // 미사용중인 셀이 있으면 맨 윗부분을 채움
        if (disable_Cells.Count != 0)
        {
            GameObject show_Cell = disable_Cells[0];
            show_Cell.SetActive(true);
            show_Cell.transform.position = new Vector2(disappear_Cell.transform.position.x, cell_CreateY);
        }
        // 미사용중인 셀이 없으면 셀을 생성해서 맨 윗부분을 채움
        else
        {
            GameObject instantiate_Cell = Instantiate(cell);
            instantiate_Cell.transform.position = new Vector2(disappear_Cell.transform.position.x, cell_CreateY);
        }
        */
        disappear_Cell.SetActive(false);
    }

    // 셀 아래로 이동
    void FillEmptyCell(GameObject own_Cell, Vector3 empty_Position)
    {
        own_Cell.transform.position = Vector2.MoveTowards(own_Cell.transform.position, new Vector2(empty_Position.x, cell_CreateY), 2.5f * Time.deltaTime);
    }
}
