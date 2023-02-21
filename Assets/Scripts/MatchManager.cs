using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    GameObject hit_Cell;        // ��ġ�� ��
    GameObject other_Cell;      // �ٲ� ��
    bool isTouch = false;       // ������Ʈ ��ġ ����
    bool isChange = false;      // ������Ʈ ��ġ ���� ����
    bool isRechange = false;
    Vector3 hit_Position;       // ��ġ�� ���� ��ġ
    Vector3 other_Position;     // �ٲ� ���� ��ġ
    Camera main_Camera;         // ���� ī�޶�

    enum Direction
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

    void Awake()
    {
        main_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        // Ŭ��
        if (Input.GetMouseButtonDown(0) && !isTouch)
        {
            hit_Cell = Ray(Input.mousePosition);
            other_Cell = null;
            hit_Position = hit_Cell.transform.position;
            other_Position = hit_Cell.transform.position;
        }

        // Ŭ�� ��
        if (Input.GetMouseButtonUp(0) && isTouch)
        {
            other_Cell = GetChangeDirection();
            isChange = false;

            if (other_Cell != null)
            {
                isChange = true;
                hit_Position = new Vector2(hit_Cell.transform.position.x, hit_Cell.transform.position.y);
                other_Position = new Vector2(other_Cell.transform.position.x, other_Cell.transform.position.y);
            }
        }

        // �� ü����
        if (isChange)
        {
            ChangeCell(hit_Cell,other_Cell);

            /*
            if (hit_Cell.transform.position == other_Position && other_Cell.transform.position == hit_Position)
            {
                // �����·� ü�����Ǿ��ٸ� ü���� ������
                if (isRechange)
                {
                    isChange = false;
                    isTouch = false;
                }

                // ��ġ���� �ʾҴٸ� �ٽ� �����·� ü����
                if (!MatchType(hit_Cell))
                {
                    //ChangeCell(other_Cell, other_Position, hit_Cell, hit_Position);
                    isRechange = true;
                }
                else
                {
                    isChange = false;
                    isTouch = false;
                }
            }
            */
        }
    }

    GameObject Ray(Vector2 ray_Position)
    {
        RaycastHit hit;             // ��ġ�� ���� ���� ����
        Ray touchray;               // ��ġ ��ǥ�� ��� ����

        touchray = Camera.main.ScreenPointToRay(ray_Position);

        // ��ġ�� ���� ray�� ����
        Physics.Raycast(touchray, out hit);

        // ray�� ������Ʈ�� �ε��� ���
        if (hit.collider != null)
        {
            isTouch = true;
            return hit.collider.gameObject.transform.parent.gameObject;     // �θ��� �� ��ü�� �����;� ��
        }

        Debug.Log("������ ������Ʈ ����");
        return null;
    }

    GameObject GetChangeDirection()
    {
        string direction;       // �ٲ� �� ����
        GameObject other_Cell;
        Vector2 mouse_Point = main_Camera.ScreenToWorldPoint(Input.mousePosition);       // ���콺 ��ġ�� �۷ι� ��ġ�̹Ƿ� ������ġ�� �����Ͽ� ��ȯ

        // ������
        if (mouse_Point.x >= hit_Cell.transform.position.x)
        {
            // ��
            if (mouse_Point.y >= hit_Cell.transform.position.y)
            {
                if (Math.Abs(mouse_Point.x - hit_Cell.transform.position.x) >= Math.Abs(mouse_Point.y - hit_Cell.transform.position.y))
                {
                    direction = "������";
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x + 0.5f, hit_Cell.transform.position.y)));
                }
                else
                {
                    direction = "��";
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x, hit_Cell.transform.position.y + 0.5f)));
                }
            }
            // �Ʒ�
            else
            {
                if (Math.Abs(mouse_Point.x - hit_Cell.transform.position.x) >= Math.Abs(hit_Cell.transform.position.y - mouse_Point.y))
                {
                    direction = "������";
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x + 0.5f, hit_Cell.transform.position.y)));
                }
                else
                {
                    direction = "�Ʒ�";
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x, hit_Cell.transform.position.y - 0.5f)));
                }
            }
        }
        // ����
        else
        {
            // ��
            if (mouse_Point.y >= hit_Cell.transform.position.y)
            {
                if (Math.Abs(hit_Cell.transform.position.x - mouse_Point.x) >= Math.Abs(mouse_Point.y - hit_Cell.transform.position.y))
                {
                    direction = "����";
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x - 0.5f, hit_Cell.transform.position.y)));
                }
                else
                {
                    direction = "��";
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x, hit_Cell.transform.position.y + 0.5f)));
                }
            }
            // �Ʒ�
            else
            {
                if (Math.Abs(mouse_Point.x - hit_Cell.transform.position.x) >= Math.Abs(hit_Cell.transform.position.y - mouse_Point.y))
                {
                    direction = "����";
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x - 0.5f, hit_Cell.transform.position.y)));
                }
                else
                {
                    direction = "�Ʒ�";
                    other_Cell = Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x, hit_Cell.transform.position.y - 0.5f)));
                }
            }
        }

        Debug.Log("�ٲ� ������Ʈ ����: " + direction);
        return other_Cell;
    }

    // �� ��ġ ����
    void ChangeCell(GameObject own_Cell,GameObject other_Cell)
    {
        own_Cell.transform.position = Vector2.MoveTowards(own_Cell.transform.position, other_Position, 2.5f * Time.deltaTime);
        other_Cell.transform.position = Vector2.MoveTowards(other_Cell.transform.position, hit_Position, 2.5f * Time.deltaTime);
    }

    // ��ġ Ÿ�� Ȯ��
    bool MatchType(GameObject own_Cell, Dictionary<string, GameObject> match_Cell_Dic = null)
    {
        if (match_Cell_Dic == null)
        {
            match_Cell_Dic = new Dictionary<string, GameObject>();
        }
       
        GameObject own_Fruit = own_Cell.transform.GetChild(0).gameObject;
        Sprite own_Sprite = own_Fruit.GetComponent<SpriteRenderer>().sprite;
        List<GameObject> direction_Cell = new List<GameObject>()
        {
            Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x - 0.5f, hit_Cell.transform.position.y))),      // ����
            Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x + 0.5f, hit_Cell.transform.position.y))),      // ������
            Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x, hit_Cell.transform.position.y + 0.5f))),      // ��
            Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x, hit_Cell.transform.position.y - 0.5f))),      // �Ʒ�
            Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x - 1.0f, hit_Cell.transform.position.y))),      // ���ʿ���
            Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x + 1.0f, hit_Cell.transform.position.y))),      // �����ʿ�����
            Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x, hit_Cell.transform.position.y + 1.0f))),      // ����
            Ray(main_Camera.WorldToScreenPoint(new Vector2(hit_Cell.transform.position.x, hit_Cell.transform.position.y - 1.0f)))       // �Ʒ��Ʒ�
        };
        List<GameObject> direction_Fruit = new List<GameObject>();
        List<Sprite> direction_Sprite = new List<Sprite>();


        for (int i = 0; i < Enum.GetValues(typeof(Direction)).Length; i++)
        {
            if (direction_Cell[i] != null)
            {
                direction_Fruit.Add(direction_Cell[i].transform.GetChild(0).gameObject);
            }
            else
            {
                direction_Fruit.Add(null);
            }

            if (direction_Fruit[i] != null)
            {
                direction_Sprite.Add(direction_Fruit[i].GetComponent<SpriteRenderer>().sprite);
            }
            else
            {
                direction_Sprite.Add(null);
            }
        }

        // �����¿� ��ġ�ϴ� ���� ã��
        int match_Count = 0;
        for (int i = 0; i < direction_Sprite.Count; i++)
        {
            if (direction_Sprite[i] != null && direction_Cell[i].transform.position != own_Cell.transform.position)
            {
                if (own_Sprite == direction_Sprite[i])
                {
                    match_Count++;
                    match_Cell_Dic.Add(Enum.GetName(typeof(Direction), i), direction_Cell[i]);
                }
            }
        }

        Debug.Log(own_Cell.transform.position);
        Debug.Log(match_Count);

        bool isSuccess = false;

        // ��ġ Ȯ��
        if (match_Count >= 2)
        {
            Debug.Log($"���� ���� {match_Count}��");
            Debug.Log("���� �� ����" + match_Cell_Dic.Count);
            isSuccess = ThreeMatch(match_Cell_Dic);
            if (isSuccess)
            {
                Board.instance.FillEmptyPlace(own_Cell);
            }
            else
            {

            }
        }

        return isSuccess;
    }

    // 3��ġ Ȯ��, 4��ġ 5��ġ�� �̻��ϰ� �Ǽ� else if �� �ٲ���
    bool ThreeMatch(Dictionary<string, GameObject> match_Cell)
    {
        bool isSuccess = false;
        if (match_Cell.ContainsKey(Direction.LEFT.ToString()) && match_Cell.ContainsKey(Direction.RIGHT.ToString()))
        {
            Board.instance.FillEmptyPlace(match_Cell[Direction.LEFT.ToString()]);
            Board.instance.FillEmptyPlace(match_Cell[Direction.RIGHT.ToString()]);
            isSuccess = true;
        }

        else if (match_Cell.ContainsKey(Direction.UP.ToString()) && match_Cell.ContainsKey(Direction.DOWN.ToString()))
        {
            Board.instance.FillEmptyPlace(match_Cell[Direction.UP.ToString()]);
            Board.instance.FillEmptyPlace(match_Cell[Direction.DOWN.ToString()]);
            isSuccess = true;
        }

        else if (match_Cell.ContainsKey(Direction.LEFT.ToString()) && match_Cell.ContainsKey(Direction.LEFTLEFT.ToString()))
        {
            Board.instance.FillEmptyPlace(match_Cell[Direction.LEFT.ToString()]);
            Board.instance.FillEmptyPlace(match_Cell[Direction.LEFTLEFT.ToString()]);
            isSuccess = true;
        }

        else if (match_Cell.ContainsKey(Direction.RIGHT.ToString()) && match_Cell.ContainsKey(Direction.RIGHTRIGHT.ToString()))
        {
            Board.instance.FillEmptyPlace(match_Cell[Direction.RIGHT.ToString()]);
            Board.instance.FillEmptyPlace(match_Cell[Direction.RIGHTRIGHT.ToString()]);
            isSuccess = true;
        }

        else if (match_Cell.ContainsKey(Direction.UP.ToString()) && match_Cell.ContainsKey(Direction.UPUP.ToString()))
        {
            Board.instance.FillEmptyPlace(match_Cell[Direction.UP.ToString()]);
            Board.instance.FillEmptyPlace(match_Cell[Direction.UPUP.ToString()]);
            isSuccess = true;
        }

        else if (match_Cell.ContainsKey(Direction.DOWN.ToString()) && match_Cell.ContainsKey(Direction.DOWNDOWN.ToString()))
        {
            Board.instance.FillEmptyPlace(match_Cell[Direction.DOWN.ToString()]);
            Board.instance.FillEmptyPlace(match_Cell[Direction.DOWNDOWN.ToString()]);
            isSuccess = true;
        }

        return isSuccess;
    }
}
