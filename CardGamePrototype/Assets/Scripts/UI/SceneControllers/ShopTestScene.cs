using GameLogic;
using MapLogic;
using UnityEngine;


namespace UI
{
    public class ShopTestScene : MonoBehaviour
    {
        public DeckObject PlayerDeck;
        public HeroObject PlayerHero;
        public int StartingGold = 100;

        private void Start()
        {
            Battle.SetPlayerDeck(PlayerDeck);
            Battle.SetPlayerHero(new Hero(PlayerHero));

            MapController.Instance.PlayerGold = StartingGold;

            ShopUI.OpenStandardShop();
        }
    }
}