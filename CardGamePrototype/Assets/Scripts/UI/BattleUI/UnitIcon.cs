using GameLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitIcon : MonoBehaviour
    {
        public Image Portrait;
        public Image SkullIcon;
        public TextMeshProUGUI NameText;
        public Sprite BrokenHeartSprite;

        internal void Setup(Card c, bool charmed)
        {
            Portrait.sprite = c.Creature.Image;
            NameText.text = c.GetName();

            if (charmed)
                SkullIcon.sprite = BrokenHeartSprite;
        }
    }
}