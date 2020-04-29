using GameLogic;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class DeckViewerUI : Singleton<DeckViewerUI>
    {
        public GameObject Holder;
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

        public void ViewDeck(Deck deck)
        {
            InstatiatedObjects.ForEach(Destroy);
            InstatiatedObjects.Clear();

            foreach (var c in deck.AllCreatures())
            {
                var inst = Instantiate(CardUIInstance, CardUIInstance.transform.parent);

                inst.Card = c;

                inst.gameObject.SetActive(true);
                inst.AlwaysFaceUp = true;

                InstatiatedObjects.Add(inst.gameObject);
            }

            Holder.SetActive(true);
        }


        public void Close()
        {
            Holder.SetActive(false);
        }
    }
}