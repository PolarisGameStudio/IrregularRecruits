using GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CardLayoutGroup : MonoBehaviour
    {
        public Deck.Zone CardZone;
        public bool CardsAreDraggable;
        //whether cards are flipped. TODO: this should be used to decide all flipping in ui and be set from game logic ensure consistency
        public bool HiddenZone;

        [Header("If the layout should stick to one point")]
        public bool PointOnly;
        public AnimationCurve XPosition;
        public AnimationCurve YPosition;

        public int FirstCardRotation;
        public int LastCardRotation;
        private RectTransform RectTransform;

        [Header("The layout is divided into this minimum of position")]
        public int MinimumPositions = 4;
        [Header("If more children than max, the layout will overflow")]
        public int MaximumPositions = 8;

        public CardLayoutGroup TransitionsTo;

        private List<CardUI> ChildCards = new List<CardUI>();

        struct PosAndRotation
        {
            public Vector3 Position;
            public float ZRotation;

            public PosAndRotation(Vector3 pos,float rot = 0f) 
            {
                Position = pos;
                ZRotation = rot;
            }
        }

        private PosAndRotation[] ChildDesiredPositions;
        private Vector2 StartPos, EndPos;

        public static UnityEvent OnSwitchingPlace = new UnityEvent();

        private bool BeenSetup = false;


        private void Setup()
        {
            BeenSetup = true;

            ChildCards = GetComponentsInChildren<CardUI>().ToList();
            RectTransform = GetComponent<RectTransform>();

            Vector3[] v = new Vector3[4];
            RectTransform.GetWorldCorners(v);

            StartPos = v[0];
            EndPos = v[2];
        }



        public void AddChild(CardUI cardUI, int pos)
        {
            if (!BeenSetup)
                Setup();

            //Debug.Log($"{cardUI.name} added to groupe {name}");

            var parent = cardUI.GetComponentInParent<CardLayoutGroup>();

            if(parent)
                parent.RemoveChild(cardUI);

            ChildCards.Insert(pos,cardUI);

            cardUI.transform.SetParent(this.RectTransform,true);

            cardUI.transform.localScale = Vector3.one;

            cardUI.CurrentZoneLayout = this;

            UpdateChildrenPositions();

            if (PointOnly) //move it yourself
                MoveCardToPos(cardUI,EvaluatePosition(0));
        }

        public void RemoveChild(CardUI cardUI)
        {
            ChildCards.Remove(cardUI);

            if(cardUI.CurrentZoneLayout == this)
                cardUI.CurrentZoneLayout = null;

            UpdateChildrenPositions();
        }

        private void UpdateChildrenPositions()
        {
            ChildCards.RemoveAll((c) => !c);

            int count = ChildCards.Count;

            //should the movements be set in 
            ChildDesiredPositions = new PosAndRotation[count];

            Vector2 middle = RectTransform.position;
            //var bottomLeft = middle - RectTransform.sizeDelta / 2;
            //var topRight = middle + RectTransform.sizeDelta / 2;

            for (int i = 0; i < count; i++)
            {
                ChildDesiredPositions[i] = EvaluatePosition(i);
            }

            if(!PointOnly)
                MoveCardsToDesiredPositions();

            // update child as movingToPosition, to only check those cards
        }

        public void MoveCardsToDesiredPositions()
        {
            for (int i = 0; i < ChildCards.Count; i++)
            {
                var card = ChildCards[i];

                if (!card)
                    continue;

                card.LayoutIndex = i;

                var desiredPos = ChildDesiredPositions[i];

                MoveCardToPos(card, desiredPos);

            }

        }

        internal void UpdateOrderFromGame(List<CardUI> cards)
        {
            if (ChildCards.Count != cards.Count && ChildCards.All(cards.Contains)) 
                Debug.LogWarning("Child cards does not match ");

            ChildCards = cards;

            UpdateChildrenPositions();
        }

        private static void MoveCardToPos(CardUI card, PosAndRotation desiredPos)
        {
            card.FrontHolder.transform.LeanRotateZ(desiredPos.ZRotation, Random.Range(0.1f, 0.3f));

            if (card.BeingDragged) return;
                    
            card.transform.LeanMove(desiredPos.Position, 0.3f).setEaseOutExpo();

            //if( !(ChildCards.Count > 8 && PointOnly))
            //if too many cards in deck. don't animate the movement. 
            //if (!(ChildCards.Count > 8 && PointOnly && !card.CurrentZoneLayout))
            //else
            //    card.transform.position = desiredPos.Position;
        }


        private PosAndRotation EvaluatePosition(int index, bool fakeHigherCount = false)
        {
            if (!BeenSetup)
                Setup();

            if (PointOnly) return new PosAndRotation( (Vector2)RectTransform.position + Random.insideUnitCircle *0.1f);

            if (index < 0) index = 0;

            int count =  ChildCards.Count ;

            if (fakeHigherCount)
                count++;

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

            Vector3 vector2 = new Vector3(
                Mathf.LerpUnclamped(StartPos.x, EndPos.x, XPosition.Evaluate(relativePos)),
                Mathf.LerpUnclamped(StartPos.y, EndPos.y, YPosition.Evaluate(relativePos)),
                //-1 to allow for room between the cards and the highlight                 
                -1);

            var rot = Mathf.LerpUnclamped(FirstCardRotation, LastCardRotation, relativePos);

            return new PosAndRotation(vector2,rot);
        }


        //TODO: maybe should just take the dragged card as parameter
        public void UpdateDraggedCardPos()
        {
            var draggedCard = ChildCards.First(c => c.BeingDragged);

            if (!draggedCard) return;

            int index = ChildCards.IndexOf(draggedCard);
            var currentDesiredPos = ChildDesiredPositions[index];

            Vector2 cardPos = draggedCard.transform.position;
            Vector2 thisPos = transform.position;

            var transitionTo = draggedCard.CanTransitionTo;

            //Debug.Log($"Moving {moving} pos: {position}, idx: {index}, layoutpos: {currentDesiredPos}");

            //closer to the transistions to layout
            if(transitionTo != null && ((Vector2)transform.position - cardPos).sqrMagnitude > ((Vector2)transitionTo.transform.position- cardPos).sqrMagnitude)
            {
                draggedCard.CurrentZoneLayout = transitionTo;
                draggedCard.CanTransitionTo = this;
                transitionTo.AddChild(draggedCard,0);
                RemoveChild(draggedCard);

                OnSwitchingPlace.Invoke();

                if(transitionTo.HiddenZone != HiddenZone)
                {
                    var state = transitionTo.HiddenZone ? CardUI.CardState.FaceDown : CardUI.CardState.Battle;

                    StartCoroutine(draggedCard.Flip(state, 0.1f));
                }
            }
            //closer to the before position
            else if (index > 0 && ((Vector2)currentDesiredPos.Position - cardPos).sqrMagnitude > ((Vector2)ChildDesiredPositions[index - 1].Position - cardPos).sqrMagnitude)
            {

                OnSwitchingPlace.Invoke();

                //switch positions
                var ca = ChildCards[index - 1];
                ChildCards[index - 1] = draggedCard;
                ChildCards[index] = ca;


                MoveCardsToDesiredPositions();
            }
            //closer to the after position
            else if (index < ChildCards.Count - 1 && ((Vector2)currentDesiredPos.Position - cardPos).sqrMagnitude > ((Vector2)ChildDesiredPositions[index + 1].Position - cardPos).sqrMagnitude)
            {
                OnSwitchingPlace.Invoke();

                //switch positions
                var ca = ChildCards[index + 1];
                ChildCards[index + 1] = draggedCard;
                ChildCards[index] = ca;

                //Debug.Log($"{draggedCard.name} swithches places UP in {this} to {index + 1}");

                MoveCardsToDesiredPositions();
            }

        }

        internal Vector2 GetFirstNewPosition()
        {
            return EvaluatePosition(0,true).Position;
        }

        internal bool HasChild(CardUI card)
        {
            return ChildCards.Contains(card);
        }
    }

}