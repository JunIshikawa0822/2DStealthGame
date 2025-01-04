using System.Diagnostics;
using UnityEngine;
public class UISystem : ASystem, IOnUpdate
{
    public override void OnSetUp()
    {
        
    }

    public void OnUpdate()
    {
        //Playerの銃がないなら...
        if(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex.Value] != null)
        {
            // UnityEngine.Debug.Log(gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex].GetMagazine());
            gameStat.AmmoText.text = $"{gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex.Value].GetMagazine().MagazineRemaining} / {gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex.Value].GetMagazine().MagazineCapacity}";
        }

        if(gameStat.isInventoryPanelActive)return;
        gameStat.cursorImage.rectTransform.position = gameStat.cursorScreenPosition;
    }
}
