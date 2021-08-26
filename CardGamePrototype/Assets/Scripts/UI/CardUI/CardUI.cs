using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Event = GameLogic.Event;

namespace UI
{

    [RequireComponent(typeof(RectTransform))]
    public class CardUI : AbilityHolderUI, IPointerClickHandler, IDragHandler,IEndDragHandler,IBeginDragHandler
    {
        public Creature Creature;

        [Header("UI Refs")]
        public GameObject FrontHolder, CardBackHolder, CardBattleUI;
        public Image[] CardImage;
        public Image RaceInstance;
        //TODO: replace with clickable
        public Image[] AttributeInstances;
        public CardAnimation CardAnimation;
        public TextMeshProUGUI[] AttackText;
        public TextMeshProUGUI[] HealthText;
        public TextMeshProUGUI CRText;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI DescriptionText;
        public TextMeshProUGUI PriceText;
        public Button BuyButton;

        public List<GameObject> InstantiatedObjects = new List<GameObject>();


        //For deck view and the like
        public bool AlwaysFaceUp;
        public bool Interactable = true;
        public class IntEvent : UnityEvent<int> { }
        public readonly IntEvent OnMoveToNextZone = new IntEvent();

        private IntEvent OnPositionChanged = new IntEvent();

        public int AttackValueDisplayed { get; private set; }
        //Not equal to Card.health, since UI may be behind
        public int HealthValueDisplayed { get; private set; }
        public int MaxHealthValueDisplayed { get; private set; }

        private Color ReducedStatsColor = new Color(0.75f, 0.75f, 0.75f);

        //being dragged? maybe change name
        public bool BeingDragged { get; internal set; }
        
        [HideInInspector]
        public CardLayoutGroup CurrentZoneLayout;
        [HideInInspector]
        public CardLayoutGroup CameFromGroup { get; private set; }

        [HideInInspector]
        public CardLayoutGroup CanTransitionTo;
        //which position those the card have in the ui
        [HideInInspector]
        public int LayoutIndex;

        public enum CardState { FaceUp, FaceDown, Battle }

        public class CardUIEvent : UnityEvent<CardUI> { }
        public static CardUIEvent OnUIDestroyed = new CardUIEvent();

        public static UnityEvent OnDragStarted = new UnityEvent();
        public static UnityEvent OnDragEnd = new UnityEvent();
        internal bool HasWard;

        private void Awake()
        {
            ActionsLeftUI.OnActionChanged.AddListener(ActionsChanged);

            OnDragStarted.AddListener(CardAnimation.TurnOffHighlight);
            OnDragEnd.AddListener(()=> ActionsChanged(Battle.PlayerDeck.DeckController.ActionsLeft));
        }

        public void OnDestroy()
        {
            ActionsLeftUI.OnActionChanged.RemoveListener(ActionsChanged);

            OnDragStarted.RemoveListener(CardAnimation.TurnOffHighlight);
            OnDragEnd.RemoveListener(() => ActionsChanged(Battle.PlayerDeck.DeckController.ActionsLeft));

            OnUIDestroyed.Invoke(this);
        }

        public void SetCard(Card c, bool ignoreCurrentState = false)
        {
            UpdateCreature(c.Creature);

            if (ignoreCurrentState)
                UpdateStats(c.Creature.Attack, c.Creature.Health, c.Creature.Health);
            else
                UpdateStats(c.Attack, c.CurrentHealth, c.MaxHealth);
            
            OnMoveToNextZone.AddListener(c.Activate);
            OnPositionChanged.AddListener(c.PositionChanged);
        }
        public void SetCreature(Creature c)
        {
            UpdateCreature(c);
            UpdateStats(c.Attack, c.Health,c.Health);

        }

