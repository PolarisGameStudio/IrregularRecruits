using UnityEngine;


namespace UI
{
    public abstract class UIWindow : Singleton<UIWindow>
    {
        [HideInInspector]
        public UIWindow FirstChild;
        [HideInInspector]
        public UIWindow Parent;
        public GameObject Holder;

        public virtual void Open() {
            Holder.SetActive(true);
        }
        public virtual void Close() {
            Holder.SetActive(false);
        }

    }

}