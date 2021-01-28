using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DeckViewerUI : Singleton<DeckViewerUI>, IUIWindow
    {
        public GameObject Holder;
        public CanvasGroup FocusGroup;
        public Image WaterMark;
        public CardUI CardUIInstance;
        private List<GameObject> InstatiatedObjects = new List<GameObject>();

        private void Start()
        {
            Holder.SetActive(false);

        }

        public static void View(Deck deck)
        {
            if (deck == null) return;

            Instance.ViewDeck(deck);
        }

        public static void ViewPlayerDeck()
        {
            View(BattleManager.Instance.PlayerDeck);
        }

        public void ViewDeck(Deck deck)
        {
            InstatiatedObjects.ForEach(Destroy);
            InstatiatedObjects.Clear();

            WaterMark.sprite = deck.Races.First().Icon;

            foreach (var c in deck.AllCreatures().OrderBy(c=> c.GetName()))
            {
                var inst = Instantiate(CardUIInstance, CardUIInstance.transform.parent);

                inst.SetCard( c);

                inst.gameObject.SetActive(true);
                inst.AlwaysFaceUp = true;

                InstatiatedObjects.Add(inst.gameObject);
            }

            UIController.Instance.Open(this);

        }


        public CanvasGroup GetCanvasGroup()
        {
            return FocusGroup;
        }


        public GameObject GetHolder()
        {
            return Holder;
        }
    }
}