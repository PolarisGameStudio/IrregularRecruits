using GameLogic;
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
        public Slider MasterVolumeSlider;
        public Toggle AiToggle;
        public Toggle AutoEndTurn;
        public GameObject Holder;
        public CanvasGroup FocusGroup;
        public GameObject RestartButton;

        public Toggle VibrationToggle;


        private void Start()
        {
            GS = GameSettings.Instance;

            AiToggle.onValueChanged.AddListener(GS.AiControlsPlayer);
            VibrationToggle.onValueChanged.AddListener(GS.Vibration);
            AutoEndTurn.onValueChanged.AddListener(i => GS.AutoEndTurn.Value = i);
            CombatSpeedSlider.onValueChanged.AddListener(i => GS.CombatSpeed.Value = i);
            StartingHandSlider.onValueChanged.AddListener(i => GS.StartingHandSize = (int)i);
            DrawSlider.onValueChanged.AddListener(i => GS.DrawPrTurn = (int)i);
            ActionsPrTurnSlider.onValueChanged.AddListener(i => GS.PlaysPrTurn = (int)i);
            MasterVolumeSlider.onValueChanged.AddListener(GS.SetVolume);

        }

        public void UpdateValues()
        {
            AiToggle.isOn = GS.AiControlledPlayer.Value;
            AutoEndTurn.isOn = GS.AutoEndTurn.Value;
            CombatSpeedSlider.value = GS.CombatSpeed.Value;
            MasterVolumeSlider.value = GS.Volume.Value;
            StartingHandSlider.value = GS.StartingHandSize;
            DrawSlider.value = GS.DrawPrTurn;
            ActionsPrTurnSlider.value = GS.PlaysPrTurn;
            VibrationToggle.isOn = GS.VibrateEnabled.Value;

            RestartButton.SetActive(Battle.PlayerDeck?.Hero != null);

        }

        public void Save()
        {
            Data.DataHandler.Instance.Save();
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