using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitIcon : MonoBehaviour
{
    public Image Portrait;
    public TextMeshProUGUI NameText;

    internal void Setup(Card c)
    {
        Portrait.sprite = c.Creature.Image;
        NameText.text = c.Name;
    }
}
