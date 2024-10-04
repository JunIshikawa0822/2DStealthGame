public interface IGun : IItem
{
    public void OnSetUp(IBulletFactories bulletFactories, IObjectPool<ABullet> objectPool);
    public void OnUpdate();

    void Reload(Entity_Magazine magazine);
    void Shot();
    void Jam();

    //void MuzzleFlash();
    Entity_Magazine GetMagazine();
    
    //UniTask ShotInterval();
}
