using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectAnimation : MonoBehaviour
{
    public float Duration = 0.4f;
    public LeanTweenType TweenType = LeanTweenType.easeSpring;
    public TextMeshProUGUI Text;

    void OnEnable()
    {
        Hide();
    }
    
    public void Show(int dmg)
    {
        Hide();

        Text.text = dmg.ToString("N0");

        LeanTween.scale(gameObject, Vector3.one, Duration).setEase(TweenType).setOnComplete(Hide);
    }

    public void Hide()
    {

        transform.localScale = Vector3.zero;
    }
}
