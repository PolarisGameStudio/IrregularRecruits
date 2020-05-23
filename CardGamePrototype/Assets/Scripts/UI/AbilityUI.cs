using GameLogic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UI
{
    public class AbilityUI : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
    {
        //false == passive
        public bool ActiveAbility;
        public Image AbilityImage;
        public Image BorderImage;
        public Material OutlineMaterial;
        public Ability Ability;
        public Hero Owner;


        public void SetAbilityAsActivable()
        {
            if (!ActiveAbility) return;

            BorderImage.material.SetFloat("OutlineThickness", 3);
            AbilityImage.color = Color.white;
        }

        public void LockAbility()
        {
            if (!ActiveAbility) return;

            BorderImage.material.SetFloat("OutlineThickness", 0);
            AbilityImage.color = BattleUI.Instance.UnactivatableAbilityColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (ActiveAbility)
                Activate();
        }

        private void Activate()
        {
            (Ability as ActiveAbility).ActivateAbility(Owner);
        }

        

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Highlight
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //dehighlight
        }

        public void SetAbility(Ability ability,Hero owner)
        {
            if (ability == Ability) return;

            Ability = ability;

            ActiveAbility = ability is ActiveAbility;

            Owner = owner;

            AbilityImage.sprite = ability.Icon;

            if (ActiveAbility)
            {
                OutlineMaterial = Instantiate(OutlineMaterial);

                BorderImage.material = OutlineMaterial;

                SetAbilityAsActivable();

            }
        }


    }

}