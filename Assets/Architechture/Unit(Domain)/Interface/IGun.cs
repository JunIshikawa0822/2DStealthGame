public interface IGun
{
    void OnUpdate();
    void Reload(Entity_Magazine magazine);
    void Shot();
    void Jam();
    Entity_Magazine GetMagazine();
}
