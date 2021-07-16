using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatHasComputedCheck : MonoBehaviour
{
    private Toggle Toggle;
    
    void Start()
    {
        Toggle = GetComponent<Toggle>();

        GameLogic.Event.OnCombatStart.AddListener(() => Toggle.isOn = false);
        GameLogic.Event.OnBattleFinished.AddListener((d,l) => Toggle.isOn = true);
    }
}
