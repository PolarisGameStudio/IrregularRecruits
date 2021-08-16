using GameLogic;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests
{
    public  class AbilityEffectTest : TestFixture
    {
        [Test]
        public void DoublerTriggerAbilityTest()
        {

            var doublerAbility = new TriggerDoublerAbility();
            doublerAbility.EffectTrigger = TriggerType.IsDAMAGED;

            var statboostAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.StatPlus, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            var doublerCreature = GenerateTestCreatureWithAbility(doublerAbility);
            var abilityCreature = GenerateTestCreatureWithAbility(statboostAbility);

            doublerCreature.ChangeLocation(Deck.Zone.Battlefield);
            abilityCreature.ChangeLocation(Deck.Zone.Battlefield);

            var executedEffects = new List<SpecialAbility>();

            var currentStrength = doublerCreature.Attack;

            Event.OnAbilityExecution.AddListener((a, o, cs) => executedEffects.Add(a));

            doublerCreature.HealthChange(-1);

            Assert.AreEqual(3, executedEffects.Count);
            Assert.Contains(statboostAbility, executedEffects);
            Assert.Contains(doublerAbility, executedEffects);

            Assert.AreEqual(currentStrength + 2, doublerCreature.Attack);
        }        
        
        [Test]
        public void DoublerAbilityEffectTest()
        {
            var doublerAbility = new EffectDoublerAbility();
            doublerAbility.Effect = EffectType.StatPlus;

            var statboostAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.StatPlus, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            var doublerCreature = GenerateTestCreatureWithAbility(doublerAbility);
            var abilityCreature = GenerateTestCreatureWithAbility(statboostAbility);

            doublerCreature.ChangeLocation(Deck.Zone.Battlefield);
            abilityCreature.ChangeLocation(Deck.Zone.Battlefield);

            var executedEffects = new List<SpecialAbility>();

            var currentStrength = doublerCreature.Attack;

            Event.OnAbilityExecution.AddListener((a, o, cs) => executedEffects.Add(a));

            doublerCreature.HealthChange(-1);

            Assert.AreEqual(3, executedEffects.Count);
            Assert.Contains(statboostAbility, executedEffects);
            Assert.Contains(doublerAbility, executedEffects);

            Assert.AreEqual(currentStrength + 2, doublerCreature.Attack);
        }

        [Test]
        public void ActionWithdrawExecutes()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Withdraw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card withdrawn = null;

            Event.OnWithdraw.AddListener((c,l) => withdrawn = c);
            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.IsNotNull(withdrawn);
        }
        [Test]
        public void ActionCharmExecutes()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Charm, Count.All, 1, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null,null,false);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            Assert.AreNotEqual(TestCard.InDeck, OtherCard.InDeck);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.AreEqual(TestCard.InDeck, OtherCard.InDeck);
        }
        
        [Test]
        public void ActionGainEnergyExecutes()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.GainEnergy, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null,null,false);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            var energy = TestCard.InDeck.DeckController.ActionsLeft;

            OtherCard.HealthChange(-1);


            Assert.IsTrue(triggered);

            Assert.AreEqual(energy+1, TestCard.InDeck.DeckController.ActionsLeft);
        }

        [Test]
        public void ActionDealDamageExecutes()
        {
            int dmg = 1;
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.DealDamage, Count.All, dmg, new Noun(Noun.CharacterTyp.This)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any,Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Any,Deck.Zone.Library), TriggerType.Withdraw),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card damaged = null;

            Event.OnDamaged.AddListener((c,l) => damaged = c);
            Event.OnWithdraw.AddListener((c,l) => triggered = true);

            OtherCard.Withdraw();

            Assert.IsTrue(triggered);
            Assert.IsNotNull(damaged);
            Assert.AreEqual(damaged, TestCard);
            Assert.AreEqual(TestCard.MaxHealth - dmg, TestCard.CurrentHealth);

        }
        [Test]
        public void ActionDrawExecutes()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Draw, Count.All, 1, new Noun(Noun.CharacterTyp.Any)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Library);

            bool triggered = false;
            Card drawn = null;

            Event.OnDraw.AddListener((c,l) => drawn = c);
            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.IsNotNull(drawn);
            Assert.AreEqual(drawn.Location, Deck.Zone.Hand);
        }
        [Test]
        public void ActionHealExecutes()
        {
            int amount = 1;
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Heal, Count.All, amount, new Noun(Noun.CharacterTyp.It)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card healed = null;

            Event.OnHealed.AddListener((c, i,l) => healed = c);
            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-amount);

            Assert.IsTrue(triggered);
            Assert.IsTrue(OtherCard.MaxHealth == OtherCard.CurrentHealth);
            Assert.IsNotNull(healed);
        }
        [Test]
        public void ActionKillExecutes()
        {
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Kill, Count.All, 1, new Noun(Noun.CharacterTyp.Other)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card killed = null;

            Event.OnDeath.AddListener((c,l) => killed = c);
            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.AreEqual(killed, OtherCard);
            Assert.IsFalse(OtherCard.Alive());
        }
        [Test]
        public void ActionResurrectExecutes()
        {
            var amount = 1;

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, amount, new Noun(Noun.CharacterTyp.Any,Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Any,Deck.Zone.Graveyard)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Other,Noun.Allegiance.Any,Noun.DamageType.Any,Noun.RaceType.Any,Deck.Zone.Graveyard), TriggerType.DIES),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card death = null;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);
            Event.OnDeath.AddListener((c,l) => death = c);

            OtherCard.Die();

            Assert.NotNull(death);
            Assert.IsTrue(triggered);
            Assert.IsTrue(OtherCard.Alive());

            Assert.AreEqual(amount, OtherCard.CurrentHealth);
        }

        [Test]
        public void ActionResurrectRemovesStatChanges()
        {

            const int amount = 1;

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, amount, new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Any, Deck.Zone.Graveyard)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            var startingAttack = OtherCard.Attack;
            var startingHealth = OtherCard.CurrentHealth;

            var statChange = 20;

            OtherCard.StatModifier(statChange);

            Assert.AreEqual(statChange + startingAttack, OtherCard.Attack);
            Assert.AreEqual(statChange + startingHealth, OtherCard.MaxHealth);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Graveyard);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            Assert.IsFalse(OtherCard.Alive());

            TestCard.HealthChange(-2);

            Assert.IsTrue(triggered);
            Assert.IsTrue(OtherCard.Alive());

            Assert.AreEqual( startingAttack, OtherCard.Attack);
            Assert.AreEqual( startingHealth, OtherCard.MaxHealth);

            Assert.AreEqual(amount, OtherCard.CurrentHealth);

        }
        [Test]
        public void ActionResurrectRemovesNegativeStatChanges()
        {
            const int amount = 1;

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Resurrect, Count.One, amount, new Noun(Noun.CharacterTyp.Any, Noun.Allegiance.Any, Noun.DamageType.Any, Noun.RaceType.Any, Deck.Zone.Graveyard)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            var startingAttack = OtherCard.Attack;
            var startingHealth = OtherCard.CurrentHealth;

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);
            
            var statChange = - startingHealth - 2 ;

            OtherCard.StatModifier(statChange);

            bool triggered = false;

            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            Assert.IsFalse(OtherCard.Alive());

            TestCard.HealthChange(-2);

            Assert.IsTrue(triggered);
            Assert.IsTrue(OtherCard.Alive());

            Assert.AreEqual( startingAttack, OtherCard.Attack);
            Assert.AreEqual( startingHealth, OtherCard.MaxHealth);

            Assert.AreEqual(amount, OtherCard.CurrentHealth);
        }


        [Test]
        public void ActionStatminusExecutes()
        {
            const int amount = 2;
            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.StatMinus, Count.One, amount, new Noun(Noun.CharacterTyp.Other)),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.This), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            var expectedAttack = OtherCard.Attack - amount;
            var expectedHealth = OtherCard.CurrentHealth - amount;
            var expectedMaxHealth = OtherCard.MaxHealth- amount;

            bool triggered = false;


            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            TestCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.AreEqual(expectedAttack, OtherCard.Attack);
            Assert.AreEqual(expectedHealth, OtherCard.CurrentHealth);
            Assert.AreEqual(expectedMaxHealth, OtherCard.MaxHealth);

        }
        
        [Test]
        public void ActionSummonExecutes()
        {
            var summon = GenerateSummon();

            var testAbility = new PassiveAbility()
            {
                ResultingAction = new AbilityEffectObject(EffectType.Summon, Count.All, 1, new Noun(Noun.CharacterTyp.Any),summon),
                TriggerCondition = new Trigger(new Noun(Noun.CharacterTyp.Any), TriggerType.IsDAMAGED),
            };

            TestCard = GenerateTestCreatureWithAbility(testAbility);
            OtherCard = GenerateTestCreatureWithAbility(null);

            TestCard.ChangeLocation(Deck.Zone.Battlefield);
            OtherCard.ChangeLocation(Deck.Zone.Battlefield);

            bool triggered = false;
            Card summoned = null;

            Event.OnSummon.AddListener((c,l) => summoned = c);
            Event.OnAbilityExecution.AddListener((a, c, ts) => triggered = true);

            OtherCard.HealthChange(-1);

            Assert.IsTrue(triggered);
            Assert.IsNotNull(summoned);
            Assert.AreEqual(summoned.Creature, summon);
        }
    }
}
