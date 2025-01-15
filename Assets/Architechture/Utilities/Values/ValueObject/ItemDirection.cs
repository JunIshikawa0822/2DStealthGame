
public class UI_ItemDirection
{
    public Dir Direction{get;}

    public UI_ItemDirection(Dir direction)
    {
        Direction = direction;
    }

    public enum Dir
    {
        Down,
        Left,
        Up,
        Right
    }

    // public Dir GetNextDir(Dir dir)
    // {
    //     switch (dir) 
    //     {
    //         default:
    //         case Dir.Down:      return Dir.Left;
    //         case Dir.Left:      return Dir.Up;
    //         case Dir.Up:        return Dir.Right;
    //         case Dir.Right:     return Dir.Down;
    //     }
    // }
}
