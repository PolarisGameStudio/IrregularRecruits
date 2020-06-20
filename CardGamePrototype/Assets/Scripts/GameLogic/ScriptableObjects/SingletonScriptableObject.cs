using UnityEngine;
using System.Linq;

//The scriptable object should be called the name of the TYPE
public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    static T _instance = null;
    public static T Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.Load<T>(typeof(T).Name);
            return _instance;
        }
    }
}
