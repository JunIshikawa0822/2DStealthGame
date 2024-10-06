public interface IGun : IItem
{
    public void OnSetUp(IFactories<ABullet> bulletFactories, IObjectPool<ABullet> objectPool);
    public void OnUpdate();

    void Reload(Entity_Magazine magazine);
    void Shot();
    void Jam();

    //void MuzzleFlash();
    Entity_Magazine GetMagazine();
    
    //UniTask ShotInterval();
}
