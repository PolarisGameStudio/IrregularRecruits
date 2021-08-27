using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace GameLogic
{


    public class Deck 
    {
        public enum Zone { Library, Battlefield, Graveyard, Hand, COUNT }
        public DeckObject DeckObject;
        public DeckController DeckController;
        public int CR;
        public HashSet<Race> Races = new HashSet<Race>();

        private Dictionary<Zone, List<Card>> Creatures = new Dictionary<Zone, List<Card>>();

        public Hero Hero;

        public Deck(DeckObject deckObject)
            : this(deckObject.Creatures.Select(c => new Card(c)).ToList())
        {
            DeckObject = deckObject;
        }

        public Deck(List<Card> initialLibrary)
        {
            for (int i = (int)Zone.Library; i < (int)Zone.COUNT; i++)
            {
                Creatures[(Zone)i] = new List<Card>();
            }
            
            foreach (var card in initialLibrary)
            {
                AddCard(card);
            }

            //if (!playerDeck)
            //    AI = new AI(this);

        }

        public void AddCard(Card card)
        {
            //unique control 
            Creature creature = card.Creature;
            if (creature.Rarity == Creature.RarityType.Unique && !DeckGeneration.UniquesGenerated.Contains(creature))
                DeckGeneration.UniquesGenerated.Add(creature);

            card.InDeck = this;
            card.ChangeLocation(Deck.Zone.Library);

            //TODO: test that this correctly reflects CR?
            if (!card.IsSummon())
            {
                UpdateCR();

                Races.Add(card.Creature.Race);
            }

            Creatures[Zone.Library].Add(card);

            Event.OnDeckSizeChange.Invoke(this);
        }
        public void Remove(Card card)
        {
            Creatures[card.Location].Remove(card);
            card.InDeck = null;
            card.CleanListeners();

            Event.OnDeckSizeChange.Invoke(this);

            UpdateCR();
        }

        private void UpdateCR()
        {
            CR = AllCreatures().Sum(c => c.Creature.CR);
        }

        internal void Add(Card card)
        {
            Creatures[card.Location].Add(card);

            if (card.Creature.SpecialAbility & !card.ListenersInitialized)
                card.Creature.SpecialAbility.SetupListeners(card);
        }

        public int GetXpValue()
        {
            return AllCreatures().Sum(creature => creature.XpValue());
        }

        public List<Card> CreaturesInZone(Zone z)
            => Creatures[z];

        public Zone GetZone(Card card)
        {
            return Creatures.Single(z => z.Value.Contains(card)).Key;
        }

        internal void DrawInitialHand(bool enemy = false)
        {
            ShuffleLibrary();

            var amountToDraw = GameSettings.Instance.StartingHandSize;

            //Move AVANTGARDE cards to the front and shuffle the rest
            while (CreaturesInZone(Zone.Library).Any(c => c.Avantgarde()))
            {
                CreaturesInZone(Zone.Library).First(c => c.Avantgarde()).ChangeLocation(Zone.Library, Zone.Battlefield);

            }


            Draw(amountToDraw);
        }

        public void Reset()
        {
            foreach (var c in AllCreatures())
                c.CleanListeners();

            Creatures.Clear();
        }

        internal void SetPosition(Card card, Zone zone, int position)
        {
            //right now only battlefield position matters
            if (zone != Zone.Battlefield)
                return;
            
            var creaturesInZone = CreaturesInZone(zone);

            if(position >= creaturesInZone.Count)
                throw new System.ArgumentException($"positionning: cannot put {card} in {zone} at pos {position}");

            if (!creaturesInZone.Contains(card))
                throw new System.ArgumentException($"positionning card: {card} not in {zone}");

            //rearranging the cards in same order, with the selected Card at selected position
            //todo: just use list.insert
            var newOrder = new List<Card>();
            for (int i = 0; i < creaturesInZone.Count; i++)
            {
                if (i == position)
                    newOrder.Add(card);

                var other = creaturesInZone[i];

                if (other == card) continue;

                newOrder.Add(other);
            }

            Creatures[zone] = newOrder;

        }

        public void PackUp(bool removeAll = false)
        {
            if (removeAll)
                while (AllCreatures().Any())
                    Remove(AllCreatures().First());

            //removing dead creatures
            while (Creatures[Zone.Graveyard].Any(c=> !c.Deathless()))
                Remove(Creatures[Zone.Graveyard].First(c => !c.Deathless()));

            foreach (var c in AllCreatures())
            {
                c.ResetAfterBattle();
            }
        }

        public void ShuffleLibrary()
        {
            Creatures[Zone.Library] = Creatures[Zone.Library].OrderBy(x => Random.value).ToList();
        }

        public void Draw(int amount)
        {
            if (Creatures[Zone.Library].Count() == 0 || amount < 0) return;
            if (Creatures[Zone.Library].Count() < amount) amount = Creatures[Zone.Library].Count();

            var draws = Creatures[Zone.Library].Take(amount).ToArray();

            foreach (var card in draws)
            {
                Draw(card);
            }
        }

        public void Draw(Card card)
        {
            card.ChangeLocation(Deck.Zone.Library, Deck.Zone.Hand);
        }

        public List<Card> AllCreatures()
        {
            return Creatures.SelectMany(x => x.Value).ToList();
        }

        //returns count of all creatures not in Graveyard
        public int Alive() => Creatures.Sum(a => a.Key == Zone.Graveyard ? 0 : a.Value.Count);

        internal Card GetAttackTarget(bool assassin = false)
        {
            //empty list check?
            if (CreaturesInZone(Zone.Battlefield).Count > 0)
            {
                //assassins don't care..
                if (assassin)
                {
                    List<Card> bf = CreaturesInZone(Zone.Battlefield);

                    if (bf.Any(e=> e.Ethereal()) && !bf.All(e => e.Ethereal()))
                        bf = CreaturesInZone(Zone.Battlefield).Where(c => !c.Ethereal()).ToList();

                    var min = bf.Min(c => c.CurrentHealth);
                    var minCards = bf.Where(c => c.CurrentHealth == min).ToList();

                    return minCards[Random.Range(0, minCards.Count())];

                }

                List<Card> battlefield = CreaturesInZone(Zone.Battlefield).Where(c => !c.Ethereal()).ToList();

                var defenders = battlefield.Where(c => c.Defender()).ToList();

                if (defenders.Any())
                    return defenders[Random.Range(0, defenders.Count())];

                if (!battlefield.Any()) //meaning only ethereals
                    return CreaturesInZone(Zone.Battlefield)[Random.Range(0, CreaturesInZone(Zone.Battlefield).Count)];

                return battlefield[Random.Range(0, battlefield.Count())];
            }
            else if (CreaturesInZone(Zone.Hand).Count > 0)
            {
                return CreaturesInZone(Zone.Hand)[Random.Range(0, CreaturesInZone(Zone.Hand).Count())];
            }
            else if (CreaturesInZone(Zone.Library).Count > 0)
            {
                return CreaturesInZone(Zone.Library)[Random.Range(0, CreaturesInZone(Zone.Library).Count())];
            }
            else return null;
        }

    }
}