using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using GameLogic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI
{
    public class CardEditor : Singleton<CardEditor>
    {
#if UNITY_EDITOR
        public GameObject Holder;
        public TMP_Dropdown RaceDropdown;
        public Image RaceImage;
        public Image CardPortrait;
        public TMP_InputField AttackInput, HealthInput, NameInput;
        public Button SaveButton;
        private List<Race> Races;
        private List<Trait> Traits;
        private List<Creature> Creatures;
        private Creature Creature;
        public AbilityEditorEntry AbilityEditor;
        public TraitEditorInstance TraitEditorInstance;
        private List<TraitEditorInstance> TraitObjects = new List<TraitEditorInstance>();

        private void Awake()
        {
            Holder.SetActive(false);

            Races = AssetManager.GetAssetsOfType<Race>();
            Traits = AssetManager.GetAssetsOfType<Trait>();

            SaveButton.onClick.AddListener(Save);
            AttackInput.onEndEdit.AddListener(UpdateAttack);
            HealthInput.onEndEdit.AddListener(UpdateHealth);
            NameInput.onEndEdit.AddListener(UpdateName);

            RaceDropdown.options = Races.Select(r => new TMP_Dropdown.OptionData(r.name, r.Icon)).ToList();

            RaceDropdown.onValueChanged.AddListener(ChangeRace);

            AbilityEditor.AddRandomTraitButton.onClick.AddListener(AddRandomAbility);
            AbilityEditor.MoveTraitToOtherButton.onClick.AddListener(MoveAbilityToOtherCreature);
            AbilityEditor.DeleteTraitButton.onClick.AddListener(DeleteAbility);

            TraitEditorInstance.AddTraitButton.onClick.AddListener(() => AddTrait());
        }

        private void AddTrait(Trait t = null)
        {
            var entry = Instantiate(TraitEditorInstance, TraitEditorInstance.transform.parent);


            entry.RemoveTraitButton.onClick.AddListener(() => RemoveTrait(entry));
            entry.ChangeTraitButton.onClick.AddListener(() => ChangeTrait(entry));
            entry.RemoveTraitButton.gameObject.SetActive(true);
            entry.ChangeTraitButton.gameObject.SetActive(true);
            entry.AddTraitButton.enabled = false;

            if (t)
            {
                entry.UpdateTrait(t);
            }
            else
                ChangeTrait(entry);

            TraitObjects.Add(entry);

            TraitEditorInstance.transform.SetAsLastSibling();
        }

        private void RemoveTrait(TraitEditorInstance entry)
        {
            Creature.Traits.Remove(entry.Trait);

            EditorUtility.SetDirty(Creature);

            DeleteTraitEntry(entry);
        }

        private void DeleteTraitEntry(TraitEditorInstance entry)
        {
            TraitObjects.Remove(entry);
            Destroy(entry.gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && CardHoverInfo.IsActive())
                Open(CardHoverInfo.GetCreature());
        }

        private void MoveAbilityToOtherCreature()
        {
            if (!(Creature.SpecialAbility is PassiveAbility ability)) return;

            AssetManager.MoveAbilityToOtherCreature(ability);
            Creature.SpecialAbility = null;

            UpdateCreature(Creature);

            EditorUtility.SetDirty(Creature);
        }

        private void DeleteAbility()
        {
            DestroyImmediate(Creature.SpecialAbility, true);

            Creature.SpecialAbility = null;

            UpdateCreature(Creature);

            EditorUtility.SetDirty(Creature);
        }

        private void AddRandomAbility()
        {
            AssetManager.GenerateAbilityForCreature(Creature);

            UpdateCreature(Creature);

            EditorUtility.SetDirty(Creature);
        }

        private void ChangeRace(int arg0)
        {
            var race = Races[arg0];

            RaceImage.sprite = race.Icon;

            if (!Creature || race == Creature.Race) return;
            Debug.Log("updating Race for " + Creature + " to " + race);
            Creature.Race = race;
            EditorUtility.SetDirty(Creature);
        }

        public void ChangeTrait(TraitEditorInstance traitEditorInstance)
        {
            Trait oldTrait = traitEditorInstance.Trait;
            Trait newTrait = oldTrait;

            do
            {
                var index = Traits.IndexOf(newTrait) + 1;
                if (index < 0) index = 0;
                if (index >= Traits.Count) index = 0;
                newTrait = Traits[index];
            }
            while (Creature.Traits.Contains(newTrait));

            traitEditorInstance.UpdateTrait(newTrait);
            Creature.Traits.Remove(oldTrait);
            Creature.Traits.Add(newTrait);

            EditorUtility.SetDirty(Creature);
        }

        private void Open(Creature creature)
        {
            Instance.Holder.SetActive(true);
            UpdateCreature(creature);
        }
        private void UpdateCreature(Creature creature)
        {
            AttackInput.text = creature.Attack.ToString("N0");
            HealthInput.text = creature.Health.ToString("N0");
            NameInput.text = creature.name;
            CardPortrait.sprite = creature.Image;
            RaceDropdown.value = Races.IndexOf(creature.Race);
            Creature = creature;

            while (TraitObjects.Any())
                DeleteTraitEntry(TraitObjects.First());

            foreach (var t in Creature.Traits)
                AddTrait(t);

            UpdateAbility();
        }

        private void UpdateAttack(string input)
        {
            Debug.Log("updating attack for " + Creature + " to " + input);

            Creature.Attack = int.Parse(input);

            EditorUtility.SetDirty(Creature);
        }
        private void UpdateHealth(string input)
        {
            Debug.Log("updating health for " + Creature + " to " + input);

            Creature.Health = int.Parse(input);

            EditorUtility.SetDirty(Creature);
        }
        private void UpdateName(string input)
        {
            Debug.Log("updating name for " + Creature + " to " + input);

            Creature.name = input;

            EditorUtility.SetDirty(Creature);
        }

        private void UpdateAbility()
        {
            AbilityEditor.Image.enabled = Creature.SpecialAbility;
            if (Creature.SpecialAbility is PassiveAbility ability)
            {
                AbilityEditor.Image.sprite = IconLibrary.GetAbilityIconSprite(ability.ResultingAction.ActionType);
                AbilityEditor.Text.text = Creature.SpecialAbility.Description(Creature);
            }
            else
            {
                AbilityEditor.Text.text = "no special ability";
            }

        }

        private void Save()
        {
            EditorUtility.SetDirty(Creature);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            //foreach (var cardUI in FindObjectsOfType<CardUI>())
            //    if (cardUI.Card.Creature == Creature)
            //        cardUI.Card.SetCreature(Creature);

            Creature = null;

            Holder.SetActive(false);
        }

#endif
    }
}