        public void UpdateCreature(Creature creature)
        {
            if (!creature) return;

            Creature = creature;

            if (String.IsNullOrEmpty(creature.name)) creature.name = creature.ToString();

            foreach (var cardImage in CardImage)
                cardImage.sprite = creature.Image;
            if (NameText)
                NameText.text = creature.name;

            name = creature?.name;

#if UNITY_EDITOR
            if (CRText)
                CRText.text = creature.CR.ToString("N0");
#endif

            InstantiatedObjects.ForEach(DestroyImmediate);
            InstantiatedObjects.Clear();

            if (DescriptionText)
                DescriptionText.text = "";

            if (RaceInstance)
                if (creature.Race)
                {
                    var instance = Instantiate(RaceInstance, RaceInstance.transform.parent);
                    instance.gameObject.SetActive(true);
                    instance.sprite = creature.Race.Shield;

                    InstantiatedObjects.Add(instance.gameObject);
                }


            foreach (var AttributeInstance in AttributeInstances)
            {
                foreach (var a in creature.Traits)
                {
                    var instance = Instantiate(AttributeInstance, AttributeInstance.transform.parent);
                    instance.gameObject.SetActive(true);
                    instance.sprite = a.Icon;

                    if(DescriptionText)
                        DescriptionText.text += $"<b>{a.name}</b>\n";

                    InstantiatedObjects.Add(instance.gameObject);
                }
            }

            foreach (var AttributeInstance in AttributeInstances)
            {
                UpdateSpecialAbility(creature.SpecialAbility, AttributeInstance);
            }
        }

        private void UpdateSpecialAbility(SpecialAbility specialAbility, Image AttributeInstance)
        {
            if (specialAbility is PassiveAbility ability)
            {

                if (DescriptionText)
                    DescriptionText.text += $"{specialAbility.Description(Creature)}\n";

                var instance = Instantiate(AttributeInstance, AttributeInstance.transform.parent);
                instance.gameObject.SetActive(true);
                instance.sprite = IconLibrary.GetAbilityIconSprite(ability.ResultingAction.ActionType);

                InstantiatedObjects.Add(instance.gameObject);

                SpecialAbilityIcon.Add(instance);
            }
            if (specialAbility is EffectDoublerAbility doubler)
            {
                if (DescriptionText)
                    DescriptionText.text += $"{specialAbility.Description(Creature)}\n";

                var instance = Instantiate(AttributeInstance, AttributeInstance.transform.parent);
                instance.gameObject.SetActive(true);
                instance.sprite = IconLibrary.GetAbilityIconSprite(doubler.Effect);

                InstantiatedObjects.Add(instance.gameObject);

                SpecialAbilityIcon.Add(instance);
            }
            if (specialAbility is TriggerDoublerAbility doubleTrigger)
            {
                if (DescriptionText)
                    DescriptionText.text += $"{specialAbility.Description(Creature)}\n";

                var instance = Instantiate(AttributeInstance, AttributeInstance.transform.parent);
                instance.gameObject.SetActive(true);
                instance.sprite = IconLibrary.GetAbilityIconSprite(doubleTrigger.EffectTrigger);

                InstantiatedObjects.Add(instance.gameObject);

                SpecialAbilityIcon.Add(instance);
            }
        }

        public void StatModifier(int amount)
        {
            if (amount > 0)
                CardAnimation.StatPlusAnimation.Show(amount);
            else if (amount < 0)
                CardAnimation.StatMinusAnimation.Show(amount);
        }

        private void UpdateStats(int attack, int health, int maxHealth)
        {
            if (!Creature) return;

            UpdateHealth(health,  maxHealth);
            UpdateAttack(attack);
        }

        public void UpdateAttack(int attack)
        {
            AttackValueDisplayed = attack;

            foreach (var a in AttackText)
            {
                a.text = attack.ToString("N0");

                a.color = Creature.Attack < attack ? Color.green :
                    attack < Creature.Attack ? ReducedStatsColor :
                    Color.white;
            }
        }

        public void UpdateHealth(int health, int maxHealth)
        {
            HealthValueDisplayed = health;
            MaxHealthValueDisplayed = maxHealth;

            var damaged = maxHealth > health;

            foreach (var h in HealthText)
            {
                h.text = health.ToString("N0");

                h.color = damaged ? Color.red :
                    health > Creature.Health ? Color.green :
                    health < Creature.Health ? Color.gray :
                    Color.white;
            }
        }

        public CardState GetCardState()
        {
            if (FrontHolder && FrontHolder.activeInHierarchy)
                return CardState.FaceUp;
            else if (CardBattleUI && CardBattleUI.activeInHierarchy)
                return CardState.Battle;

            return CardState.FaceDown;
        }

