using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    void OnEnable()
    {
        Board.instance.enable_Cells.Add(gameObject);
        Board.instance.disable_Cells.Remove(gameObject);
    }

    void OnDisable()
    {
        Board.instance.enable_Cells.Remove(gameObject);
        Board.instance.disable_Cells.Add(gameObject);
    }
}
