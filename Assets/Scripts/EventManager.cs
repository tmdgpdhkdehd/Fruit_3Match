using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;       // ΩÃ±€≈Ê
    public delegate void EventHandler();
    public static event EventHandler MatchBoard;

    void Awake()
    {
        instance = this;
    }

    public void InstantiateBoard()
    {
        MatchBoard();
    }
}
