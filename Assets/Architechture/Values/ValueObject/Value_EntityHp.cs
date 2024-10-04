
public class Value_EntityHp
{
    private readonly uint entityMaxHp;
    private readonly uint entityCurrentHp;

    public Value_EntityHp(uint maxHp, uint currentHp)
    {
        if(maxHp < 1)
        {
            maxHp = 1;
        }

        if(currentHp < 1)
        {
            currentHp = 1;
        }

        if(maxHp < currentHp)
        {
            currentHp = maxHp;
        }

        currentHp = maxHp;
    }
}
