using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MapUI
{
    public class MapEdge : MonoBehaviour
    {
        public Image Line;
        private float DrawTime = 0.25f;

        private void Start()
        {
            Line.sprite = MapUI.GetRandomLineSprite();

            //StartCoroutine(DrawLine());
        }

        public IEnumerator DrawLine()
        {
            var start = Time.time;
            float endtime = start + DrawTime;

            while (Time.time < endtime)
            {
                Line.fillAmount = Mathf.Lerp(1, 0, (endtime - Time.time)/DrawTime);
                yield return new WaitForFixedUpdate();
            }

            Line.fillAmount = 1f;
        }
    }

}