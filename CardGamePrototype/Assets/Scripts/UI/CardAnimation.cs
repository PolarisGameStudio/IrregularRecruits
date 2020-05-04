using GameLogic;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class CardAnimation : MonoBehaviour
    {
        public Material DissolveMaterial, HighlightMaterial;

        public EffectAnimation DamageAnimation, StatPlusAnimation, StatMinusAnimation;

        private Material TargetMaterial, InstigatorMaterial;

        private Image[] ControlledImages;
        private TextMeshProUGUI[] ControlledTexts;

        [Range(0.1f, 3)]
        public float DissolveSpeed = 1f;
        private float DissolveAmount;

        public ParticleSystem PushoutParticles;

        public Image SpecialAbilityIcon;

        public LayoutElement LayoutElement;
        private float prefWidth;

        void Start()
        {
            ControlledImages = GetComponentsInChildren<Image>(true);
            ControlledTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
            DissolveMaterial = Instantiate(DissolveMaterial);
            HighlightMaterial = Instantiate(HighlightMaterial);

            prefWidth = LayoutElement.preferredWidth;

            foreach (var i in ControlledImages)
                i.material = DissolveMaterial;
        }
        
        public IEnumerator Dissolve()
        {
            //Debug.Log("Dissolving " + gameObject.name + ", controlled images:" + ControlledImages.Length);

            while (DissolveAmount < 1)
            {
                DissolveAmount = Mathf.Clamp01(DissolveAmount + Time.deltaTime * DissolveSpeed);
                DissolveMaterial.SetFloat("DissolveAmount", DissolveAmount);

                foreach (var t in ControlledTexts)
                    t.alpha = 1 - DissolveAmount;
                yield return null;
            }

        }
        public IEnumerator UnDissolve()
        {

            while (DissolveAmount > 0)
            {
                DissolveAmount = Mathf.Clamp01(DissolveAmount - Time.deltaTime);
                DissolveMaterial.SetFloat("DissolveAmount", DissolveAmount);

                foreach (var t in ControlledTexts)
                    t.alpha = 1 - DissolveAmount;
                yield return null;
            }
        }

        public void HighlightAbility()
        {
            if (!SpecialAbilityIcon) return;

            LeanTween.scale(SpecialAbilityIcon.rectTransform, Vector3.one * 3.5f, 0.4f).setOnComplete(() =>
                LeanTween.scale(SpecialAbilityIcon.rectTransform, Vector3.one, 0.3f));

            //PushoutParticles.Play();

        }

        public void ChangeLayoutSizeWhileMoving()
        {
            StartCoroutine(ChangeWidthToMove());
        }

        private IEnumerator ChangeWidthToMove()
        {
            var parent = transform.parent;

            var anitime = 0.2f;
            var start = Time.time;

            LayoutElement.preferredWidth = 0;

            yield return new WaitUntil(() => parent != transform.parent);

            start = Time.time;
            while (Time.time < start + anitime)
            {
                LayoutElement.preferredWidth = Mathf.Lerp(0f, prefWidth, (Time.time - start) / anitime);
                yield return null;
            }

        }

        public void EnterBattleField()
        {
            //PushoutParticles.Play();
        }

    }
}