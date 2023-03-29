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
        // 과일 이미지 랜덤 지정
        int random = Random.Range(0, fruit_Sprite.Count);
        fruit_SpriteRender.sprite = fruit_Sprite[random];
    }
}
