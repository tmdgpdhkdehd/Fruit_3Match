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
    public void FillEmptyPlace(GameObject disappear_Cell)
    {
        for (int i = 0; i < enable_Cells.Count; i++)
        {
            if (enable_Cells[i].transform.position.y > disappear_Cell.transform.position.y && enable_Cells[i].transform.position.x == disappear_Cell.transform.position.x)
            {
                enable_Cells[i].transform.position = new Vector2(enable_Cells[i].transform.position.x, enable_Cells[i].transform.position.y - 0.5f);
            }
        }

        if (disable_Cells.Count != 0)
        {
            GameObject show_Cell = disable_Cells[0];
            show_Cell.SetActive(true);
            show_Cell.transform.position = new Vector2(disappear_Cell.transform.position.x, cell_CreateY);
        }
        else
        {
            GameObject instantiate_Cell = Instantiate(cell);
            instantiate_Cell.transform.position = new Vector2(disappear_Cell.transform.position.x, cell_CreateY);
        }
        disappear_Cell.SetActive(false);
    }
}
