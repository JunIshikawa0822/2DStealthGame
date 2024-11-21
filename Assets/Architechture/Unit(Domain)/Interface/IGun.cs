public interface IGun
{
    void Init(float velocity, int simulNum, float shotInterval);
    void OnUpdate();
    void Reload(Entity_Magazine magazine);
    void Shot();
    void Jam();
    Entity_Magazine GetMagazine();
}
