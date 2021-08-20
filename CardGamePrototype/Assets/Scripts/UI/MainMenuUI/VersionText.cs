using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class VersionText : MonoBehaviour
    {
        public void Start()
        {
            GetComponent<TextMeshProUGUI>().text = Application.version;
        }
    }
}