using System.Diagnostics;
using UnityEngine;
using System;
using UniRx;
using UnityEngine.UI;
public class UISystem : ASystem, IOnUpdate
{
    CompositeDisposable _disposablesByGameCycle;
    public override void OnSetUp()
    {
        ToggleUI(gameStat.ammoText.gameObject, () => false);
        ToggleUI(gameStat.playerHPSlider.gameObject, () => true);

        //PlayerHPの初期化
        gameStat.playerHPSlider.maxValue = gameStat.playerHP.MaxHp;
        gameStat.playerHPSlider.value = gameStat.playerHP.MaxHp;

        _disposablesByGameCycle = new CompositeDisposable();

        SetEvent();
    }

    public void SetEvent()
    {
        gameStat.playerGunsArray.ObserveReplace().Subscribe(x =>
            {
                ToggleUI(gameStat.ammoText.gameObject, () => gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex.Value] == null ? false : true);
            });
    }

    public void OnUpdate()
    {
        gameStat.playerHPSlider.value = gameStat.playerHP.CurrentHp;

        if(gameStat.isInventoryPanelActive)return;
        gameStat.cursorImage.rectTransform.position = gameStat.cursorScreenPosition;

        if(gameStat.ammoText.gameObject.activeSelf == false)return;
        if(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex.Value] != null)
        {
            // UnityEngine.Debug.Log(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex].GetMagazine());
            gameStat.ammoText.text = $"{gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex.Value].Magazine.MagazineRemaining} / {gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex.Value].Magazine.MagazineCapacity}";
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
