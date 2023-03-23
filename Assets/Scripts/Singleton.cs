using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : class, new()
{
    private static T _instance;

    public static T instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = new T();
            }
            return _instance;
        }
    }
}