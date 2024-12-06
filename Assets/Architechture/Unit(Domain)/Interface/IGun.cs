public interface IGun
{
    //void Init(float velocity, int simulNum, float shotInterval);
    public void OnSetUp(IObjectPool objectPool, string name);
    void OnUpdate();
    void Reload(Entity_Magazine magazine);
    void Shot();
    void Jam();
    Entity_Magazine GetMagazine();
}
