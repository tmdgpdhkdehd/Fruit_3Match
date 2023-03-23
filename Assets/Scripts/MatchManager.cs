using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;       // �̱���

    GameObject change_Cell;     // ü������ ��
    GameObject other_Cell;      // ü������ ��
    bool isChange = false;      // ������Ʈ ��ġ ���� ����
    bool isRechange = false;    // ������Ʈ ��ġ �����·� ���� ����
    Vector3 hit_Position;       // ��ġ�� ���� ��ġ
    Vector3 other_Position;     // �ٲ� ���� ��ġ
    Camera main_Camera;         // ���� ī�޶�

    // ��ġ���� Ȯ���� ����
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

    // ��ġ�� �������� Ȯ���� ����
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

        main_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();        // �����ϱ� ���� ������
    }

    void Update()
    {
        // Ŭ��, ���尡 ��ȣ�ۿ� ���� ���� Ŭ�� ����
        if (Input.GetMouseButtonDown(0) && !isChange && Board.instance.empty_X.Count == 0)
        {
            change_Cell = Ray(Input.mousePosition);
        }

        // Ŭ�� ��
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
        // �� ü���� ����
        if (isChange)
        {
            // �ٽ� �����·� ü����
            if (isRechange)
            {
                ChangeCell(other_Cell, other_Position, change_Cell, hit_Position);

                // �����·� ü���� �Ϸ�, �ʱ�ȭ
                if (change_Cell.transform.position == hit_Position && other_Cell.transform.position == other_Position)
                {
                    isRechange = false;
                    ResetChangeCell();
                }

            }
            // �� ü����
            else
            {
                ChangeCell(change_Cell, other_Position, other_Cell, hit_Position);

                // ü���� �Ϸ�
                if (change_Cell.transform.position == other_Position && other_Cell.transform.position == hit_Position)
                {
                    // ��ġ���� �ʾҴٸ� �ٽ� �����·� ü����
                    if (!MatchCheck(change_Cell) && !MatchCheck(other_Cell))
                    {
                        isRechange = true;
                    }
                    // ��ġ�ƴٸ� �ʱ�ȭ
                    else
                    {
                        ResetChangeCell();
                    }
                }
            }
        }
    }

    // �� ü���� ���� �ʱ�ȭ
    void ResetChangeCell()
    {
        change_Cell = null;
        other_Cell = null;
        hit_Position = new Vector3(0, 0, 0);
        other_Position = new Vector3(0, 0, 0);
        isChange = false;
    }

    // ������Ʈ ����
    GameObject Ray(Vector3 ray_Position)
    {
        RaycastHit hit;             // ��ġ�� ���� ���� ����
        Ray ray;                    // ��ġ ��ǥ�� ��� ����

        ray = Camera.main.ScreenPointToRay(ray_Position);

        // �˰���� ��ġ�� ray�� ����
        Physics.Raycast(ray, out hit);

        // ray�� ������Ʈ�� �ε��� ���
        if (hit.collider != null)
        {
            return hit.collider.gameObject.transform.parent.gameObject;     // �θ��� �� ��ü�� �����;� ��
        }

        Debug.Log("������ ������Ʈ ����");
        return null;
    }

    // ü������ ��
    GameObject GetChangeDirectionCell()
    {
        GameObject other_Cell;
        Vector3 mouse_Point = main_Camera.ScreenToWorldPoint(Input.mousePosition);       // ���콺 ��ġ�� �۷ι� ��ġ�̹Ƿ� ������ġ�� �����Ͽ� ��ȯ

        // ������
        if (mouse_Point.x >= change_Cell.transform.position.x)
        {
            // ��
            if (mouse_Point.y >= change_Cell.transform.position.y)
            {
                // ������
                if (Math.Abs(mouse_Point.x - change_Cell.transform.position.x) >= Math.Abs(mouse_Point.y - change_Cell.transform.position.y))
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x + 0.5f, change_Cell.transform.position.y)));
                }
                // ��
                else
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x, change_Cell.transform.position.y + 0.5f)));
                }
            }
            // �Ʒ�
            else
            {
                // ������
                if (Math.Abs(mouse_Point.x - change_Cell.transform.position.x) >= Math.Abs(change_Cell.transform.position.y - mouse_Point.y))
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x + 0.5f, change_Cell.transform.position.y)));
                }
                // �Ʒ�
                else
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x, change_Cell.transform.position.y - 0.5f)));
                }
            }
        }
        // ����
        else
        {
            // ��
            if (mouse_Point.y >= change_Cell.transform.position.y)
            {
                // ����
                if (Math.Abs(change_Cell.transform.position.x - mouse_Point.x) >= Math.Abs(mouse_Point.y - change_Cell.transform.position.y))
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x - 0.5f, change_Cell.transform.position.y)));
                }
                // ��
                else
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x, change_Cell.transform.position.y + 0.5f)));
                }
            }
            // �Ʒ�
            else
            {
                // ����
                if (Math.Abs(mouse_Point.x - change_Cell.transform.position.x) >= Math.Abs(change_Cell.transform.position.y - mouse_Point.y))
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x - 0.5f, change_Cell.transform.position.y)));
                }
                // �Ʒ�
                else
                {
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector3(change_Cell.transform.position.x, change_Cell.transform.position.y - 0.5f)));
                }
            }
        }
        return other_Cell;
    }

    // �� ��ġ ����
    void ChangeCell(GameObject own_Cell, Vector3 other_Position, GameObject other_Cell, Vector3 own_Position)
    {
        own_Cell.transform.position = Vector3.MoveTowards(own_Cell.transform.position, other_Position, 2.5f * Time.deltaTime);
        other_Cell.transform.position = Vector3.MoveTowards(other_Cell.transform.position, own_Position, 2.5f * Time.deltaTime);
    }

    // ��ġ �������� Ȯ��
    public bool CanMatchCheck(GameObject own_Cell)
    {
        Dictionary<string, GameObject> match_Cell_Dic = new Dictionary<string, GameObject>();

        GameObject own_Fruit = own_Cell.transform.GetChild(0).gameObject;
        Sprite own_Sprite = own_Fruit.GetComponent<SpriteRenderer>().sprite;
        List<GameObject> direction_Cell = new List<GameObject>()
        {
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 0.5f, own_Cell.transform.position.y))),              // ����
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 0.5f, own_Cell.transform.position.y))),              // ������
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y + 0.5f))),              // ��
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y - 0.5f))),              // �Ʒ�
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 0.5f, own_Cell.transform.position.y + 0.5f))),       // ���� ��
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 0.5f, own_Cell.transform.position.y - 0.5f))),       // ���� �Ʒ�
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 0.5f, own_Cell.transform.position.y + 0.5f))),       // ������ ��
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 0.5f, own_Cell.transform.position.y - 0.5f))),       // ������ �Ʒ�
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 1.0f, own_Cell.transform.position.y))),              // ���ʿ���
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 1.0f, own_Cell.transform.position.y))),              // �����ʿ�����
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y + 1.0f))),              // ����
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y - 1.0f)))               // �Ʒ��Ʒ�
        };
        List<GameObject> direction_Fruit = new List<GameObject>();
        List<Sprite> direction_Sprite = new List<Sprite>();


        for (int i = 0; i < Enum.GetValues(typeof(CanMatchDirection)).Length; i++)
        {
            // ���� ������ Ȯ��
            if (direction_Cell[i] != null)
            {
                direction_Fruit.Add(direction_Cell[i].transform.GetChild(0).gameObject);
            }
            else
            {
                direction_Fruit.Add(null);
            }

            // �̹����� ������ Ȯ��
            if (direction_Fruit[i] != null)
            {
                direction_Sprite.Add(direction_Fruit[i].GetComponent<SpriteRenderer>().sprite);
            }
            else
            {
                direction_Sprite.Add(null);
            }
        }

        // ��ġ�ϴ� ���� ã��
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

        // ��ġ Ȯ��
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

    // ��ġ Ȯ��
    public bool MatchCheck(GameObject own_Cell, bool isPlayer = true)
    {
        Dictionary<string, GameObject> match_Cell_Dic = new Dictionary<string, GameObject>();

        GameObject own_Fruit = own_Cell.transform.GetChild(0).gameObject;
        Sprite own_Sprite = own_Fruit.GetComponent<SpriteRenderer>().sprite;
        List<GameObject> direction_Cell = new List<GameObject>()
        {
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 0.5f, own_Cell.transform.position.y))),      // ����
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 0.5f, own_Cell.transform.position.y))),      // ������
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y + 0.5f))),      // ��
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y - 0.5f))),      // �Ʒ�
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x - 1.0f, own_Cell.transform.position.y))),      // ���ʿ���
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x + 1.0f, own_Cell.transform.position.y))),      // �����ʿ�����
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y + 1.0f))),      // ����
            Ray(main_Camera.WorldToScreenPoint(new Vector3(own_Cell.transform.position.x, own_Cell.transform.position.y - 1.0f)))       // �Ʒ��Ʒ�
        };
        List<GameObject> direction_Fruit = new List<GameObject>();
        List<Sprite> direction_Sprite = new List<Sprite>();

        for (int i = 0; i < Enum.GetValues(typeof(MatchDirection)).Length; i++)
        {
            // ���� ������ Ȯ��
            if (direction_Cell[i] != null)
            {
                direction_Fruit.Add(direction_Cell[i].transform.GetChild(0).gameObject);
            }
            else
            {
                direction_Fruit.Add(null);
            }

            // �̹����� ������ Ȯ��
            if (direction_Fruit[i] != null)
            {
                direction_Sprite.Add(direction_Fruit[i].GetComponent<SpriteRenderer>().sprite);
            }
            else
            {
                direction_Sprite.Add(null);
            }
        }

        // ��ġ�ϴ� ���� ã��
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

        Debug.Log($"���� ���� {match_Count}��");

        // ��ġ Ȯ��
        if (match_Count >= 2)
        {
            // �÷��̾� ���� �� ��ġ�� ���, �̹����� �ٲ�
            if (!isPlayer)
            {
                Debug.Log("�̹��� �ٲ�");
                own_Fruit.SetActive(false);
                own_Fruit.SetActive(true);

                isSuccess = true;
            }
            // �÷��̾� ���� �� ��ġ�� ���
            else
            {
                isSuccess = ThreeMatch(own_Cell, match_Cell_Dic);
            }
        }

        return isSuccess;
    }

    // 3��ġ Ȯ��, 4��ġ 5��ġ�� �̻��ϰ� �Ǽ� else if �� �ٲ���
    bool ThreeMatch(GameObject center_Cell, Dictionary<string, GameObject> match_Cell_Dic)
    {
        bool isSuccess = false;

        // ���� ������
        if (match_Cell_Dic.ContainsKey(MatchDirection.LEFT.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.RIGHT.ToString()))
        {
            SuccessMatch(match_Cell_Dic[MatchDirection.LEFT.ToString()]);
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.RIGHT.ToString()]);
            isSuccess = true;
        }

        // �� �Ʒ�
        else if (match_Cell_Dic.ContainsKey(MatchDirection.UP.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.DOWN.ToString()))
        {
            SuccessMatch(match_Cell_Dic[MatchDirection.UP.ToString()]);
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.DOWN.ToString()]);
            isSuccess = true;
        }

        // ���� �޿���
        else if (match_Cell_Dic.ContainsKey(MatchDirection.LEFT.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.LEFTLEFT.ToString()))
        {
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.LEFT.ToString()]);
            SuccessMatch(match_Cell_Dic[MatchDirection.LEFTLEFT.ToString()]);
            isSuccess = true;
        }

        // ������ ����������
        else if (match_Cell_Dic.ContainsKey(MatchDirection.RIGHT.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.RIGHTRIGHT.ToString()))
        {
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.RIGHT.ToString()]);
            SuccessMatch(match_Cell_Dic[MatchDirection.RIGHTRIGHT.ToString()]);
            isSuccess = true;
        }

        // �� ����
        else if (match_Cell_Dic.ContainsKey(MatchDirection.UP.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.UPUP.ToString()))
        {
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.UP.ToString()]);
            SuccessMatch(match_Cell_Dic[MatchDirection.UPUP.ToString()]);
            isSuccess = true;
        }

        // �Ʒ� �Ʒ��Ʒ�
        else if (match_Cell_Dic.ContainsKey(MatchDirection.DOWN.ToString()) && match_Cell_Dic.ContainsKey(MatchDirection.DOWNDOWN.ToString()))
        {
            SuccessMatch(center_Cell);
            SuccessMatch(match_Cell_Dic[MatchDirection.DOWN.ToString()]);
            SuccessMatch(match_Cell_Dic[MatchDirection.DOWNDOWN.ToString()]);

            isSuccess = true;
        }

        return isSuccess;
    }

    // ��ġ ����
    void SuccessMatch(GameObject match_Cell)
    {
        Board.instance.FillEmptyBoard(match_Cell.transform.position);
        match_Cell.SetActive(false);
    }
}