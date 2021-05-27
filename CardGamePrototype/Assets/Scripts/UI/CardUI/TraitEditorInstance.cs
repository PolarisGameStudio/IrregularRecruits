using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TraitEditorInstance : MonoBehaviour
    {
        public Button AddTraitButton;
        public TextMeshProUGUI Text;
        public Button ChangeTraitButton;
        public Button RemoveTraitButton;
        public Trait Trait;


        public void UpdateTrait(Trait t)
        {
            AddTraitButton.image.sprite = t.Icon;
            Text.text = t.name + " (" + t.Description + ")";

            Trait = t;
        }
    }
}