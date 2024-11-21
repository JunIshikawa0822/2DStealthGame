
public class ItemFactories
{
    private Handgun_CreateConcreteFactory handgunFactory;
    private SubMachinegun_CreateConcreteFactory subMachinegunFactory;
    public ItemFactories()
    {
        
    }

    public IItem ItemInstantiate(IObjectData itemData)
    {
        if(itemData is Scriptable_Handgun_Data) return handgunFactory.ObjectInstantiate(itemData);
        if(itemData is Scriptable_SubMachinegun_Data) return subMachinegunFactory.ObjectInstantiate(itemData);

        return null;
    }
}
