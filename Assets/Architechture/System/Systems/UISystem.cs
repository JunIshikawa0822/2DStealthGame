using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : ASystem, IOnUpdate
{
    public override void OnSetUp()
    {
        
    }

    public void OnUpdate()
    {
        //Playerの銃がないなら...
        if(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex] != null)
        {
            // UnityEngine.Debug.Log(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex].GetMagazine());
            gameStat.AmmoText.text = $"{gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex].GetMagazine().MagazineRemaining} / {gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex].GetMagazine().MagazineCapacity}";
        }

        if(gameStat.isInventoryPanelActive)return;
        gameStat.cursorImage.rectTransform.position = gameStat.cursorScreenPosition;
    }

    public void ToggleUI<T>(T uiElement) where T : Component
    {
        if (uiElement == null)
        {
            return;
        }

        GameObject targetGameObject = uiElement.gameObject;
        bool currentState = targetGameObject.activeSelf;
        targetGameObject.SetActive(!currentState);
    }
}
