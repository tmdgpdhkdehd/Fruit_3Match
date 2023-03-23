using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;       // 싱글톤

    public GameObject board;               // 보드
    public GameObject cell;         // 셀
    public float cell_CreateX;      // 셀 처음 X축 위치
    public float cell_CreateY;      // 셀 처음 Y축 위치
    public float cell_NextX;        // 만들어질 셀 X축 위치
    public float cell_NextY;        // 만들어질 셀 Y축 위치
    public int board_X;             // 보드 x축 길이
    public int board_Y;             // 보드 y축 길이

    public List<GameObject> enable_Cells = new List<GameObject>();      // 사용중인 셀
    public List<GameObject> disable_Cells = new List<GameObject>();     // 미사용중인 셀

    public Dictionary<decimal, int> empty_X = new Dictionary<decimal, int>();           // 빈 공간 x축 위치의 개수, 부동소수점(float) 오차 때문에 비교 버그나서 decimal 사용
    public List<GameObject> down_Cells = new List<GameObject>();                        // 빈 공간에 내려오고 있는 셀

    Dictionary<GameObject, Vector3> target_Positions = new Dictionary<GameObject, Vector3>();       // 내려올 셀들의 목표 위치

    void Awake()
    {
        instance = this;

        board = GameObject.Find("Board");

        cell_NextX = cell_CreateX;
        cell_NextY = cell_CreateY;
    }

    private void Reset()
    {
        cell_NextX = cell_CreateX;
        cell_NextY = cell_CreateY;
    }

    private void Start()
    {
        InstantiateCellBoard();
        Invoke("StartBoardMatch", 0.001f);
        Invoke("CanBoardMatchCheck", 0.001f);
    }

    void FixedUpdate()
    {
        // 비어있는 공간이 있다면
        if (empty_X.Count != 0)
        {
            FallingCell();
        }
    }

    // 보드 생성
    public void InstantiateCellBoard()
    {
        Debug.Log("보드 생성");
        for (int i = 0; i < board_Y; i++)
        {
            for (int j = 0; j < board_X; j++)
            {
                GameObject current_Cell = Instantiate(cell);
                current_Cell.transform.position = new Vector3(cell_NextX, cell_NextY);

                cell_NextX += 0.5f;
            }
            cell_NextX = cell_CreateX;
            cell_NextY -= 0.5f;
        }
    }
    
    // 보드 생성 직후 보드에 매치 있는지 확인
    public void StartBoardMatch()
    {
        Debug.Log("매치 있는지 확인");
        bool isMatch = false;
        for (int i = 0; i < enable_Cells.Count; i++)
        {
            if (MatchManager.instance.MatchCheck(enable_Cells[i], false))
            {
                isMatch = true;
            }
        }

        // 매치되서 바꿨을 경우, 다시 매치되었는지 확인
        if (isMatch)
        {
            StartBoardMatch();
        }
    }

    // 매치할 수 있는 보드인지 확인
    public void CanBoardMatchCheck()
    {
        Debug.Log("매치할 수 있는지 확인");
        bool isCanMatch = true;

        for (int i = 0; i < enable_Cells.Count; i++)
        {
            isCanMatch = MatchManager.instance.CanMatchCheck(enable_Cells[i]);
            
            // 보드에 매치되는 게 있으므로 더 확인할 필요 없음
            if (isCanMatch)
            {
                Debug.Log("매치되는 거 있음");
                return;
            }
        }

        // 보드에 매치되는 게 없으므로 보드 다시 생성
        if (!isCanMatch)
        {
            Debug.Log("매치되는 거 없음");
            Reset();
            for (int i = 0; i < enable_Cells.Count; i++)
            {
                Destroy(enable_Cells[i]);
            }
            for (int i = 0; i < disable_Cells.Count; i++)
            {
                Destroy(disable_Cells[i]);
            }
            InstantiateCellBoard();
            StartBoardMatch();
            CanBoardMatchCheck();
        }
    }

    // 빈 공간을 채울 셀 생성
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

        // 미사용중인 셀이 있으면 맨 윗부분을 채움
        if (disable_Cells.Count != 0)
        {
            GameObject show_Cell = disable_Cells[0];
            show_Cell.SetActive(true);
            show_Cell.transform.position = new Vector3((float)x, cell_CreateY + 0.5f * empty_X[x]);
        }
        // 미사용중인 셀이 없으면 셀을 생성해서 맨 윗부분을 채움
        else
        {
            GameObject instantiate_Cell = Instantiate(cell);
            instantiate_Cell.transform.position = new Vector3((float)x, cell_CreateY + 0.5f * empty_X[x]);
        }

        SetFallingCell(empty_Position);
    }

    // 빈 공간을 채울 셀의 목표 위치 설정
    void SetFallingCell(Vector3 empty_Position)
    {
        decimal x = (decimal)empty_Position.x;
        float tmp = 0.5f * empty_X[x];

        for (int j = 0; j < enable_Cells.Count; j++)
        {
            if (enable_Cells[j].transform.position.y > empty_Position.y && enable_Cells[j].transform.position.x == empty_Position.x)
            {
                Debug.Log(tmp);
                // 내려올 셀 목록에 없다면 추가
                if (!target_Positions.ContainsKey(enable_Cells[j]))
                {
                    target_Positions.Add(enable_Cells[j], new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - tmp));
                }
                // 내려올 셀 목록에 있다면 변경
                else
                {
                    target_Positions[enable_Cells[j]] = new Vector3(enable_Cells[j].transform.position.x, enable_Cells[j].transform.position.y - tmp);
                }
            }
        }
    }

    // 셀 아래로 이동
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
                    obj.transform.position = Vector3.MoveTowards(obj.transform.position, target_Positions[obj], 2.5f * Time.deltaTime);

                    // 셀이 목표 위치에 도달
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

            // 같은 x축 셀들이 모두 목표 위치에 도달했다면 해당 x축을 빈 x축에서 제외 
            if (check_Cell >= target_Positions.Count)
            {
                empty_X.Remove(x);
            }
        }

        // 모든 셀이 내려왔을 경우
        if (empty_X.Count == 0)
        {
            for (int i = 0; i < enable_Cells.Count; i++)
            {
                MatchManager.instance.MatchCheck(enable_Cells[i]);
            }
        }
    }
}
