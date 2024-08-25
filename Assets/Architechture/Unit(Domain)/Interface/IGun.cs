public interface IGun : IItem
{
    public void OnSetUp(IBulletFactories bulletFactories, IObjectPool objectPool);
    public void OnUpdate();
    void Reload();
    void Shot();
    void Jam();
}
