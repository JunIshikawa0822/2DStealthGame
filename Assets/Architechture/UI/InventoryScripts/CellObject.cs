using UnityEngine;

public class CellObject
{
    //private Vector3 cellObjectPosition;
    //private Grid<CellObject> grid;
    private int position_x;
    private int position_y;

    private PlacedObject placedObject;
    public CellObject(/*Grid<CellObject> grid,*/ int x, int y) 
    {
        //this.grid = grid;
        position_x = x;
        position_y = y;
        //cellObjectPosition = worldPosition;
    }

    public PlacedObject SetGetPlacedObject
    {
        set{placedObject = value;}
        get{return placedObject;}
    }
}
