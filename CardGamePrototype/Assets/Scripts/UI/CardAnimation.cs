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
            DissolveMaterial = Instantiate(DissolveMaterial);

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

        public void Highlight()
        {
            //HighlightParticles.Play();
        }
        public void TurnOffHighlight()
        {
            //HighlightParticles.Stop();
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

            AnimationSystem.OnCreatureExclamation.Invoke(GetComponentInParent<CardUI>(),CreatureBark.Resurrection);
        }        
    }
}