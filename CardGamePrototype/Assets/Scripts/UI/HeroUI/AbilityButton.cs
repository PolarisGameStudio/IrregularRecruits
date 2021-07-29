using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UI
{
    public class AbilityButton : Button
    {
        public AbilityUI AbilityUI;
        public bool Held { get; private set; }
        public float HeldStartTime { get; private set; }

        //how long must the user hold for the click not to count and how long before it then activates the ability
        public static float TimeForNonClick = 0.5f;
        public static float TimeToActivate = 1.5f;
        public static UnityEvent OnHolding = new UnityEvent();
        public static UnityEvent OnFizzle = new UnityEvent();

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            Held = true;
            HeldStartTime = Time.unscaledTime;

            if (!AbilityUI. HeroViewAbility && AbilityUI. OutlineParticles.isPlaying)
                StartCoroutine(HoldRoutine());

        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            var heldDown = Time.unscaledTime - HeldStartTime;

            if (heldDown < TimeForNonClick)
            {
                base.OnPointerUp(eventData);

                AbilityHoverInfo.Show(AbilityUI);
            }
            else if (heldDown > TimeToActivate && !AbilityUI. HeroViewAbility && AbilityUI. OutlineParticles.isPlaying)
                AbilityUI. Activate();
            else if (!AbilityUI.HeroViewAbility && AbilityUI.OutlineParticles.isPlaying)
            {
                //fizzle sound
                OnFizzle.Invoke();
            }

            AbilityUI. ActivationFillImage.fillAmount = 0f;
            Held = false;

        }


        public IEnumerator HoldRoutine()
        {
            var nonClickTime = HeldStartTime + TimeForNonClick;

            yield return new WaitUntil(() => !Held || Time.unscaledTime > nonClickTime);

            if (Held)
                OnHolding.Invoke();
            else
                yield break;


            while (Held && Time.unscaledTime < nonClickTime + TimeToActivate)
            {
                yield return null;

               AbilityUI. ActivationFillImage.fillAmount = (Time.unscaledTime - nonClickTime) / TimeToActivate;
            }

        }


    }

}