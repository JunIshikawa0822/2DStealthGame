
public class Entity_Magazine
{
    private readonly uint _magazineCapaity;
    private uint _remainingNum;

    public Entity_Magazine(uint magazineCapaity, uint currentRemainingNum)
    {
        if(magazineCapaity < 1)
        {
            magazineCapaity = 1;
        }

        _magazineCapaity = magazineCapaity;
        _remainingNum = currentRemainingNum;
    }

    public uint MagazineCapacity{get{return _magazineCapaity;}}
    public uint MagazineRemaining{get{return _remainingNum;}}

    public void ConsumeBullet()
    {
        if(_remainingNum > 0)
        {
            _remainingNum  = _remainingNum - 1;
        }
    }
}
