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

        GameLogic.Event.OnCombatSetup.AddListener((d, k) => Toggle.isOn = false);
        GameLogic.Event.OnBattleFinished.AddListener(() => Toggle.isOn = true);
    }
}
