using GameLogic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UI
{
    public class AbilityUI : MonoBehaviour,IPointerClickHandler,  IPointerExitHandler, IPointerEnterHandler
    {
        //false == passive
        public bool HeroViewAbility;
        public bool ActiveAbility;
        public Image AbilityImage;
        public Image BorderImage;
        public ParticleSystem OutlineParticles;
        public Ability Ability;
        public Hero Owner;
        public Button Button;

        private void Start()
        {
            Button.onClick.AddListener(Click);

        }

        private void Click()
        {
            //TODO: write nicer
            if (AbilityHoverInfo.IsActive())
            {
                AbilityHoverInfo.Hide();
                return;
            }

            AbilityHoverInfo.Hide();


            if (HeroViewAbility)
                SelectLevelUp();
            else
                Activate();
        }

        public void SetAbilityAsActivable()
        {
            if (!ActiveAbility) return;

            OutlineParticles.Play();
            
            AbilityImage.color = HeroView.Instance.NormalAbilityColor;
        }

        public void LockAbility()
        {
            if (!ActiveAbility) return;

            OutlineParticles.Stop();

            AbilityImage.color = HeroView.Instance.UnselectableColor;
        }


        private void Activate()
        {
            if (!Ability || !(Ability is ActiveAbility)) return;

            LockAbility();

            (Ability as ActiveAbility).ActivateAbility(Owner);
        }        

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Highlight
            AbilityHoverInfo.Show(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //dehighlight
            AbilityHoverInfo.Hide();
        }

        public void SetAbility(Ability ability,Hero owner)
        {
            Ability = ability;

            ActiveAbility = ability is ActiveAbility;

            Owner = owner;

            AbilityImage.sprite = ability.Icon;

            OutlineParticles.Stop();

            if (HeroViewAbility)
            {
                //normal look
                if(owner.Abilities.Contains(ability))
                {
                    AbilityImage.color = HeroView.Instance.NormalAbilityColor;

                }
                else if(owner.GetLevelUpOptions().Contains(ability))
                {
                    // Level up ability possible
                    if(owner.LevelUpPoints >0)
                    {
                        OutlineParticles.Play();

                        AbilityImage.color = HeroView.Instance.NormalAbilityColor;
                    }
                    else
                    {
                        //unselected 
                        AbilityImage.color = HeroView.Instance.NotSelectedColor;

                    }
                }
                else
                {
                    //unselectable look
                    AbilityImage.color = HeroView.Instance.UnselectableColor;


                }
            }
            else if (ActiveAbility)
            {
                SetAbilityAsActivable();
            }
        }

        private void SelectLevelUp()
        {
            if (!HeroViewAbility || !OutlineParticles.isPlaying)
                return;

            Owner.SelectLevelUpAbility(Ability);            

            AnimationSystem.PlayAbilitySelection(transform.position);

            BattleUI.OnAbilitySelect.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            AbilityHoverInfo.Hide();
        }
    }

}