
using System.Collections.Generic;

public class ItemFacade
{
    Dictionary<IGunData.CaliberTypes, IFactory> _gunFactoriesDic;
    public ItemFacade(Dictionary<IGunData.CaliberTypes, IFactory> gunFactories, List<IFactory> factories)
    {

    }

    public void ItemUse(A_Item_Data data)
    {
        
    }

    public void ItemInstantiate(A_Item_Data data)
    {
        if(data is IGunData)
        {
            IGunData gunData = data as IGunData;

        }
        else
        {

        }
    }
}
