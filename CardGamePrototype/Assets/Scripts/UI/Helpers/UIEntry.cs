using UnityEngine;

//Used by EntryPopulaterUI
public abstract class UIEntry<T> : MonoBehaviour
{
    public abstract void Open(T data);

}
