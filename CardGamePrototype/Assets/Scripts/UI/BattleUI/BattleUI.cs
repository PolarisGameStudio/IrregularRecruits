using GameLogic;
using MapLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Event = GameLogic.Event;
using Random = UnityEngine.Random;


namespace UI
{

    /// <summary>
    /// Responsible for handling all ui during combat
    /// </summary>
    public class BattleUI : Singleton<BattleUI>, IUIWindow
    {
        public CardUI CardPrefab;

        private static Dictionary<Guid, AbilityHolderUI> CardUIs = new Dictionary<Guid, AbilityHolderUI>();

        public UIZone[] PlayerUIZones;
        public UIZone[] EnemyUIZones;

        public Button ViewPlayerDeckButton;

        [Serializable]
        public struct UIZone
        {
            public Deck.Zone Zone;
            public RectTransform RectTransform;
            public CardLayoutGroup CardLayout;
            public int CardRotation;
            public float CardPosAdjust;
        }

        public TextMeshProUGUI PlayerDeckDescription, EnemyDeckDescription;

        [Header("Movement")]
        public float MoveDuration = 0.5f;
        public AnimationCurve MoveAnimationCurve;

        private CardUI Attacker;
        private CardUI AttackTarget;
        public Deck PlayerDeck;
        public Deck EnemyDeck;
        internal HashSet<CardAnimation> WardOnBattlefield;

        public List<Card> InitialEnemyDeck;
        public List<Card> InitialPlayerDeck;

        //todo: move ´this into seperate class
        public int XpAtStartOfBattle;
        public int GoldAtStartOfBattle;

        public static UnityEvent OnBattleFinished = new UnityEvent();
        public static Event.FactionEvent OnBattleBegin = new Event.FactionEvent();

        //TODO: move these to animation system
        public class AbEvent : UnityEvent<AbilityWithEffect> { }
        public static AbEvent OnAbilitySelect = new AbEvent();


        public static UnityEvent OnLevelUp = new UnityEvent();
        public static UnityEvent OnLevelAnimation = new UnityEvent();

        public Button EndTurnButton;
        public Image BackgroundImage;


        public CanvasGroup FocusCanvas;


        [HideInInspector]
        public bool UILocked;

        public bool BattleRunning { get; private set; }

