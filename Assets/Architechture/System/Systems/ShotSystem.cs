
public class ShotSystem : ASystem, IOnUpdate
{
    IGun gun1;
    IGun gun2;

    public IGun[] gunsInPlayerItem = new IGun[2];

    public override void OnSetUp()
    {
        gun1 = gameStat.Pistol1;
        gun1.OnSetUp(gameStat.bullet_10mm_Factories, gameStat.obj);
    }

    public void OnUpdate()
    {
        if(gameStat.onClick)
        {
            gun1.Shot();
        }
    }
}
