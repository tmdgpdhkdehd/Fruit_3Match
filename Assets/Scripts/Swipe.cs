using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("OnMouseEnter");
    }
    void OnMouseOver()
    {
        Debug.Log("OnMouseOver");
    }
    void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
    }
}
