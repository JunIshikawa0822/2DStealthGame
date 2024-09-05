public interface IBulletCaliberFactories
{
    public void SetUp();
    public IBulletFactory BulletFactory(IBulletCaliberType.BulletCaliberType bullet);
}
