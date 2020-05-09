using GameLogic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class AddMinionScreen : Singleton<AddMinionScreen>
    {
        public GameObject Holder;
        public Image DeckIcon;
        //todo: replace with holder array and holder script containing ref to button and ui
        public CardUI FirstCreatureChoice, SecondCreatureChoice, ThirdCreatureChoice;
        public Button AddFirstCreature, AddSecondCreature, AddThirdCreature;
        private Deck Deck;

        public static void SetupMinionScreen(Deck deck)
        {
            Instance.SetupDeckChoice(deck);
        }

        private void SetupDeckChoice(Deck deck)
        {
            var cs = CombatPrototype.Instance.AllCreatures.ToList();

            var friends = cs.Where(c => deck.DeckObject.FriendRaces.Contains(c.Race)).ToList();
            var possibles = cs.Where(c => !deck.DeckObject.EnemyRaces.Contains(c.Race)).ToList();

            DeckIcon.sprite = deck.DeckObject.DeckIcon;
            Deck = deck;

            if (!possibles.Any())
            {
                possibles = cs;
            }
            if (!friends.Any())
            {
                friends = possibles;
            }

            Creature selected = friends[Random.Range(0, friends.Count())];
            SetupChoice(FirstCreatureChoice, AddFirstCreature, selected);

            if(friends.Count() > 1)
                friends.Remove(selected);
            Creature selected2 = friends[Random.Range(0, friends.Count())];
            SetupChoice(SecondCreatureChoice, AddSecondCreature, selected2);

            if (possibles.Count() > 2)
            {
                possibles.Remove(selected);
                possibles.Remove(selected2);
            }
            SetupChoice(ThirdCreatureChoice, AddThirdCreature, possibles[Random.Range(0, possibles.Count())]);

            Holder.SetActive(true);
        }

        private void SetupChoice(CardUI ui, Button addButton, Creature creature)
        {
            ui.UpdateCreature(creature);
            addButton.onClick.RemoveAllListeners();

            addButton.onClick.AddListener(() => Deck.AddCard(new Card(creature)));
            addButton.onClick.AddListener(Close);
        }

        private void Close()
        {
            Holder.SetActive(false);
            CardHighlight.Hide();
        }
    }
}