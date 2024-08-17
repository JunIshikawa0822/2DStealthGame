
public class CellObject
{
    private Grid<CellObject> grid;
    private int position_x;
    private int position_y;

    private PlacedObject placedObject;
    public CellObject(Grid<CellObject> grid, int x, int y) 
    {
        this.grid = grid;
        position_x = x;
        position_y = y;
    }

    public bool CanBuild()
    {
        //placedObjectがnullかどうかを返す
        return placedObject == null;
    }

    public void SetPlacedObject(PlacedObject placedObject)
    {
        
    }
}
