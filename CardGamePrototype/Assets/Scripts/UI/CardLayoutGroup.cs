using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CardLayoutGroup : MonoBehaviour
    {
        [Header("If the layout should stick to one point")]
        public bool PointOnly;
        public AnimationCurve XPosition;
        public AnimationCurve YPosition;
        private RectTransform RectTransform;

        public int MinimumPositions = 4;
        public int MaximumPositions = 8;

        private List<CardUI> ChildCards = new List<CardUI>();

        private Vector2[] ChildDesiredPositions;
        private Vector2 StartPos, EndPos;


        private void OnEnable()
        {
            ChildCards = GetComponentsInChildren<CardUI>().ToList();
            RectTransform = GetComponent<RectTransform>();

            Vector3[] v = new Vector3[4];
            RectTransform.GetWorldCorners(v);

            StartPos = v[0];
            EndPos = v[2];
        }


        public void AddChild(CardUI cardUI)
        {
            var parent = cardUI.GetComponentInParent<CardLayoutGroup>();

            if(parent)
                parent.RemoveChild(cardUI);

            ChildCards.Add(cardUI);

            cardUI.transform.SetParent(this.RectTransform,true);

            cardUI.transform.localScale = Vector3.one;

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
            ChildCards.RemoveAll((c) => !c );


            int count = ChildCards.Count;

            //should the movements be set in 
            ChildDesiredPositions = new Vector2[count];

            Vector2 middle = RectTransform.position;
            //var bottomLeft = middle - RectTransform.sizeDelta / 2;
            //var topRight = middle + RectTransform.sizeDelta / 2;

            for (int i = 0; i < count; i++)
            {
                ChildDesiredPositions[i] = EvaluatePosition( i);
            }


            for (int i = 0; i < ChildCards.Count; i++)
            {
                var card = ChildCards[i];
                var desiredPos = ChildDesiredPositions[i];

                if(!PointOnly)
                    card.transform.LeanMove(desiredPos, UnityEngine.Random.Range(0.1f, 0.3f));
            }

            // update child as movingToPosition, to only check those cards
        }

        private Vector2 EvaluatePosition(int index)
        {
            if (PointOnly) return RectTransform.position;

            if (index < 0) index = 0;

            int count = ChildCards.Count > 0 ? ChildCards.Count : 1;

            if(count < MinimumPositions)
            {
                var newCount = MinimumPositions % 2 == count % 2 ? MinimumPositions : MinimumPositions -1; 

                index += Mathf.RoundToInt((newCount - count) / 2f);

                count = newCount;
            }
            else if(count > MaximumPositions)
            {
                var newCount = MaximumPositions % 2 == count % 2 ? MaximumPositions : MaximumPositions + 1;

                index += Mathf.RoundToInt((newCount - count) / 2f);

                count = newCount;
            }

            var relativePos = (index + 0.5f) / count;

            Vector2 vector2 = new Vector2(
                Mathf.LerpUnclamped(StartPos.x, EndPos.x, XPosition.Evaluate(relativePos)),
                Mathf.LerpUnclamped(StartPos.y, EndPos.y, YPosition.Evaluate(relativePos)));

            return vector2;
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

        internal Vector2 GetLastPosition()
        {
            return EvaluatePosition(ChildCards.Count-1);
        }
    }

}