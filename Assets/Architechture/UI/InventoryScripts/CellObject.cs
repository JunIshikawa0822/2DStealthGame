public class CellObject
{
    //private Vector3 cellObjectPosition;
    //private Grid<CellObject> grid;
    private int position_x;
    private int position_y;

    private PlacedObject placedObject;

    private int stackNum;

    //private bool isStackableOnCell;

    public CellObject(/*Grid<CellObject> grid,*/ int x, int y) 
    {
        //this.grid = grid;
        position_x = x;
        position_y = y;
        //cellObjectPosition = worldPosition;
    }

    public void SetPlacedObject(PlacedObject placedObject)
    {
        this.placedObject = placedObject;
    }

    public PlacedObject GetPlacedObject()
    {
        return placedObject;
    }

    public int GetStackNum()
    {
        return stackNum;
    }

    public bool CanInsertToCellObject(PlacedObject placedObject)
    {
        bool canInsert = false;

        if(this.placedObject == null)
        {
            canInsert = true;
        }
        else if(this.placedObject.GetItemData() == placedObject.GetItemData())
        {
            if(placedObject.GetItemData().stackableNum > this.stackNum)
            {
                canInsert = true;
            }
        }   

        return canInsert;
    }

    public void InsertToCellObject(PlacedObject placedObject, int insertNum)
    {
        if(this.placedObject == null)
        {
            this.placedObject = placedObject;
            stackNum = 1;
        }
        else
        {
           if(this.placedObject.GetItemData() != placedObject.GetItemData()) return;

           if(placedObject.GetItemData().stackableNum > this.stackNum) 
           {
                stackNum = stackNum + 1;
           }
        }
    }
}