        void Awake()
        {
            ViewPlayerDeckButton.onClick.AddListener(() => DeckViewerUI.View(Battle.PlayerDeck));

            Event.OnCombatStart.AddListener(SetupDecks);

            EndTurnButton.onClick.AddListener(EndPlayerTurn);

            //todo could probably be done nicer. use getzoneholder maybe? and remove .CardZone
            foreach (var c in PlayerUIZones)
                c.CardLayout.CardZone = c.Zone;
            foreach (var c in EnemyUIZones)
                c.CardLayout.CardZone = c.Zone;

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) & !UILocked)
                EndTurnButton.onClick.Invoke();
        }

        private void EndPlayerTurn()
        {
            if (PlayerDeck.DeckController is PlayerController controller)
                controller.FinishTurn();
        }

        private void SetupDecks()
        {

            PlayerDeck = Battle.PlayerDeck;
            EnemyDeck = Battle.EnemyDeck;

            MoveDuration = GameSettings.Speed() / 2;

            SetupUI(PlayerDeck,true);
            SetupUI(EnemyDeck,false);

            if(PlayerDeck.Hero != null)
            {
                HeroUI.Instance.SetHero(PlayerDeck.Hero);

                XpAtStartOfBattle = PlayerDeck.Hero.Xp;
                GoldAtStartOfBattle = Map.PlayerGold;
            }


            InitialPlayerDeck =  PlayerDeck.AllCreatures();
            InitialEnemyDeck = EnemyDeck.AllCreatures();


            Race enmRace = EnemyDeck.Races.First();

            BackgroundImage.sprite = enmRace.GetBackground();

            WardOnBattlefield = new HashSet<CardAnimation>();

            BattleRunning = true;
            OnBattleBegin.Invoke(enmRace);
        }

        internal void ResetWards()
        {
            foreach (var w in WardOnBattlefield)
                w.SetWarded(true,false);
        }

        private void SetupUI(Deck deck,bool playerDeck)
        {
            foreach (var card in deck.AllCreatures())
            {
                CreateCardUI(card,playerDeck);
            }

            if (deck.Hero != null)
            {
                CardUIs[deck.Hero.Guid] = HeroUI.Instance;
            }
            else if (playerDeck)
                HeroUI.Instance?.Disable();
        }

        private void CreateCardUI(Card card, bool playerDeck, bool summon = false)
        {
            var ui = Instantiate(Instance.CardPrefab,transform);

            ui.HasWard = card.Ward();

            if (!ui.HasWard)
                ui.CardAnimation.DestroyWardAni();

            GetZoneHolder(card.Location, !playerDeck).AddChild(ui,0);

            ui.SetCard(card,summon);

            CardUIs[card.Guid] = ui;

            StartCoroutine(ui.Flip(CardUI.CardState.FaceDown, 0f));

            if(summon)
                AnimationSystem.Instance.SummonParticles(ui);

            //Should we just move it to library nomatter what?
            Deck.Zone destination = summon ? Deck.Zone.Battlefield :  card.Location;
            StartCoroutine(MoveCard(ui, destination, playerDeck,true));
        }

        public static IEnumerator CleanUpUI(Deck winner)
        {
            yield return null;
            Instance.EndBattle(winner);
        }

        public static IEnumerator Summon(Card summon,bool playerdekc)
        {
            Instance.CreateCardUI(summon, playerdekc,true);
            
            yield return new WaitForSeconds(1f);
        }
        public static IEnumerator UnSummon(Guid summon)
        {
            CardUI ui = GetCardUI(summon);

            if (!ui) yield break;

            //TODO: should the effect be bfore or after
            yield return AnimationSystem.UnsummonFx(ui);

            Destroy(CardUIs[summon].gameObject);
        }

        internal static IEnumerator WardedAttack(Guid guid)
        {
            CardUI ui = GetCardUI(guid);

            if (!ui) yield break;

            
            yield return AnimationSystem.Instance.WardPopParticles(ui);
        }


        private void EndBattle(Deck winner)
        {
            foreach (var kp in CardUIs)
            {
                if (kp.Value is HeroUI || !kp.Value)
                    continue;

                Destroy(kp.Value.gameObject);
            }

            CardUIs.Clear();
            BattleRunning = false;

            OnBattleFinished.Invoke();

            var endXp = PlayerDeck.Hero != null ? PlayerDeck.Hero. Xp : 0;


            if (winner != PlayerDeck)
            {
                Event.OnGameOver.Invoke();
                return;
            }
            else
                BattleSummary.ShowSummary(InitialPlayerDeck, InitialEnemyDeck, PlayerDeck.AllCreatures(), EnemyDeck.AllCreatures(),XpAtStartOfBattle,endXp,PlayerDeck.Hero,GoldAtStartOfBattle,Map.PlayerGold);
            
        }

        private CardLayoutGroup GetZoneHolder(Deck.Zone zone, bool enm)
        {
            var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

            return z.FirstOrDefault(u => u.Zone == zone).CardLayout;
        }

        private float GetZoneAdjust(Deck.Zone zone, bool enm)
        {
            var z = enm ? EnemyUIZones : PlayerUIZones;

            return z.FirstOrDefault(u => u.Zone == zone).CardPosAdjust;
        }

        //Handles death/ etb / withdraw / resurrection / draw animation
        internal static IEnumerator Move(Guid card, Deck.Zone to, Deck.Zone from, bool playerDeck)
        {
            yield return Instance.MoveCard(card, to, from, playerDeck);
        }

        private IEnumerator MoveCard(Guid card, Deck.Zone to, Deck.Zone from, bool playerDeck)
        {
            CardUI ui = GetCardUI(card);

            if (!ui) yield break;

            //Battlefield effects after move
            if(to != Deck.Zone.Battlefield)
                yield return AnimationSystem.ZoneMoveEffects(ui, from, to);

            yield return MoveCard(ui, to, playerDeck);

            ui.CardAnimation.SetWarded(ui.HasWard && to == Deck.Zone.Battlefield,true);

            if (to == Deck.Zone.Battlefield)
                yield return AnimationSystem.ZoneMoveEffects(ui, from, to);

        }

        internal static IEnumerator AbilityTriggered(SpecialAbility a, Guid guid, IEnumerable<Guid> ts)
        {
            var ui = GetAbilityHolderUI(guid);

            if (!ui) yield break;

            if(a is AbilityWithEffect ae)
                yield return AnimationSystem.Instance.PlayAbilityFx(ae, ui, ts.Select(GetCardUI).ToList(), 0.25f);
            else
                yield return AnimationSystem.Instance.PlayDoublerFx(a, ui, 0.25f);


        }

        private static CardUI GetCardUI(Guid cardGuid)
        {
            if (!CardUIs.ContainsKey(cardGuid))
            {
                //Debug.LogError("ui for guid not instantiated");
                return null;
            }

            var ui = CardUIs[cardGuid];

            if (ui is CardUI)
                return ui as CardUI;
            else
                return null;
        }
        
        private static AbilityHolderUI GetAbilityHolderUI(Guid guid)
        {
            if (!CardUIs.ContainsKey(guid))
            {
                //Debug.LogError("ui for guid not instantiated");
                return null;
            }

            var ui = CardUIs[guid];
            return ui;
        }

        internal static IEnumerator SetAttacker(Guid card)
        {
            CardUI ui = GetCardUI(card);

            if (!ui) yield break;

            //Hack to avoid it from being moved by the layout group
            ui.BeingDragged = true;

            Instance.Attacker = ui;

            yield return AnimationSystem.StartAttack(ui);

            //do ready attack animation
        }

        internal static IEnumerator SetAttackTarget(Guid card)
        {
            CardUI ui = GetCardUI(card);

            if (!ui) yield break;

            Instance.AttackTarget = ui;

            ui.transform.LeanScale(Vector3.one, 0.5f * GameSettings.Speed());

            yield return Instance.AttackAnimation();
        }

        internal static IEnumerator PullBackAttacker(Guid guid)
        {
            CardUI ui = GetCardUI(guid);

            if (!ui ) yield break;

            ui.BeingDragged = false;

            ui.CurrentZoneLayout.MoveCardsToDesiredPositions();

            float seconds = 0.5f * GameSettings.Speed();

            ui.transform.LeanScale(Vector3.one, seconds);

            if(Instance.AttackTarget != null)
                Instance.AttackTarget.transform.LeanScale(Vector3.one, seconds);

            yield return new WaitForSeconds(seconds);

            Instance.AttackTarget = Instance.Attacker = null;
        }

        private IEnumerator AttackAnimation()
        {
            if (AttackTarget.GetCardState() == CardUI.CardState.FaceDown)
            {
                AttackTarget.transform.SetAsLastSibling();
            }

            yield return (AnimationSystem.AttackAnimation(Attacker, AttackTarget, 1f));

        }

        //negative for damage, positive for heal
        internal static IEnumerator CardHealthChange(Guid card, int val, int currentHealth, int maxHealth)
        {
            CardUI ui = GetCardUI(card);

            if (!ui) yield break;

            if (val < 0)
            {
                ui.CardAnimation.DamageAnimation.Show(val);
                AnimationSystem.Instance.OnDamaged.Invoke();
            }
            else if (val > 0)
            {
                //AnimationSystem.OnHeal.Invoke();
                //handled by CardHeal Routine
            }
            else
            {
                //Debug.LogWarning("health change of 0");
            }
            ui.UpdateHealth(currentHealth,  maxHealth);

            yield return null;
        }


        internal static IEnumerator CardHeal(Guid guid, int val, int currentHealth, int maxHealth)
        {
            CardUI ui = GetCardUI(guid);

            if (!ui) yield break;

            if (val <= 0)
            {

                //Debug.LogError("Card heal with value "+ val);
            }
            else if (val > 0)
            {
                AnimationSystem.Instance.OnHeal.Invoke();
                ui.CardAnimation.HealAnimation.Show(val);
                //handled by CardHeal Routine
            }
            else

            ui.UpdateHealth(currentHealth, maxHealth);

            yield return null;
        }


        internal static IEnumerator CardStatsModified(Guid card, int val, int currentHealth, int currentAttack, int maxHealth)
        {
            CardUI ui = GetCardUI(card);

            if (!ui) yield break;

            ui.StatModifier(val);

            ui.UpdateHealth(currentHealth, maxHealth);
            ui.UpdateAttack(currentAttack);
            yield return null;
        }

        public static int GetZoneRotation(Deck.Zone zone, bool enm)
        {
            var z = enm ? Instance.EnemyUIZones : Instance.PlayerUIZones;

            return z.FirstOrDefault(u => u.Zone == zone).CardRotation;
        }

        public static void UpdateLibrary()
        {
            //Instance.PlayerDeckDescription.text = "Deck size: " + BattleManager.PlayerDeck.CreaturesInZone(Deck.Zone.Library).Count;
            //Instance.EnemyDeckDescription.text = "Deck size: " + BattleManager.EnemyDeck.CreaturesInZone(Deck.Zone.Library).Count;
        }

        //otherwise make an onclick event in CardUI

        private IEnumerator MoveCard(CardUI card, Deck.Zone zone, bool player, bool instantly = false, bool wardOn = false)
        {
            if (!card) yield break;

            //Debug.Log($"Moving {card} to {zone}");

            if (zone == Deck.Zone.Graveyard)
            {
                yield return card.Flip(CardUI.CardState.Battle);
                yield return new WaitForSeconds(0.4f);
            }
            var zoneHolder = GetZoneHolder(zone, !player);

            var fliptime = instantly ? 0f : 0.2f;

            switch (zone)
            {
                case Deck.Zone.Library:
                    StartCoroutine(card.Flip(CardUI.CardState.FaceDown,fliptime));
                    break;
                case Deck.Zone.Battlefield:
                    StartCoroutine(card.Flip(CardUI.CardState.Battle,fliptime));
                    break;
                case Deck.Zone.Hand:
                    if (!player)
                        StartCoroutine(card.Flip(CardUI.CardState.FaceDown, fliptime));
                    else
                        StartCoroutine(card.Flip(CardUI.CardState.FaceUp,fliptime));
                    break;
                default:
                    StartCoroutine(card.Flip(CardUI.CardState.FaceUp,fliptime));
                    break;
            }

            //if not already dragged there
            if (!zoneHolder.HasChild(card))
            {
                var rect = card.GetComponent<RectTransform>();
                Vector2 startPos = rect.position;

                if (!zoneHolder) yield break;

                var startTime = Time.time;
                float posAdjust = GetZoneAdjust(zone, !player);
                var rot = GetZoneRotation(zone, !player);
                Vector2 endPosition = zoneHolder.GetFirstNewPosition();

                endPosition += new Vector2(Random.Range(-posAdjust, posAdjust), Random.Range(-posAdjust, posAdjust));

                rect.Rotate(new Vector3(0, 0, Random.Range(-rot, rot)));

                var adjustDirection = (startPos - endPosition);

                adjustDirection = new Vector2(adjustDirection.y, -adjustDirection.x).normalized;


                while (!instantly && Time.time < startTime + MoveDuration)
                {
                    yield return null;

                    float t = (Time.time - startTime) / MoveDuration;

                    rect.position = Vector3.LerpUnclamped(startPos, endPosition, t);

                    rect.position += (Vector3)(MoveAnimationCurve.Evaluate(t) * adjustDirection);

                }

                zoneHolder.AddChild(card,0);
            }

            card.Interactable = true;
        }

        public CanvasGroup GetCanvasGroup()
        {
            return FocusCanvas;
        }

        public GameObject GetHolder() => null;
        public int GetPriority() => 0;
    }

}