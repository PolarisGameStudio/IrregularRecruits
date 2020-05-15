using UnityEngine;


namespace UI
{
    public class GameSettingsUI : MonoBehaviour
    {
        private GameSettings GS;

        private void Start()
        {
            GS = GameSettings.Instance;
        }


        //TODO: move the settings to a gamesettings Behaviour
        public void AiControlsPlayer(bool ai)
        {
            GS.AiControlledPlayer = ai;
        }
        public void SetCombatSpeed(float f)
        {
            GS.CombatSpeed = f;
        }

        public void SetStartingHandSize(float val)
        {
            GS.StartingHandSize = (int)val;
        }
        public void SetDrawsPrTurn(float val)
        {
            GS.DrawPrTurn = (int)val;
        }
        public void SetPlayerActionsPrTurn(float val)
        {
            GS.PlaysPrTurn = (int)val;
        }
        public void SetEnemyStartCreatures(float val)
        {
            GS.EnemyBattlefieldSize = (int)val;
        }
        public void SetEnemyDeckSize(float val)
        {
            GS.EnemyDeckSize = (int)val;
        }
        public void SetRareEnemiesPrBattle(float val)
        {
            GS.MaxRareEnemiesPrCombat = (int)val;
        }

    }
}