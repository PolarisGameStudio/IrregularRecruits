using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    public Image Portrait;
    public TextMeshProUGUI NameText;
 
    internal void Setup(Card c)
    {
        Portrait.sprite = c.CardImage.sprite;
        NameText.text = c.name;
    }
}
