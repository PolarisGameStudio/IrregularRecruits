using MapLogic;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class OpenShopButton : Button
{
    protected override void Start()
    {
        onClick.AddListener(ShopUI.OpenStandardShop);
    }
}
