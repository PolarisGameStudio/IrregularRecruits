using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardHighlight : Singleton<CardHighlight>
{
    public TextMeshProUGUI CardTitleText;
    public TextMeshProUGUI AttackText;
    public TextMeshProUGUI HealthText;
    public ImageTextEntry RaceIcon;
    public ImageTextEntry AbilityIcon;
    public ImageTextEntry TraitPrefab;
    public Image ImageMask, Image;
    public static Coroutine ShowAfterDelayRoutine;

    private List<ImageTextEntry> InstantiatedObjects = new List<ImageTextEntry>();

    public GameObject Holder;

    private void Start()
    {
        Hide();
    }

    public static bool IsActive()
    {
        return Instance.Holder.activeSelf;
    }

    //DISABLE mask if creature has small image
    //remember color the same as health and attack of card
    public static void Show(CardUI card)
    {
        if(ShowAfterDelayRoutine == null)
            ShowAfterDelayRoutine = Instance.StartCoroutine( Instance.ShowAfterDelay(card));
    }

    private IEnumerator ShowAfterDelay(CardUI card)
    {
        yield return new WaitForSeconds(0.3f);

        ShowCard(card);
    }

    private void ShowCard(CardUI cardUI)
    {
        ShowAfterDelayRoutine = null;

        InstantiatedObjects.ForEach(t => Destroy(t.gameObject));

        InstantiatedObjects.Clear();

        var card = cardUI.Card;

        //public TextMeshProUGUI CardTitleText;
        CardTitleText.text = card.Name;


        //public Image ImageMask, Image;
        if(card.Creature.IconImage)
        {
            Image.sprite = card.Creature.IconImage;
            ImageMask.enabled = false;
        }
        else
        {
            Image.sprite = card.Creature.Image;
            ImageMask.enabled = true;
        }

        //public TextMeshProUGUI AttackText;
        AttackText.text = cardUI.AttackText.text;
        AttackText.color = cardUI.AttackText.color;

        //public TextMeshProUGUI HealthText;
        HealthText.text = cardUI.HealthText.text + "/" + card.MaxHealth.ToString("N0");
        HealthText.color = cardUI.HealthText.color;

        //public ImageTextEntry RaceIcon;
        RaceIcon.Text.text = card.Creature.Race.name;
        RaceIcon.Image.sprite = card.Creature.Race.Icon;

        //public ImageTextEntry AbilityIcon;
        AbilityIcon.gameObject.SetActive(card.Creature.SpecialAbility);

        AbilityIcon.gameObject.SetActive(card.Creature.SpecialAbility);

        if (card.Creature.SpecialAbility) {
            AbilityIcon.Text.text = card.Creature.SpecialAbility?.Description(card);
            AbilityIcon.Image.sprite = IconManager.GetAbilityIconSprite(card.Creature.SpecialAbility.ResultingAction.ActionType);
        }

        //public ImageTextEntry TraitPrefab;
        foreach(var t in card.Creature.Traits)
        {
            var trait = Instantiate(TraitPrefab, TraitPrefab.transform.parent);
            trait.Text.text = $" <b>{t.name}</b> <i>({t.Description})</i>";
            trait.gameObject.SetActive(true);
            trait.Image.sprite = t.Icon;

            InstantiatedObjects.Add(trait);
        }

        var rect = GetComponent<RectTransform>();
        rect.position = cardUI.GetComponent<RectTransform>().position;

        Instance.Holder.transform.localScale = Vector3.zero;

        LeanTween.scale(Instance.Holder, Vector3.one, 0.15f);

        Instance.Holder.SetActive(true);
    }

    public static void Hide()
    {
        Instance.Holder.SetActive(false);

        if (ShowAfterDelayRoutine != null)
        {
            Instance.StopCoroutine(ShowAfterDelayRoutine);
            ShowAfterDelayRoutine = null;
        }
    }
}
