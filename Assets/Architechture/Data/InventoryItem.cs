
public class InventoryItem : IInventoryItem
{
    public InventoryItem(I_Data_Item data, uint stackNum)
    {
        _itemData = data;
        Direction = IInventoryItem.ItemDir.Down;

        if(stackNum > data.StackableNum)_stackNum = data.StackableNum;
        else _stackNum = stackNum;
    }

    private I_Data_Item _itemData;
    private uint _stackNum;

    public I_Data_Item Data {get => _itemData;}
    public CellNumber Address {get; set;}
    public IInventoryItem.ItemDir Direction {get; set;}

    public uint StackingNum
    {
        get => _stackNum; 
        set => _stackNum = value;
    }
}