        //Checks whether the card should highlight or not
        private void ActionsChanged(int actionsLeft)
        {
            if (IsDraggable() && actionsLeft > 0)
                CardAnimation.Highlight();
            else
                CardAnimation.TurnOffHighlight();
        }

        private bool IsDraggable()
        {
            return CurrentZoneLayout && CurrentZoneLayout.CardsAreDraggable && !BattleUI.Instance.UILocked;
        }


        #region Input Handling
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!BeingDragged && (AlwaysFaceUp || IsClickable()))
            {
                CardHoverInfo.Show(this);
            }
        }

        private bool IsClickable()
        {
            return GetCardState() == CardState.FaceUp || GetCardState() == CardState.Battle;
        }


        public void OnDrag(PointerEventData eventData)
        {
            if (!BeingDragged) //if the card is not allowed to be dragged
                return;

            transform.position = eventData.pointerCurrentRaycast.worldPosition;

            CurrentZoneLayout.UpdateDraggedCardPos();

            if (CurrentZoneLayout.HiddenZone)
                transform.SetAsFirstSibling();
            else
                transform.SetAsLastSibling();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            CurrentZoneLayout = GetComponentInParent<CardLayoutGroup>();
            CameFromGroup = CurrentZoneLayout;

            CanTransitionTo = CurrentZoneLayout?.TransitionsTo;

            if (IsDraggable())
            {
                OnDragStarted.Invoke();
                BeingDragged = true;
                //AnimationSystem.OnCreatureExclamation.Invoke(this, CreatureBark.Grunt);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!BeingDragged)
                return;

            OnDragEnd.Invoke();

            BeingDragged = false;

            //if we have moved to the other layout group
            if (CameFromGroup != CurrentZoneLayout)
            {
                //Debug.Log($"{name} dragged to {CurrentZoneLayout.CardZone} at pos {LayoutIndex}");
                OnMoveToNextZone.Invoke(LayoutIndex);
            }
            else
                OnPositionChanged.Invoke(LayoutIndex);

            CanTransitionTo = null;

            //move it into place
            CurrentZoneLayout.MoveCardsToDesiredPositions();
        }
        #endregion

        internal IEnumerator Flip(CardState state, float flipTime = 0.2f)
        {
            
            //already correct face up
            if (state == GetCardState()) 
                yield break;

            if (AlwaysFaceUp) yield break;

            bool flipping = GetCardState() == CardState.FaceDown || state == CardState.FaceDown;

            if (flipping)
            {
                gameObject.LeanScaleX(0, flipTime);

                yield return new WaitForSeconds(flipTime);


                CardBackHolder.SetActive(state == CardState.FaceDown);
                CardBattleUI.SetActive(state == CardState.Battle);
                FrontHolder.SetActive(state == CardState.FaceUp);

                FrontHolder.GetComponent<CanvasGroup>().alpha = 1;

                gameObject.LeanScaleX(1, flipTime);

                yield return new WaitForSeconds(flipTime);
            }
            else
            {
                var from = GetCardState() == CardState.Battle ? CardBattleUI : FrontHolder;
                var to = state == CardState.FaceUp ? FrontHolder : CardBattleUI;

                to.SetActive(true);

                StartCoroutine(Fade(to.GetComponent<CanvasGroup>(), true, flipTime));
                StartCoroutine(Fade(from.GetComponent<CanvasGroup>(), false, flipTime));

                yield return new WaitForSeconds(flipTime);

                from.SetActive(false);

            }

            CardBackHolder.SetActive(state == CardState.FaceDown);
            CardBattleUI.SetActive(state == CardState.Battle);
            FrontHolder.SetActive(state == CardState.FaceUp);
        }

        private IEnumerator Fade(CanvasGroup canvasGroup, bool fadeIn, float duration)
        {
            var endTime = Time.time + duration;

            if (fadeIn)
            {
                while (Time.time < endTime)
                {
                    canvasGroup.alpha = 1f - (endTime - Time.time) / duration;
                    yield return null;
                }
            }
            else
            {
                while (Time.time < endTime)
                {
                    canvasGroup.alpha = (endTime - Time.time) / duration; 
                    yield return null;
                }
            }
        }

    }
}
