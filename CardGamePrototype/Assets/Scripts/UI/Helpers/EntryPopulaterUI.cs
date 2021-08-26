using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;

//Entry populater populates the UI with entries containing the UI entry
//E is the Entry class which shows the data of type T
//Entries are parented to the EntryParent GameObject. EntryParent should probably use a LayoutGroup
//When the class is reopened all instantiated Entries and destroyed and new ones are created, to reset it.
public abstract class EntryPopulaterUI<E,T> : MonoBehaviour where E : UIEntry<T>
{
    public GameObject Holder;
    public GameObject EntryParent;
    public E EntryPrefab;
    private UnityEvent OnReload = new UnityEvent();

    private void Start()
    {
        Holder?.SetActive(false);

        EntryPrefab.gameObject.SetActive(false);
    }

    public virtual void Open(List<T> datas)
    {
        OnReload.Invoke();
        OnReload.RemoveAllListeners();

        Holder?.SetActive(true);

        foreach(var dataObject in datas)
        {
            var inst = Instantiate(EntryPrefab, EntryParent.transform);

            inst.Open(dataObject);
            inst.gameObject.SetActive(true);

            OnReload.AddListener(() => Destroy(inst.gameObject));
        }

        if(this is IUIWindow window)
        {
            UIController.Instance.Open(window);
        }
    }

    public virtual void Close()
    {
        Holder?.SetActive(false);

    }
}
