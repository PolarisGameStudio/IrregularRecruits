using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CardAnimation : MonoBehaviour
    {
        public Material DissolveMaterial;

        public ParticleSystem HighlightParticles;

        public ParticleSystem WardActiveParticles;

        public EffectAnimation DamageAnimation, StatPlusAnimation, StatMinusAnimation,HealAnimation;

        private Material TargetMaterial, InstigatorMaterial;

        private Image[] ControlledImages;
        private TextMeshProUGUI[] ControlledTexts;

        [Range(0.1f, 3)]
        public float DissolveSpeed = 1f;
        private float DissolveAmount;

        public LayoutElement LayoutElement;
        private float prefWidth;

        void Awake()
        {
            ControlledImages = GetComponentsInChildren<Image>(true);
            ControlledTexts = GetComponentsInChildren<TextMeshProUGUI>(true);

            prefWidth = LayoutElement.preferredWidth;

            if (!GetComponent<CardUI>().PreviewCard)
            {
                DissolveMaterial = Instantiate(DissolveMaterial);
            }
            else
            {
                DissolveMaterial = null;
            }
            foreach (var i in ControlledImages)
                i.material = DissolveMaterial;
        }
        
        public IEnumerator Dissolve()
        {
            //Debug.Log("Dissolving " + gameObject.name + ", controlled images:" + ControlledImages.Length);

            while (DissolveAmount < 1)
            {
                DissolveAmount = Mathf.Clamp01(DissolveAmount + Time.deltaTime * DissolveSpeed);
                DissolveMaterial?.SetFloat("DissolveAmount", DissolveAmount);

                foreach (var t in ControlledTexts)
                    t.alpha = 1 - DissolveAmount;
                yield return null;
            }
        }

        public void Highlight()
        {
            HighlightParticles?.Play();
        }
        public void TurnOffHighlight()
        {
            HighlightParticles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public IEnumerator UnDissolve(bool shout = true)
        {
            DissolveAmount = 1f;
            DissolveMaterial?.SetFloat("DissolveAmount", DissolveAmount);

            while (DissolveAmount > 0)
            {
                DissolveAmount = Mathf.Clamp01(DissolveAmount - Time.deltaTime);
                DissolveMaterial? .SetFloat("DissolveAmount", DissolveAmount);

                foreach (var t in ControlledTexts)
                    t.alpha = 1 - DissolveAmount;
                yield return null;
            }

            if(shout)
                AnimationSystem.Instance.OnCreatureExclamation.Invoke(GetComponentInParent<CardUI>(),CreatureBark.Resurrection);
        }

        internal void SetWarded(bool warded,bool addRemoveFromWatchlist)
        {
            if (!WardActiveParticles)
                return;

            if (warded)
            {
                WardActiveParticles.Play();

                if(addRemoveFromWatchlist)
                    BattleUI.Instance.WardOnBattlefield.Add(this);
            }
            else
            {
                WardActiveParticles.Stop();

                if (addRemoveFromWatchlist)
                    BattleUI.Instance.WardOnBattlefield.Remove(this);
            }
        }

        internal void DestroyWardAni()
        {
            Destroy(WardActiveParticles);
        }

        internal bool Dissolved()
        {
            return DissolveAmount > 0;
        }
    }
}