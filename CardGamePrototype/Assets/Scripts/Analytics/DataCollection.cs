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

        //unlock event

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
            {
                Dictionary<string, object> eventData = new Dictionary<string, object>()
                {
                    { "NotBought", creature.Item1.name},
                    {"Hero", Battle.PlayerDeck.Hero.HeroObject.name },
                };
                int i = 1;
                foreach (var strat in ShopRecommendation.GetTopStrategies(Battle.PlayerDeck))
                    eventData.Add("Strat" + i++, strat.Key.ToString());

                Analytics.CustomEvent("Card not bought", eventData);
            }
        }

    }

    private void SendPurchaseEvent(Creature creature, Shop shop)
    {
        Dictionary<string, object> eventData = new Dictionary<string, object>()
        {
            { "Bought", creature.name},
            {"Hero", Battle.PlayerDeck.Hero.HeroObject.name }
        };

        int i = 1;
        foreach (var strat in ShopRecommendation.GetTopStrategies(Battle.PlayerDeck))
            eventData.Add("Strat" + i++, strat.Key.ToString());


        Analytics.CustomEvent("Card purchase", eventData);

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
            {"Player Strategy", ShopRecommendation.GetTopStrategies(BattleUI.Instance.InitialPlayerDeck).FirstOrDefault().Key },
            {  "Enemy Faction", enmDeck.GroupBy(q => q.GetRace())
                               .OrderByDescending(gp => gp.Count())
                               .First().Key.name},
            { "Enemy Strategy",ShopRecommendation.GetTopStrategies(BattleUI.Instance.InitialEnemyDeck).FirstOrDefault().Key },
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
        var winners = BattleUI.Instance.InitialPlayerDeck;

        var path = Map.CurrentNode.Map.ChosenPath;

        Dictionary<string, object> eventData = new Dictionary<string, object>
        {
            {"Hero",Battle.PlayerDeck.Hero.HeroObject.name },
            {"Main Strategy", ShopRecommendation.GetTopStrategies(winners).FirstOrDefault().Key },
            //hard battles
            {"Hard Battles Nodes", path.Count(c=>  c == MapNodeType.HardCombat) },
            //normal battles
            {"Std Battles Nodes", path.Count(c=>  c == MapNodeType.StdCombat) },
            //villages
            {"Villages Visited", path.Count(c=>  c == MapNodeType.Village) },
            // events 
            {"Event Loc Visited", path.Count(c=>  c == MapNodeType.Event) },
            //treasures
            {"Treasures Collected", path.Count(c=>  c == MapNodeType.Treasure) },
            //camps
            {"Campfires", path.Count(c=>  c == MapNodeType.Xp) },

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
        var path = Map.CurrentNode.Map.ChosenPath;

        Dictionary<string, object> eventData = new Dictionary<string, object>
        {
            {"Hero",Battle.PlayerDeck.Hero.HeroObject.name },
            {"EndXP",Battle.PlayerDeck.Hero.Xp},
            {"Main Strategy", ShopRecommendation.GetTopStrategies(BattleUI.Instance.InitialPlayerDeck).FirstOrDefault().Key },
            //nodes visited
            {"Nodes visited", path.Count() },
            //hard battles
            {"Hard Battles Nodes", path.Count(c=>  c == MapNodeType.HardCombat) },
            //normal battles
            {"Std Battles Nodes", path.Count(c=>  c == MapNodeType.StdCombat) },
            //villages
            {"Villages Visited", path.Count(c=>  c == MapNodeType.Village) },
            // events 
            {"Event Loc Visited", path.Count(c=>  c == MapNodeType.Event) },
            //treasures
            {"Treasures Collected", path.Count(c=>  c == MapNodeType.Treasure) },
            //camps
            {"Campfires", path.Count(c=>  c == MapNodeType.Xp) },

        };

        Analytics.CustomEvent("Game Over", eventData);

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
                eventData.Add(nonhero.name, false);

        Analytics.CustomEvent("HeroSelected", eventData);

        //TODO: problems when we have more than 10 heroes. we can reset this data at that point anyway

    }

    private void SendCustomEvent(string eventName, params KeyValuePair<string, object>[] eventData)
    {
        Analytics.CustomEvent(eventName, eventData.ToDictionary(pair => pair.Key, pair => pair.Value));
    }



#endif
}
