public class UISystem : ASystem, IOnUpdate
{
    public override void OnSetUp()
    {
        
    }

    public void OnUpdate()
    {
        gameStat.AmmoText.text = $"{gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex].GetMagazine().MagazineRemaining} / {gameStat.playerGunsArray[gameStat.selectingGunsArrayIndex].GetMagazine().MagazineCapacity}";

        if(gameStat.isInventoryPanelActive)return;
        gameStat.cursorImage.rectTransform.position = gameStat.cursorScreenPosition;
    }
}
