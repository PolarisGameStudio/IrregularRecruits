using UnityEngine.UI;


namespace UI
{
    public class OpenSettingsButton : Button
    {
        protected override void Start()
        {
            onClick.AddListener(GameSettingsUI.Instance.UpdateValues);
            onClick.AddListener(() => UIController.Instance.Open(GameSettingsUI.Instance));
        }
    }

}