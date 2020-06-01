using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class GameSettingsUI : MonoBehaviour
    {
        private GameSettings GS;

        public Slider CombatSpeedSlider;
        public Slider StartingHandSlider;
        public Slider DrawSlider;
        public Slider ActionsPrTurnSlider;
        public Slider EnemyAdditionsSlider;
        public Slider RareEnemySlider;

        private void Start()
        {
            GS = GameSettings.Instance;

            CombatSpeedSlider.value = GS.CombatSpeed; 
            CombatSpeedSlider.onValueChanged.AddListener(i => GS.CombatSpeed = i);
            StartingHandSlider.value = GS.StartingHandSize;
            StartingHandSlider.onValueChanged.AddListener(i => GS.StartingHandSize = (int)i);
            DrawSlider.value = GS.DrawPrTurn;
            DrawSlider.onValueChanged.AddListener(i => GS.DrawPrTurn = (int)i);
            ActionsPrTurnSlider.value = GS.PlaysPrTurn;
            ActionsPrTurnSlider.onValueChanged.AddListener(i => GS.PlaysPrTurn = (int)i);
            EnemyAdditionsSlider.value = CombatPrototype.Instance.GetCombatDifficultyIncrease();
            EnemyAdditionsSlider.onValueChanged.AddListener(i=>  CombatPrototype.Instance.SetCombatDifficultyIncrease( (int)i));
        }



        //TODO: move the settings to a gamesettings Behaviour
        public void AiControlsPlayer(bool ai)
        {
            GS.AiControlledPlayer = ai;
        }


    }
}