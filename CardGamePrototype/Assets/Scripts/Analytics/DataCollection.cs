using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic;
using Event = GameLogic.Event;
using UnityEngine.Analytics;
using System;
using System.Linq;
using UI;
using MapLogic;


//TODO: reuse a Dictionary object and just clear it before putting in new stuff, to help performance
public class DataCollection : MonoBehaviour
{
#if ENABLE_CLOUD_SERVICES_ANALYTICS

    void Awake()
    {
        Event.OnHeroSelect.AddListener(SendHeroSelect);
        Event.OnGameOver.AddListener(SendGameOver);
        Event.OnGameWin.AddListener(SendGameWin);
        Event.OnBattleFinished.AddListener(SendBattleData);

        Shop.OnShopPurchase.AddListener(SendPurchaseEvent);
        Shop.OnShopRemovingOptions.AddListener(SendNonPurchaseEvent);

        Event.OnAbilitySelection.AddListener(SendLevelUpData);
    }

    private void SendLevelUpData(SpecialAbility arg0)
    {
        var hero = Battle.PlayerDeck.Hero;

        Analytics.CustomEvent("Ability Select", new Dictionary<string, object>()
        {
            { "Ability", arg0.Name},
            { "CurrentLevel",hero.CurrentLevel },
            {"Hero", Battle.PlayerDeck.Hero.HeroObject.name }
        });
    }


    private void SendNonPurchaseEvent(Shop shop)
    {
        var gold = Map.PlayerGold;

        foreach (var creature in shop.OnOffer)
        {
            if (creature.Item2 <= gold)
                Analytics.CustomEvent("Card not bought", new Dictionary<string, object>()
            {
                { "NotBought", creature.Item1.name},
                {"Hero", Battle.PlayerDeck.Hero.HeroObject.name }
            });
        }

    }

    private void SendPurchaseEvent(Creature creature, Shop shop)
    {
        Analytics.CustomEvent("Card purchase", new Dictionary<string, object>()
        {
            { "Bought", creature.name},
            {"Hero", Battle.PlayerDeck.Hero.HeroObject.name }
        });

    }

    private void SendBattleData(Deck winnger, Deck loser)
    {
        var initialPDeck = BattleUI.Instance.InitialPlayerDeck;
        var enmDeck = BattleUI.Instance.InitialEnemyDeck;

        Deck playerDeck = Battle.PlayerDeck;
        bool playerwon = playerDeck == winnger;
        var battleResult = playerwon ? "BattleWon" : "BattleLost";

        int pCr = initialPDeck.Sum(c => c.Creature.CR);
        int eCr = enmDeck.Sum(c => c.Creature.CR);

        Dictionary<string, object> eventData = new Dictionary<string, object>
        {
            {  "Hero", playerDeck.Hero.HeroObject.name},
            {  "Player Faction", initialPDeck.GroupBy(q => q.GetRace())
                               .OrderByDescending(gp => gp.Count())
                               .First().Key.name},
            {  "Enemy Faction", enmDeck.GroupBy(q => q.GetRace())
                               .OrderByDescending(gp => gp.Count())
                               .First().Key.name},
            { "PlayerCR", pCr },
            { "EnemyCr", eCr },
            { "CR Difference",pCr-eCr },
        };

        Analytics.CustomEvent(battleResult, eventData);

        //Creature results
        if (!playerwon)
            foreach (var creature in enmDeck)
                SendCustomEvent("Player Killer", new KeyValuePair<string, object>("Killer", creature.GetName()));

    }

    private void SendGameWin()
    {
        var winners =  BattleUI.Instance.InitialPlayerDeck; 
        
        Dictionary<string, object> eventData = new Dictionary<string, object>
        {
            {"Hero",Battle.PlayerDeck.Hero.HeroObject.name },
            //{"Main Strategy", ShopRecommendation.GetTopStrategies(Battle.PlayerDeck).Fi }
            //hard battles
            //normal battles
            //villages
            // events 
            //treasures
            //camps

        };

        Analytics.CustomEvent("Game Won", eventData);

        //abilities
        foreach (var ability in Battle.PlayerDeck.Hero.Abilities)
            SendCustomEvent("Ability Won Game", new KeyValuePair<string, object>("Ability", ability.Name));

        foreach (var creature in winners)
            SendCustomEvent("Unit Won Game", new KeyValuePair<string, object>("Unit", creature.GetName()));

    }

    private void SendGameOver()
    {

        //nodes visited
        //hero
        //main strat
        //end xp
        //hard battles
        //normal battles
        //villages
        // events 
        //treasures
        //camps
    }


    //send what hero the player selects. And which possible to select were not selected
    private void SendHeroSelect(Hero hero)
    {
        Dictionary<string, object> eventData = new Dictionary<string, object>
        {
            { hero.HeroObject.name, true}
        };

        foreach (var nonhero in DeckLibrary.GetHeroes(false))
            if (nonhero != hero.HeroObject)
                eventData.Add( nonhero.name,false);

        Analytics.CustomEvent("HeroSelected", eventData);

        //TODO: problems when we have more than 10 heroes. we can reset this data at that point anyway

    }

    private void SendCustomEvent(string eventName, params KeyValuePair<string, object>[] eventData)
    {
        Analytics.CustomEvent(eventName, eventData.ToDictionary(pair => pair.Key, pair => pair.Value));
    }



#endif
}
