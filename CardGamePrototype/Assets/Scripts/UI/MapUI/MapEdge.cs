using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
    public class MapEdge : MonoBehaviour
    {
        public Image Line;
        public float DrawTime = 0.5f;

        private void Start()
        {
            StartCoroutine(DrawLine());
        }

        private IEnumerator DrawLine()
        {
            var start = Time.time;
            float endtime = start + DrawTime;

            while (Time.time < endtime)
            {
                yield return new WaitForFixedUpdate();
                Line.fillAmount = Mathf.Lerp(1, 0, (endtime - Time.time)/DrawTime);
            }

            Line.fillAmount = 1f;
        }
    }

}