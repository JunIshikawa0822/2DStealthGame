using UnityEngine;

public abstract class AGun : MonoBehaviour
{
    //void Init(float velocity, int simulNum, float shotInterval);
    public abstract void OnSetUp(IObjectPool objectPool, string name);
    //public abstract void OnUpdate();
    public abstract void Reload(Entity_Magazine magazine);
    public abstract void Shot();
    public abstract void Jam();
    public abstract Entity_Magazine GetMagazine();

    private IGunData _gunData;
    public IGunData GunData{get => _gunData; set => _gunData = value;}

    private string _gunName;
    public string Name{get => _gunName; set => _gunName = value;}

    private float _reloadTime;
    public float ReloadTime{get => _reloadTime; set => _reloadTime = value;}
}
