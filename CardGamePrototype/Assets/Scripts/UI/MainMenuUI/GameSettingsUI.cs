using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class GameSettingsUI : Singleton<GameSettingsUI>, IUIWindow
    {
        private GameSettings GS;

        public Slider CombatSpeedSlider;
        public Slider StartingHandSlider;
        public Slider DrawSlider;
        public Slider ActionsPrTurnSlider;
        public Slider RareEnemySlider;
        public Toggle AiToggle;
        public GameObject Holder;
        public CanvasGroup FocusGroup;

        private void Start()
        {
            GS = GameSettings.Instance;

            AiToggle.isOn = GS.AiControlledPlayer;
            AiToggle.onValueChanged.AddListener(GS.AiControlsPlayer);
            CombatSpeedSlider.value = GS.CombatSpeed; 
            CombatSpeedSlider.onValueChanged.AddListener(i => GS.CombatSpeed = i);
            StartingHandSlider.value = GS.StartingHandSize;
            StartingHandSlider.onValueChanged.AddListener(i => GS.StartingHandSize = (int)i);
            DrawSlider.value = GS.DrawPrTurn;
            DrawSlider.onValueChanged.AddListener(i => GS.DrawPrTurn = (int)i);
            ActionsPrTurnSlider.value = GS.PlaysPrTurn;
            ActionsPrTurnSlider.onValueChanged.AddListener(i => GS.PlaysPrTurn = (int)i);
            
        }


        public CanvasGroup GetCanvasGroup()
        {
            return FocusGroup;
        }

        public GameObject GetHolder()
        {
            return Holder;
        }

        public int GetPriority() => 10;
    }
}