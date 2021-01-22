using GameLogic;
using MapLogic;
using UnityEngine;


namespace UI
{
    public class ShopTest : MonoBehaviour
    {
        public Race ShopRace;
        public Race[] ShopRaces;
        public int StartingGold = 100;

        private void Start()
        {
            MapController.Instance.PlayerGold = StartingGold;
            var shop = new Shop(ShopRace);
        }
    }
}