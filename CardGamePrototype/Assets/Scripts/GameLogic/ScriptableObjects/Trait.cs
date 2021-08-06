using UnityEngine;

[CreateAssetMenu(menuName = "Create Game Objects/Trait")]
public class Trait : ScriptableObject
{
    public new string name;
    public Sprite Icon;
    public string Description;
    public int CR;
    public bool CRScalesWithPower;


}
