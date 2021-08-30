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
        public static float TimeForNonClick = 0.25f;
        public static float TimeToActivate = 1f;

        public class AbilityButtonEvent : UnityEvent<AbilityButton> { }
        public static AbilityButtonEvent OnHolding = new AbilityButtonEvent();
        public static UnityEvent OnFizzle = new UnityEvent();

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            Held = true;
            HeldStartTime = Time.unscaledTime;

            if (AbilityUI. Activatable)
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
            else if (Held && !(heldDown > TimeToActivate + TimeForNonClick) && AbilityUI.Activatable)
            //    AbilityUI. Activate();
            //else if (AbilityUI.Activatable)
            {
                //fizzle sound
                OnFizzle.Invoke();
            }

            ResetButton();

        }

        public void ResetButton()
        {
            transform.LeanScale(Vector3.one, 0.15f);

            AbilityUI.ActivationFillImage.fillAmount = 0f;
            Held = false;
        }

        public IEnumerator HoldRoutine()
        {
            var nonClickTime = HeldStartTime + TimeForNonClick;

            transform.LeanScale(Vector3.one * 2.5f, 0.15f);

            yield return new WaitUntil(() => !Held || Time.unscaledTime > nonClickTime);

            if (Held)
                OnHolding.Invoke(this);
            else
                yield break;

            AnimationSystem.Instance.Vibrate();

            while (Held && Time.unscaledTime < nonClickTime + TimeToActivate)
            {
                //TODO: shake a little

               AbilityUI. ActivationFillImage.fillAmount = (Time.unscaledTime - nonClickTime) / TimeToActivate;

                yield return null;
            }


            if (Held)
                AbilityUI.Activate();

        }


    }

}