using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private void Start()
    {
        gameObject.transform.parent = Board.instance.board.transform;
    }

    void OnEnable()
    {
        Board.instance.enable_Cells.Add(gameObject);
        if (Board.instance.disable_Cells.Contains(gameObject))
        {
            Board.instance.disable_Cells.Remove(gameObject);
        }
    }

    void OnDisable()
    {
        Board.instance.enable_Cells.Remove(gameObject);
        Board.instance.disable_Cells.Add(gameObject);
    }
}
