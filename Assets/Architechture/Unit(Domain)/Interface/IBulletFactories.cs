public interface IBulletFactories
{
    public void SetUp();
    public IBulletFactory BulletFactory(IBulletType.BulletType bulletType);
}
