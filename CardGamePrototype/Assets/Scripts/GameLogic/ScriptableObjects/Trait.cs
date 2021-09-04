using UnityEngine;

[CreateAssetMenu(menuName = "Create Game Objects/Trait")]
public class Trait : ScriptableObject
{
    [Header("This only maps the values with the trait of that name written in code.")]
    public new string name;
    public Sprite Icon;
    public string Description;
    public int CR;
    public bool CRScalesWithPower;


}
