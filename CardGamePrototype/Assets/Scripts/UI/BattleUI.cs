using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleUI : Singleton<BattleUI>
{

    public CardUI CardPrefab;

    public UIZone[] PlayerUIZones;
    public UIZone[] EnemyUIZones;

    public Button ViewPlayerDeckButton;

    [Serializable]
    public struct UIZone { 
        public Deck.Zone Zone; 
        public RectTransform RectTransform; 
        public int CardRotation; 
        public int CardPosAdjust; 
    }

    public TextMeshProUGUI PlayerDeckDescription, EnemyDeckDescription;

    public float MoveDuration = 0.2f;

    void Awake()
    {
        Event.OnDraw.AddListener(c=> UpdateLibrary());
        Event.OnWithdraw.AddListener(c=> UpdateLibrary());
        ViewPlayerDeckButton.onClick.AddListener(() => DeckViewerUI.View(CombatManager.PlayerDeck));
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
        Instance.StartCoroutine(Instance.MoveCard(card,Deck.Zone.Library,true,0));
    }
    public static void Move(Card card, Deck.Zone zone, bool player,float delay = 0)
    {
        Instance.StartCoroutine(Instance.MoveCard(card, zone, player,delay));
    }

    private IEnumerator MoveCard(Card card, Deck.Zone zone,bool player, float delay)
    {
        if (!card.BattleRepresentation) yield break;

        yield return new WaitForSeconds(delay);

                       
        var rect = card.BattleRepresentation.GetComponent<RectTransform>();
        var startPos = rect.position;
        var zoneRect = GetZoneHolder(zone,!player);

        if (!zoneRect) yield break;


        var startTime = Time.time;
        var posAdjust = GetZoneAdjust(zone, !player);
        var rot = GetZoneRotation(zone, !player);

        Vector3 endPosition = zoneRect.position + new Vector3 (Random.Range(-posAdjust,posAdjust), Random.Range(-posAdjust, posAdjust)) ;
        
        rect.Rotate(new Vector3(0, 0, Random.Range(-rot, rot)));

        //TODO: use lean tween instead
        //LeanTween.move(card.BattleRepresentation.gameObject, endPosition, duration).setEaseInExpo();//.setOnComplete(c => rect.SetParent(zoneRect));

        card.BattleRepresentation?.CardAnimation.ChangeLayoutSizeWhileMoving();

        while (Time.time < startTime + MoveDuration)
        {
            yield return null;

            //TODO: use animation curve
            rect.position = Vector3.LerpUnclamped(startPos, endPosition, (Time.time - startTime) / MoveDuration);
        }

        //TODO: hack that should not be needed
        rect.localScale = Vector3.one;
        rect.SetParent(zoneRect);
        rect.SetAsLastSibling();
    }


}
