using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    public SpriteRenderer fruit_SpriteRender;
    public List<Sprite> fruit_Sprite = new List<Sprite>();

    void Awake()
    {
    }



    void OnEnable()
    {
        // ���� �̹��� ���� ����
        int random = Random.Range(0, 15);
        fruit_SpriteRender.sprite = fruit_Sprite[random];
    }
}
