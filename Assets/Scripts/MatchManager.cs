using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;       // 싱글톤

    GameObject change_Cell;     // 체인지할 셀
    GameObject other_Cell;      // 체인지될 셀
    bool isChange = false;      // 오브젝트 위치 변경 여부
    bool isRechange = false;    // 오브젝트 위치 원상태로 변경 여부
    Vector3 hit_Position;       // 터치된 셀의 위치
    Vector3 other_Position;     // 바뀔 셀의 위치
    Camera main_Camera;         // 메인 카메라

    // 매치인지 확인할 방향
    enum MatchDirection
    {
        LEFT = 0,
        RIGHT,
        UP,
        DOWN,
        LEFTLEFT,
        RIGHTRIGHT,
        UPUP,
        DOWNDOWN
    };

    // 매치가 가능한지 확인할 방향
    enum CanMatchDirection
    {
        LEFT = 0,
        RIGHT,
        UP,
        DOWN,
        LEFTUP,
        LEFTDOWN,
        RIGHTUP,
        RIGHTDOWN,
        LEFTLEFT,
        RIGHTRIGHT,
        UPUP,
        DOWNDOWN
    };

    void Awake()
    {
        instance = this;

        main_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();        // 레이하기 위해 가져옴
    }

    void Update()
    {
        // 클릭, 보드가 상호작용 중일 때는 클릭 제한
        if (Input.GetMouseButtonDown(0) && !isChange && Board.instance.empty_X.Count == 0)
        {
            change_Cell = Ray(Input.mousePosition);
        }

        // 클릭 뗌
        if (Input.GetMouseButtonUp(0)&& change_Cell != null && !isChange)
        {
            other_Cell = GetChangeDirectionCell();
            isChange = false;

            if (other_Cell != null)
            {
                isChange = true;
                hit_Position = new Vector3(change_Cell.transform.position.x, change_Cell.transform.position.y);
                other_Position = new Vector3(other_Cell.transform.position.x, other_Cell.transform.position.y);
            }
        }
    }

    void FixedUpdate()
    {
        // 셀 체인지 여부
        if (isChange)
        {
            // 다시 원상태로 체인지
            if (isRechange)
            {
                ChangeCell(other_Cell, other_Position, change_Cell, hit_Position);

                // 원상태로 체인지 완료, 초기화
                if (change_Cell.transform.position == hit_Position && other_Cell.transform.position == other_Position)
                {
                    isRechange = false;
                    ResetChangeCell();
                }

            }
            // 셀 체인지
            else
            {
                ChangeCell(change_Cell, other_Position, other_Cell, hit_Position);

                // 체인지 완료
                if (change_Cell.transform.position == other_Position && other_Cell.transform.position == hit_Position)
                {
                    // 매치되지 않았다면 다시 원상태로 체인지
                    if (!MatchCheck(change_Cell) && !MatchCheck(other_Cell))
                    {
                        isRechange = true;
                    }
                    // 매치됐다면 초기화
                    else
                    {
                        ResetChangeCell();
                    }
                }
            }
        }
    }

    // 셀 체인지 정보 초기화
    void ResetChangeCell()
    {
        change_Cell = null;
        other_Cell = null;
        hit_Position = new Vector3(0, 0, 0);
        other_Position = new Vector3(0, 0, 0);
        isChange = false;
    }

    // 오브젝트 감지
    GameObject Ray(Vector3 ray_Position)
    {
        RaycastHit hit;             // 터치된 셀을 담을 변수
        Ray ray;                    // 터치 좌표를 담는 변수

        ray = Camera.main.ScreenPointToRay(ray_Position);

        // 알고싶은 위치에 ray를 보냄
        Physics.Raycast(ray, out hit);

        // ray가 오브젝트에 부딪힐 경우
        if (hit.collider != null)
        {
            return hit.collider.gameObject.transform.parent.gameObject;     // 부모인 셀 자체를 가져와야 함
        }

        Debug.Log("감지된 오브젝트 없음");
        return null;
    }

    // 체인지할 셀
    GameObject GetChangeDirectionCell()
    {
        GameObject other_Cell;
        Vector3 mouse_Point = main_Camera.ScreenToWorldPoint(Input.mousePosition);       // 마우스 위치가 글로벌 위치이므로 로컬위치로 변경하여 반환

        // 오른쪽
        if (mouse_Point.x >= change_Cell.transform.position.x)
        {
            // 위
            if (mouse_Point.y >= change_Cell.transform.position.y)
            {
                // 오른쪽
                if (Math.Abs(mouse_Point.x - change_Cell.transform.position.x) >= Math.Abs(mouse_Point.y - change_Cell.transform.position.y))
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x + 0.5f, change_Cell.transform.position.y)));
                }
                // 위
                else
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x, change_Cell.transform.position.y + 0.5f)));
                }
            }
            // 아래
            else
            {
                // 오른쪽
                if (Math.Abs(mouse_Point.x - change_Cell.transform.position.x) >= Math.Abs(change_Cell.transform.position.y - mouse_Point.y))
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x + 0.5f, change_Cell.transform.position.y)));
                }
                // 아래
                else
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x, change_Cell.transform.position.y - 0.5f)));
                }
            }
        }
        // 왼쪽
        else
        {
            // 위
            if (mouse_Point.y >= change_Cell.transform.position.y)
            {
                // 왼쪽
                if (Math.Abs(change_Cell.transform.position.x - mouse_Point.x) >= Math.Abs(mouse_Point.y - change_Cell.transform.position.y))
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x - 0.5f, change_Cell.transform.position.y)));
                }
                // 위
                else
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x, change_Cell.transform.position.y + 0.5f)));
                }
            }
            // 아래
            else
            {
                // 왼쪽
                if (Math.Abs(mouse_Point.x - change_Cell.transform.position.x) >= Math.Abs(change_Cell.transform.position.y - mouse_Point.y))
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x - 0.5f, change_Cell.transform.position.y)));
                }
                // 아래
                else
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x, change_Cell.transform.position.y - 0.5f)));
                }
            }
        }
        return other_Cell;
    }

    // 셀 위치 변경
    void ChangeCell(GameObject own_Cell, Vector3 other_Position, GameObject other_Cell, Vector3 own_Position)
    {
        own_Cell.transform.position = Vector3.MoveTowards(own_Cell.transform.position, other_Position, 2.5f * Time.deltaTime);
        other_Cell.transform.position = Vector3.MoveTowards(other_Cell.transform.position, own_Position, 2.5f * Time.deltaTime);
    }

    // 매치 가능한지 확인
    public bool CanMatchCheck(GameObject own_Cell)
    {
        Dictionary<string, GameObject> match_Cell_Dic = new Dictionary<string, GameObject>();

        GameObject own_Fruit = own_Cell.transform.GetChild(0).gameObject;
        Sprite own_Sprite = own_Fruit.GetComponent<SpriteRenderer>().sprite;
        List<GameObject> direction_Cell = new List<GameObject>()
        {
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 0.5f, own_Cell.transform.position.y))),              // 왼쪽
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 0.5f, own_Cell.transform.position.y))),              // 오른쪽
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y + 0.5f))),              // 위
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y - 0.5f))),              // 아래
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 0.5f, own_Cell.transform.position.y + 0.5f))),       // 왼쪽 위
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 0.5f, own_Cell.transform.position.y - 0.5f))),       // 왼쪽 아래
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 0.5f, own_Cell.transform.position.y + 0.5f))),       // 오른쪽 위
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 0.5f, own_Cell.transform.position.y - 0.5f))),       // 오른쪽 아래
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 1.0f, own_Cell.transform.position.y))),              // 왼쪽왼쪽
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 1.0f, own_Cell.transform.position.y))),              // 오른쪽오른쪽
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y + 1.0f))),              // 위위
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y - 1.0f)))               // 아래아래
        };
        List<GameObject> direction_Fruit = new List<GameObject>();
        List<Sprite> direction_Sprite = new List<Sprite>();


        for (int i = 0; i < Enum.GetValues(typeof(CanMatchDirection)).Length; i++)
        {
            // 셀이 없는지 확인
            if (direction_Cell[i] != null)
            {
                direction_Fruit.Add(direction_Cell[i].transform.GetChild(0).gameObject);
            }
            else
            {
                direction_Fruit.Add(null);
            }

            // 이미지가 없는지 확인
            if (direction_Fruit[i] != null)
            {
                direction_Sprite.Add(direction_Fruit[i].GetComponent<SpriteRenderer>().sprite);
            }
            else
            {
                direction_Sprite.Add(null);
            }
        }

        // 일치하는 과일 찾기
        int match_Count = 0;
        for (int i = 0; i < direction_Sprite.Count; i++)
        {
            if (direction_Sprite[i] != null && direction_Cell[i].transform.position != own_Cell.transform.position)
            {
                if (own_Sprite == direction_Sprite[i])
                {
                    match_Count++;
                    match_Cell_Dic.Add(Enum.GetName(typeof(CanMatchDirection), i), direction_Cell[i]);
                }
            }
        }

        bool isCanMatch = false;

        // 매치 확인
        if (match_Count >= 2)
        {
            if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFTUP.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHTUP.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFTDOWN.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHTDOWN.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFTUP.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.LEFTDOWN.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHTUP.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHTDOWN.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFT.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHTUP.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFT.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHTDOWN.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFTDOWN.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHT.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFTUP.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHT.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFTDOWN.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.UP.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHTDOWN.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.UP.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFTUP.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.DOWN.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHTUP.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.DOWN.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.UP.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.DOWNDOWN.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.UPUP.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.DOWN.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFTLEFT.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHT.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }

            else if (match_Cell_Dic.ContainsKey(CanMatchDirection.LEFT.ToString()) && match_Cell_Dic.ContainsKey(CanMatchDirection.RIGHTRIGHT.ToString()))
            {
                isCanMatch = true;
                return isCanMatch;
            }
        }

        return isCanMatch;
    }

    // 매치 확인
    public bool MatchCheck(GameObject own_Cell, bool isPlayer = true)
    {
        Dictionary<string, GameObject> match_Cell_Dic = new Dictionary<string, GameObject>();

        GameObject own_Fruit = own_Cell.transform.GetChild(0).gameObject;
        Sprite own_Sprite = own_Fruit.GetComponent<SpriteRenderer>().sprite;
        List<GameObject> direction_Cell = new List<GameObject>()
        {
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 0.5f, own_Cell.transform.position.y))),      // 왼쪽
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 0.5f, own_Cell.transform.position.y))),      // 오른쪽
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y + 0.5f))),      // 위
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y - 0.5f))),      // 아래
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 1.0f, own_Cell.transform.position.y))),      // 왼쪽왼쪽
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 1.0f, own_Cell.transform.position.y))),      // 오른쪽오른쪽
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y + 1.0f))),      // 위위
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y - 1.0f)))       // 아래아래
        };
        List<GameObject> direction_Fruit = new List<GameObject>();
        List<Sprite> direction_Sprite = new List<Sprite>();

        for (int i = 0; i < Enum.GetValues(typeof(MatchDirection)).Length; i++)
        {
            // 셀이 없는지 확인
            if (direction_Cell[i] != null)
            {
                direction_Fruit.Add(direction_Cell[i].transform.GetChild(0).gameObject);
            }
            else
            {
                direction_Fruit.Add(null);
            }

            // 이미지가 없는지 확인
            if (direction_Fruit[i] != null)
            {
                direction_Sprite.Add(direction_Fruit[i].GetComponent<SpriteRenderer>().sprite);
            }
            else
            {
                direction_Sprite.Add(null);
            }
        }

        // 일치하는 과일 찾기
        int match_Count = 0;
        for (int i = 0; i < direction_Sprite.Count; i++)
        {
            if (direction_Sprite[i] != null && direction_Cell[i].transform.position != own_Cell.transform.position)
            {
                if (own_Sprite == direction_Sprite[i])
                {
                    match_Count++;
                    match_Cell_Dic.Add(Enum.GetName(typeof(MatchDirection), i), direction_Cell[i]);
                }
            }
        }

        bool isSuccess = false;

        Debug.Log($"같은 과일 {match_Count}개");

        // 매치 확인
        if (match_Count >= 2)
        {
            // 플레이어 시작 전 매치된 경우, 이미지만 바꿈
            if (!isPlayer)
            {
                Debug.Log("이미지 바꿈");
                own_Fruit.SetActive(false);
                own_Fruit.SetActive(true);

                isSuccess = true;
            }
            // 플레이어 시작 후 매치된 경우
            else
            {
                isSuccess = ThreeMatch(own_Cell, match_Cell_Dic);
            }
        }

        return isSuccess;
    }

    // 3매치 확인, 4매치 5매치는 이상하게 되서 else if 로 바꿨음
    bool ThreeMatch(GameObject center_Cell, Dictionary<string, GameObject> match_Cell_Dic)
    {
        bool isSuccess = false;

        // 왼쪽 오른쪽
        if (match_Cell_Dic.ContainsKey(MatchDirection.LEFT.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.RIGHT.ToString()))
        {
            SuccessMatch(match_Cell_Dic[MatchDirection.LEFT.ToString()]);
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.RIGHT.ToString()]);
            isSuccess = true;
        }

        // 위 아래
        else if (match_Cell_Dic.ContainsKey(MatchDirection.UP.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.DOWN.ToString()))
        {
            SuccessMatch(match_Cell_Dic[MatchDirection.UP.ToString()]);
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.DOWN.ToString()]);
            isSuccess = true;
        }

        // 왼쪽 왼왼쪽
        else if (match_Cell_Dic.ContainsKey(MatchDirection.LEFT.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.LEFTLEFT.ToString()))
        {
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.LEFT.ToString()]);
            SuccessMatch(match_Cell_Dic[MatchDirection.LEFTLEFT.ToString()]);
            isSuccess = true;
        }

        // 오른쪽 오른오른쪽
        else if (match_Cell_Dic.ContainsKey(MatchDirection.RIGHT.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.RIGHTRIGHT.ToString()))
        {
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.RIGHT.ToString()]);
            SuccessMatch(match_Cell_Dic[MatchDirection.RIGHTRIGHT.ToString()]);
            isSuccess = true;
        }

        // 위 위위
        else if (match_Cell_Dic.ContainsKey(MatchDirection.UP.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.UPUP.ToString()))
        {
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.UP.ToString()]);
            SuccessMatch(match_Cell_Dic[MatchDirection.UPUP.ToString()]);
            isSuccess = true;
        }

        // 아래 아래아래
        else if (match_Cell_Dic.ContainsKey(MatchDirection.DOWN.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.DOWNDOWN.ToString()))
        {
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.DOWN.ToString()]);
            SuccessMatch(match_Cell_Dic[MatchDirection.DOWNDOWN.ToString()]);

            isSuccess = true;
        }

        return isSuccess;
    }

    // 매치 성공
    void SuccessMatch(GameObject match_Cell)
    {
        Board.instance.FillEmptyBoard(match_Cell.transform.position);
        match_Cell.SetActive(false);
    }
}