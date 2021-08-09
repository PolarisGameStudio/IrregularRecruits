using GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{

    public class AbilityUI : MonoBehaviour
    {
        //false == passive
        //TODO: divide into different classes to clean up
        public AbilityButton AbilityButton;
        public bool HeroViewAbility;
        public bool ActiveAbility;
        public Image AbilityImage;
        public Image BorderImage;
        public Image ActivationFillImage;
        public ParticleSystem OutlineParticles;

        public bool Activatable;

        public AbilityWithEffect Ability;
        public Hero Owner;
        [HideInInspector]
        public bool IsLevelUpOption;

        private void Start() 
        {
            if(!HeroViewAbility)
                UIFlowController.Instance.OnEmptyQueue.AddListener(SetExecutability);

            AbilityButton.AbilityUI = this;
        }



        public void SetExecutability()
        {
            if (!Ability) return;

            if (Ability.CanExecute(Owner, null) )
                SetAbilityAsActivable();
            else
                LockAbility();
        }

        private void SetAbilityAsActivable()
        {
            if (!ActiveAbility) return;

            Activatable = true;

            OutlineParticles.Play();
            
            AbilityImage.color = HeroView.Instance.NormalAbilityColor;
            AbilityImage.material = HeroView.Instance.NormalMaterial;
        }

        public void LockAbility()
        {
            if (!ActiveAbility) return;

            Activatable = false;

            OutlineParticles.Stop();

            AbilityImage.color = HeroView.Instance.UnselectableColor;
            AbilityImage.material = HeroView.Instance.GrayScaleMaterial;
        }

        public void Activate()
        {
            AbilityHoverInfo.Instance.Hide();

            if (!Ability || !(Ability is ActiveAbility)) return;

            LockAbility();

            (Ability as ActiveAbility).ActivateAbility(Owner);            
        }        

        public void SetAbility(AbilityWithEffect ability,Hero owner)
        {
            Ability = ability;

            ActiveAbility = ability is ActiveAbility;

            Owner = owner;

            AbilityImage.sprite = ability.Icon;

            IsLevelUpOption = false;

            OutlineParticles.Stop();

            if (HeroViewAbility)
            {
                //normal look
                if(owner.Abilities.Contains(ability))
                {
                    AbilityImage.color = HeroView.Instance.NormalAbilityColor;
                    AbilityImage.material = HeroView.Instance.NormalMaterial;

                }
                else if(owner.GetLevelUpOptions().Contains(ability))
                {
                    // Level up ability possible
                    if(owner.LevelUpPoints >0)
                    {
                        IsLevelUpOption = true;

                        OutlineParticles.Play();

                        AbilityImage.color = HeroView.Instance.NormalAbilityColor;
                        AbilityImage.material = HeroView.Instance.NormalMaterial;
                    }
                    else
                    {
                        //unselected 
                        AbilityImage.color = HeroView.Instance.NotSelectedColor;
                        AbilityImage.material = HeroView.Instance.NormalMaterial;

                    }
                }
                else
                {
                    //unselectable look
                    AbilityImage.color = HeroView.Instance.UnselectableColor;
                    AbilityImage.material = HeroView.Instance.GrayScaleMaterial;


                }
            }
            else
                SetExecutability();
        }

        public void SelectLevelUp()
        {

            Owner.SelectLevelUpAbility(Ability);            

            AnimationSystem.PlayAbilitySelection(transform.position);

            BattleUI.OnAbilitySelect.Invoke(Ability);
        }

    }

}