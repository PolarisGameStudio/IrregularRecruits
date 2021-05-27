using GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitIcon : MonoBehaviour
    {
        public Image Portrait;
        public TextMeshProUGUI NameText;

        internal void Setup(Card c)
        {
            Portrait.sprite = c.Creature.Image;
            NameText.text = c.GetName();
        }
    }
}