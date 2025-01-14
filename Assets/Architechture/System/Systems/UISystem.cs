using System.Diagnostics;
using UnityEngine;
using System;
using UnityEngine.UI;
public class UISystem : ASystem, IOnUpdate
{
    public override void OnSetUp()
    {
        ToggleUI(gameStat.ammoText.gameObject, () => false);
        ToggleUI(gameStat.playerHPSlider.gameObject, () => true);

        //PlayerHPの初期化
        gameStat.playerHPSlider.maxValue = gameStat.playerHP.MaxHp;
        gameStat.playerHPSlider.value = gameStat.playerHP.MaxHp;

        //gameStat.onSelectGunChange += ToggleAmmoUI;
    }

    public void OnUpdate()
    {
        //gameStat.playerHPSlider.value = gameStat.playerHP.CurrentHp;

        if(gameStat.isInventoryPanelActive)return;
        gameStat.cursorImage.rectTransform.position = gameStat.cursorScreenPosition;

        if(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex] == null)
        {
            if(gameStat.ammoText.gameObject.activeSelf == false)return;

            ToggleUI(gameStat.ammoText.gameObject, () => false);
        }
        else
        {
            gameStat.ammoText.text = $"{gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex].Magazine.MagazineRemaining} / {gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex].Magazine.MagazineCapacity}";

            if(gameStat.ammoText.gameObject.activeSelf == true)return;

            ToggleUI(gameStat.ammoText.gameObject, () => true);
        }
    }

    public void ToggleUI(GameObject uiObject, Func<bool> toggleCondition)
    {
        if (uiObject == null)
        {
            return;
        }

        // 条件に基づいて UI を有効化または無効化
        bool isActive = toggleCondition();
        uiObject.SetActive(isActive);
    }
}
