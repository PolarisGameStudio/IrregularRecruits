using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLogic;
using Event = GameLogic.Event;
using UnityEngine.Analytics;
using System;
using System.Linq;

public class DataCollection : MonoBehaviour
{
#if ENABLE_CLOUD_SERVICES_ANALYTICS

    void Awake()
    {
        Event.OnHeroSelect.AddListener(SendHeroSelect);
        Event.OnGameBegin.AddListener(SendGameBegin);
        Event.OnGameWin.AddListener(SendGameOver);
        Event.OnBattleFinished.AddListener(SendBattleData);
    }

    private void SendBattleData(Deck arg0, Deck arg1)
    {
    }

    private void SendGameWin()
    {

    }

    private void SendGameOver()
    {
    }

    private void SendGameBegin()
    {
    }

    private void SendHeroSelect(Hero hero)
    {
        Analytics.CustomEvent("HeroSelected", new Dictionary<string, object>
        {
            { "Selected", hero.HeroObject.name}
        });
    }

    private void SendCustomEvent(string eventName, params KeyValuePair<string, object>[] eventData )
    {
        Analytics.CustomEvent(eventName, eventData.ToDictionary(pair => pair.Key,pair => pair.Value));
    }



#endif
}
