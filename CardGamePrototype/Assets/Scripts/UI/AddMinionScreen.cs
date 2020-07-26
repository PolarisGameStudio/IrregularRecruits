using GameLogic;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Event = GameLogic.Event;

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

        private void Start()
        {
            Event.OnHireMinions.AddListener(SetupChoice);
        }

        public static void SetupMinionScreen(Deck deck)
        {
            Instance.SetupDeckChoice(deck);
        }

        private void SetupDeckChoice(Deck deck)
        {
            var cs = CreatureLibrary.Instance.EnemyCreatures;

            if (deck == null || !deck.DeckObject)
                return;

            var friends = cs.Where(c => !c.IsSummon() && deck.DeckObject.FriendRaces.Contains(c.Race)).ToList();
            var possibles = cs.Where(c => !c.IsSummon() && !deck.DeckObject.EnemyRaces.Contains(c.Race)).ToList();

            DeckIcon.sprite = deck.DeckObject.DeckIcon;
            Deck = deck;

            if (!possibles.Any())
            {
                possibles = cs.ToList();
            }
            if (!friends.Any())
            {
                friends = possibles;
            }

            Creature selected = friends[Random.Range(0, friends.Count())];

            if (friends.Count() > 1)
                friends.Remove(selected);

            Creature selected2 = friends[Random.Range(0, friends.Count())];

            if (possibles.Count() > 2)
            {
                possibles.Remove(selected);
                possibles.Remove(selected2);
            }
            var selected3 = possibles[Random.Range(0, possibles.Count())];

            SetupChoice(selected,selected2, selected3);
        }

        //should be a list instead of three specific. Remove this and only use the array args method
        public void SetupChoice(Creature first, Creature second, Creature third)
        {
            SetupChoice(FirstCreatureChoice, AddFirstCreature, first);

            SetupChoice(SecondCreatureChoice, AddSecondCreature, second);

            SetupChoice(ThirdCreatureChoice, AddThirdCreature, third);

            Holder.SetActive(true);
        }

        private void SetupChoice(Creature[] creatures)
        {
            if (creatures.Length < 3)
                SetupChoice(creatures[0], creatures[0], creatures[0]);
            else
                SetupChoice(creatures[0], creatures[1], creatures[2]);

        }

        private void SetupChoice(CardUI ui, Button addButton, Creature creature)
        {
            if (Deck == null) Deck = BattleManager.Instance.PlayerDeck;

            ui.UpdateCreature(creature);
            addButton.onClick.RemoveAllListeners();

            addButton.onClick.AddListener(() => Deck.AddCard(new Card(creature)));
            addButton.onClick.AddListener(Close);
        }

        private void Close()
        {
            Holder.SetActive(false);
            CardHoverInfo.Hide();
        }
    }
}