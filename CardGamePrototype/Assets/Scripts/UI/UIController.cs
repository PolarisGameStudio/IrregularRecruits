using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{

    //responsible for handling open windows and order of open windows
    //todo: should also handle focus, so non-top windows are not interactable
    public class UIController : Singleton<UIController>
    {
        public Stack<IUIWindow> UIWindows = new Stack<IUIWindow>();

        [Header("Objects should be of type IUIWindow, Last Object will be on top")]
        public List<MonoBehaviour> OpenUIWindowsInSceneStart;

        private void Start()
        {
            foreach (var window in OpenUIWindowsInSceneStart)
            {
                if (!(window is IUIWindow win))
                {
                    Debug.LogError($"{window.name} does not inherit ui window");
                    continue;
                }
                Open(win);
            }

        }

        //should this handle all ui open events?
        public void Close(IUIWindow window)
        {

            if (!UIWindows.Any() ||UIWindows.Peek() != window)
                Debug.LogWarning("closing non-top window: " + window);
            else
                UIWindows.Pop();


            window.GetHolder()?.SetActive(false);

            if (UIWindows.Any())
            {
                OpenTopWindow();
            }
        }

        public void Open(IUIWindow window)
        {
            if (UIWindows.Contains(window))
                Debug.LogWarning("opening window already open in window stack");

            //disables the current top window
            if(UIWindows.Any())
            UIWindows.Peek().GetCanvasGroup().interactable = false;

            Debug.Log("opening " + window);

            UIWindows.Push(window);
            OpenTopWindow();
        }

        private void OpenTopWindow()
        {
            IUIWindow newTopWindow = UIWindows.Peek();
            newTopWindow.GetHolder()?.SetActive(true);
            newTopWindow.GetCanvasGroup().interactable = true;
        }
    }
}