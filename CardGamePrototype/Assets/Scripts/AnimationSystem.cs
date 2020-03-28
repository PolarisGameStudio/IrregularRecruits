using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem : Singleton<AnimationSystem>
{
    public AnimationCurve AttackAnimationCurve;

    public static IEnumerator AttackAnimation(Card owner, Card target, float duration)
    {
        var rect = owner.GetComponent<RectTransform>();
        var startPos = rect.position;
        var endPos = target.GetComponent<RectTransform>();


        var startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            yield return null;

            rect.position = Vector3.LerpUnclamped(startPos, endPos.position, AnimationSystem.Instance.AttackAnimationCurve.Evaluate((Time.time - startTime) / duration));
        }
    }

}
