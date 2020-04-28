using TMPro;
using UnityEngine;

public class EffectAnimation : MonoBehaviour
{
    public float Duration = 0.4f;
    public float ChildAdditionalDuration = 0.1f;
    public float SizeRandomElement = 0.1f;
    public float DurationRandomElement = 0.1f;
    public LeanTweenType TweenType = LeanTweenType.easeSpring;
    public TextMeshProUGUI Text;
    public RectTransform[] ChildImages;

    void OnEnable()
    {
        Hide();
    }

    public void Show(int dmg)
    {
        Hide();

        Text.text = dmg.ToString("N0");
        if (gameObject)
            LeanTween.scale(gameObject, Vector3.one * Random.Range(1 - SizeRandomElement, 1 + SizeRandomElement), Duration + Random.Range(-DurationRandomElement, DurationRandomElement)).setEase(TweenType).setOnComplete(Hide);

        //TODO: the higher the damage, the bigger, quicker and more child objects
        var childDur = Duration;

        foreach (var c in ChildImages)
        {
            childDur += ChildAdditionalDuration;
            c.transform.localScale = Vector3.zero;

            LeanTween.scale(c, Vector3.one * Random.Range(1 - SizeRandomElement, 1 + SizeRandomElement), childDur + Random.Range(-DurationRandomElement, DurationRandomElement)).setEase(TweenType);

        }
    }

    public void Hide()
    {


        LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEase(TweenType);

    }
}
