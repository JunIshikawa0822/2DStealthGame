
public class ItemSystem : ASystem, IOnUpdate
{
    public override void OnSetUp()
    {
        gameStat.itemFacade = new ItemFacade
        (
            gameStat.player, 
            gameStat.foodDataArray, 
            gameStat.medicineDataArray
        );
    }

    public void OnUpdate()
    {

    }


}
