using GameLogic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UI
{
    public class AbilityUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
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
            if (HeroViewAbility)
                Button.onClick.AddListener(SelectLevelUp);
            else
                Button.onClick.AddListener(Activate);
        }

        public void SetAbilityAsActivable()
        {
            if (!ActiveAbility) return;

            OutlineParticles.Play();
        }

        public void LockAbility()
        {
            if (!ActiveAbility) return;

            OutlineParticles.Stop();

            AbilityImage.color = BattleUI.Instance.UnactivatableAbilityColor;
        }


        private void Activate()
        {
            if (!Ability || !(Ability is ActiveAbility)) return;

            (Ability as ActiveAbility).ActivateAbility(Owner);
        }        

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Highlight
            AbilityHighlight.Show(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //dehighlight
            AbilityHighlight.Hide();
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
                        Button.interactable = true;
                    }
                    else
                    {
                        //unselected 
                        AbilityImage.color = HeroView.Instance.NotSelectedColor;

                        Button.interactable = false;
                    }
                }
                else
                {
                    //unselectable look
                    AbilityImage.color = HeroView.Instance.UnselectableColor;

                    Button.interactable = false;

                }
            }
            else if (ActiveAbility)
            {
                SetAbilityAsActivable();
            }
        }

        private void SelectLevelUp()
        {
            if(HeroViewAbility && OutlineParticles.isPlaying)
                Owner.SelectLevelUpAbility(Ability);
        }

    }

}