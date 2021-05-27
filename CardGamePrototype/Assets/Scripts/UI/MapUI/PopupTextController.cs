using CartoonFX;
using UnityEngine;

namespace UI
{
    public class PopupTextController : Singleton<PopupTextController>
    {
        [SerializeField]
        private CFXR_ParticleText_Runtime TextParticlePrefab;


        public void DisplayText(string text, Vector3 position)
        {
            text = text.ToUpper();

            var instance = Instantiate(TextParticlePrefab);

            instance.transform.position = position;

            instance.text = text;
            //instance.InitializeFirstParticle();
            instance.GenerateText(text);

        }
    }

}