using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{

    //responsible for handling open windows and order of open windows
    //todo: should also handle focus, so non-top windows are not interactable
    public class UIController : Singleton<UIController>
    {
        public List<IUIWindow> UIWindows = new List<IUIWindow>();

        [Header("Objects should be of type IUIWindow, Last Object will be on top")]
        public List<MonoBehaviour> OpenUIWindowsInSceneStart;

        private void Start()
        {
            foreach (var window in OpenUIWindowsInSceneStart)
            {
                if (!(window is IUIWindow win))
                {
                    Debug.LogWarning($"{window.name} does not inherit ui window");
                    continue;
                }
                Open(win);
            }

        }

        //should this handle all ui open events?
        public void Close(IUIWindow window)
        {
            UIWindows.Remove(window);

            window.GetHolder()?.SetActive(false);

            if (UIWindows.Any())
            {
                OpenTopWindow();
            }
        }

        public void Open(IUIWindow window)
        {
            if (UIWindows.Contains(window))
            {
                Debug.LogWarning("opening window already open in window stack: " + window);
                return;
            }
            else
                UIWindows.Add(window);

            Debug.Log("opening " + window);

            UIWindows = UIWindows.OrderBy(ui => ui.GetPriority()).ToList();

            //disables the current top window
            foreach (var ui in UIWindows)
            {
                ui.GetCanvasGroup().interactable = ui == UIWindows.Last();
            }

            Debug.Log("opening " + UIWindows.Last());

            OpenTopWindow();
        }

        private void OpenTopWindow()
        {
            IUIWindow newTopWindow = UIWindows.Last();
            newTopWindow.GetHolder()?.SetActive(true);
            newTopWindow.GetCanvasGroup().interactable = true;
        }
    }
}