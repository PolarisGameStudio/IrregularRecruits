using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    public UIZone[] PlayerUIZones;
    public UIZone[] EnemyUIZones;
    [Serializable]
    public struct UIZone { 
        public Deck.Zone Zone; 
        public RectTransform RectTransform; 
        public int CardRotation; 
        public int CardPosAdjust; 
    }

    public TextMeshProUGUI PlayerDeckDescription, EnemyDeckDescription;

    void Awake()
    {
        if (!Instance) Instance = this;

        Event.OnDraw.AddListener(c=> UpdateLibrary());
        Event.OnWithdraw.AddListener(c=> UpdateLibrary());        
    }

    public static Transform GetZoneHolder(Deck.Zone zone, bool enm)
    {
        var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

        return z.FirstOrDefault(u => u.Zone == zone).RectTransform;
    }
    public static int GetZoneAdjust(Deck.Zone zone, bool enm)
    {
        var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

        return z.FirstOrDefault(u => u.Zone == zone).CardPosAdjust;
    }
    public static int GetZoneRotation(Deck.Zone zone, bool enm)
    {
        var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

        return z.FirstOrDefault(u => u.Zone == zone).CardRotation;
    }

    public static void UpdateLibrary()
    {
        Instance.PlayerDeckDescription.text = "Deck size: "+ CombatManager.PlayerDeck.CreaturesInZone(Deck.Zone.Library).Count;
        Instance.EnemyDeckDescription.text = "Deck size: "+ CombatManager.EnemyDeck.CreaturesInZone(Deck.Zone.Library).Count;
    }

    public static void MoveCardToLibrary(Card card)
    {
        Instance.StartCoroutine(Instance.MoveCard(card,Deck.Zone.Library,true));
    }
    public static void Move(Card card, Deck.Zone zone, bool player)
    {
        Instance.StartCoroutine(Instance.MoveCard(card, zone, player));
    }

    private IEnumerator MoveCard(Card card, Deck.Zone zone,bool player)
    {
        var duration = 0.2f;
                       
        var rect = card.GetComponent<RectTransform>();
        var startPos = rect.position;
        var zoneRect = GetZoneHolder(zone,!player);

        if (!zoneRect) yield break;

        var startTime = Time.time;
        var posAdjust = GetZoneAdjust(zone, !player);
        var rot = GetZoneRotation(zone, !player);

        Vector3 endPosition = zoneRect.position + new Vector3 (Random.Range(-posAdjust,posAdjust), Random.Range(-posAdjust, posAdjust)) ;

        rect.SetParent(zoneRect,true);
        rect.SetAsFirstSibling();

        while (Time.time < startTime + duration)
        {
            yield return null;

            rect.position = Vector3.LerpUnclamped(startPos, endPosition, (Time.time - startTime) / duration);
            rect.Rotate(new Vector3(0, 0, Random.Range(-rot, rot)));
        }
        rect.SetParent(zoneRect);
        rect.SetAsLastSibling();
    }


}
