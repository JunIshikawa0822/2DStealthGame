public class CellObject
{   
    public int position_x;
    public int position_y;

    private PlacedObject placedObject;

    private bool canStackOnCell;

    //現在Stackされている数
    private int stackNum;

    //private bool isStackableOnCell;

    public CellObject(/*Grid<CellObject> grid,*/ int x, int y) 
    {
        //this.grid = grid;
        position_x = x;
        position_y = y;
        stackNum = 0;
        SetStackability();
        //cellObjectPosition = worldPosition;
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
            if(this.stackNum < placedObject.GetItemData().stackableNum)
            {
                canInsert = true;
            }  
        }   
        return canInsert;
    }

    public void SetStackability()
    {
        if(this.placedObject == null)
        {
            canStackOnCell = true;
            return;
        }

        int canStackNum = this.placedObject.GetItemData().stackableNum;

        if(canStackNum > 0)
        {
            if(this.stackNum < canStackNum)
            {
                canStackOnCell = true;
                return;
            }
        }

        canStackOnCell = false;
    }

    public bool GetStackability()
    {
        return canStackOnCell;
    }

    public void InsertToCellObject(PlacedObject placedObject)
    {
        this.placedObject = placedObject;
    }

    public void SetStackNum()
    {
        if(this.placedObject == null)
        {
            stackNum = 0;
            return;
        }

        if(this.canStackOnCell == false) return;

        stackNum = stackNum + 1;
    }
}
