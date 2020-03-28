using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardAnimation : MonoBehaviour
{
    public Material DissolveMaterial, HighlightMaterial;

    public Animation Animator;

    public EffectAnimation DamageAnimation,StatPlusAnimation,StatMinusAnimation;

    private Material TargetMaterial, InstigatorMaterial;

    private Image[] ControlledImages;
    private TextMeshProUGUI[] ControlledTexts;

    private float DissolveAmount;
    private bool IsDissolving;
    private UnityAction OnDissolved;

    public string HighlightAbilityAnimationName;

    void Start()
    {
        ControlledImages = GetComponentsInChildren<Image>(true);
        ControlledTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        DissolveMaterial = Instantiate(DissolveMaterial);
        HighlightMaterial = Instantiate(HighlightMaterial);

        foreach (var i in ControlledImages)
            i.material = DissolveMaterial;
    }

    private void Update()
    {
        if (IsDissolving)
        {
            DissolveAmount = Mathf.Clamp01(DissolveAmount + Time.deltaTime);
            DissolveMaterial.SetFloat("DissolveAmount", DissolveAmount);

            foreach (var t in ControlledTexts)
                t.alpha = 1 - DissolveAmount;
        }
        else if(DissolveAmount > 0f)
        {
            DissolveAmount = Mathf.Clamp01(DissolveAmount - Time.deltaTime);
            DissolveMaterial.SetFloat("DissolveAmount", DissolveAmount);

            foreach (var t in ControlledTexts)
                t.alpha = 1 - DissolveAmount;
        }

        if (DissolveAmount >= 1f && OnDissolved != null)
        { 
            OnDissolved.Invoke();
            OnDissolved = null;
        }

    }

    public void Dissolve(UnityAction onFinishDissolving = null)
    {
        //Debug.Log("Dissolving " + gameObject.name + ", controlled images:" + ControlledImages.Length);

        IsDissolving = true;
        OnDissolved = onFinishDissolving;
    }
    public void UnDissolve()
    {
        IsDissolving = true;
    }

    public void HighlightAbility()
    {
        Animator.Play(HighlightAbilityAnimationName);
    }
}
