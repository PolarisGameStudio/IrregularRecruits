using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DissolveEffect : MonoBehaviour
{
    public Image Image;
    private Material Material;
    private float DissolveAmount;
    private bool IsDissolving;
    private UnityAction OnDissolved;

    private void Start()
    {
        Material = Instantiate(Image.material);
        Image.material = Material;
    }

    private void Update()
    {
        if (IsDissolving)
        {
            DissolveAmount = Mathf.Clamp01(DissolveAmount + Time.deltaTime);
            Material.SetFloat("DissolveAmount", DissolveAmount);
        }
        else
        {
            DissolveAmount = Mathf.Clamp01(DissolveAmount - Time.deltaTime);
            Material.SetFloat("DissolveAmount", DissolveAmount);
        }

        if (DissolveAmount >= 1f && OnDissolved != null)
            OnDissolved.Invoke();

    }

    public void Dissolve(UnityAction onFinishDissolving = null)
    {
        IsDissolving = true;
        OnDissolved = onFinishDissolving;
    }

}
