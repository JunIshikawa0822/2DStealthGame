public interface IGun : IGun<Bullet_10mm>
{
    public void OnUpdate();

    void Reload(Entity_Magazine magazine);
    void Shot();
    void Jam();

    //void MuzzleFlash();
    Entity_Magazine GetMagazine();

}
