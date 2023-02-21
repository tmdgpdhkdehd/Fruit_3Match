using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;       // ΩÃ±€≈Ê
    public delegate void EventHandler(GameObject obj);
    public static event EventHandler EmptyBoard;

    void Awake()
    {
        instance = this;
    }

    public void Match(GameObject obj)
    {
        Debug.Log(obj);
        EmptyBoard(obj);
    }
}
