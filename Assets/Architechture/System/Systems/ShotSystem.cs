
public class ShotSystem : ASystem, IOnUpdate
{
    IGun _gun1;
    IGun _gun2;

    public IGun[] gunsInPlayerItem = new IGun[2];

    public override void OnSetUp()
    {
        //_gun1 = gameStat.Pistol1;
        //_gun1.OnSetUp(gameStat.bullet_10mm_Factories, gameStat.objectPool);
    }

    public void OnUpdate()
    {
        // if(gameStat.onAttack)
        // {
        //     _gun1.Shot();
        // }
    }
}
