using UnityEngine;
using UnityEngine.UI;

namespace UI
{

    //when the button is clicked it closes the uiwindow in its parents
    [RequireComponent(typeof(Button))]
    public class CloseUIWindowButton : MonoBehaviour
    {
        private void Awake()
        {
            var window= GetComponentInParent<IUIWindow>();

            GetComponent<Button>().onClick.AddListener((() => UIController.Instance.Close(window)));
        }

    }
}