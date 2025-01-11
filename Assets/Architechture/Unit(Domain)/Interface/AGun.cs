using UnityEngine;

public abstract class AGun : MonoBehaviour, IObject
{
    //void Init(float velocity, int simulNum, float shotInterval);
    public virtual void OnSetUp(IObjectPool objectPool)
    {
        _objectPool = objectPool;
    }

    public virtual void Init(string name, float reloadTime, uint maxAmmoNum)
    {
        _gunName = name;
        _reloadTime = reloadTime;
        _maxAmmoNum = maxAmmoNum;
    }
    //public abstract void OnUpdate();
    public abstract void Reload(Entity_Magazine magazine);
    public abstract void Shot();
    public abstract void Jam();

    private IGunData _gunData;
    public IGunData GunData{get => _gunData; set => _gunData = value;}

    protected IObjectPool _objectPool;
    public abstract Entity_Magazine Magazine{get;}
    private float _reloadTime;
    public float ReloadTime{get => _reloadTime; set => _reloadTime = value;}

    private string _gunName;
    public string Name {get => _gunName;}

    private uint _maxAmmoNum;
    public uint MaxAmmoNum{get => _maxAmmoNum;}
}
