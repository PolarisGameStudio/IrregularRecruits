using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CardLayoutGroup : MonoBehaviour
    {
        public AnimationCurve XPosition;
        public AnimationCurve YPosition;
        private RectTransform RectTransform;
        public float StopThreshold = 0.02f;

        private List<CardUI> ChildCards = new List<CardUI>();

        private Vector2[] ChildDesiredPositions;

        private void OnEnable()
        {
            ChildCards = GetComponentsInChildren<CardUI>().ToList();
            RectTransform = GetComponent<RectTransform>();
        }

        //private void Update()
        //{
        //    if (ChildCards.Count != ChildDesiredPositions.Length)
        //        UpdateChildrenPositions();

        //    for (int i = 0; i < ChildCards.Count; i++)
        //    {
        //        var card = ChildCards[i];
        //        var desiredPos = ChildDesiredPositions[i];

        //        Vector2 position = card.transform.position;

        //        if ((position - desiredPos).SqrMagnitude() <= StopThreshold)
        //            continue;

        //    }
        //}

        public void AddChild(CardUI cardUI)
        {
            var parent = cardUI.GetComponentInParent<CardLayoutGroup>();

            parent.RemoveChild(cardUI);

            ChildCards.Add(cardUI);

            cardUI.transform.parent = this.RectTransform;

            UpdateChildrenPositions();
        }

        private void RemoveChild(CardUI cardUI)
        {
            ChildCards.Remove(cardUI);

            UpdateChildrenPositions();
        }

        //Should ensure that each card has a 
        private void UpdateChildrenPositions()
        {
            int count = ChildCards.Count;

            //should the movements be set in 
            ChildDesiredPositions = new Vector2[count];

            Vector2 middle = RectTransform.position;
            var bottomLeft = middle - RectTransform.sizeDelta / 2;

            for (int i = 0; i < count; i++)
            {
                var x = i / count;

                ChildDesiredPositions[i] = new Vector2(XPosition.Evaluate(x), YPosition.Evaluate(x)) + bottomLeft;
            }


            for (int i = 0; i < ChildCards.Count; i++)
            {
                var card = ChildCards[i];
                var desiredPos = ChildDesiredPositions[i];

                card.transform.LeanMove(desiredPos, Random.Range(0.1f, 0.3f));

            }

            // update child as movingToPosition, to only check those cards
        }


        //Should ensure that each card has a 
        private void UpdateChildOrder()
        {
            var moving = ChildCards.First(c => c.Moving);

            if (!moving) return;

            int index = ChildCards.IndexOf(moving);
            var currentDesiredPos = ChildDesiredPositions[index];

            Vector2 position = moving.transform.position;

            //closer to the before position
            if (index > 0 && (currentDesiredPos - position).sqrMagnitude > (ChildDesiredPositions[index - 1] - position).sqrMagnitude)
            {
                //switch positions
                var ca = ChildCards[index - 1];
                ChildCards[index - 1] = moving;
                ChildCards[index] = ca;

                //TODO: should also update order in unity hierachy

            }
            //closer to the after position
            else if (index < ChildCards.Count - 1 && (currentDesiredPos - position).sqrMagnitude > (ChildDesiredPositions[index + 1] - position).sqrMagnitude)
            {
                //switch positions
                var ca = ChildCards[index + 1];
                ChildCards[index + 1] = moving;
                ChildCards[index] = ca;

            }

        }
    }

}