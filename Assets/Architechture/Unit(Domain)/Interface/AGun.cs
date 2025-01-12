using UnityEngine;

public abstract class AGun : MonoBehaviour, IObject
{
    //void Init(float velocity, int simulNum, float shotInterval);
    // private IGunData _gunData;
    // public IGunData GunData{get => _gunData; set => _gunData = value;}
    protected float _muzzleVelocity = 700f;
    protected float _shotInterval = 0.5f;
    protected uint _maxAmmoNum;
    protected string _gunName;
    protected float _reloadTime;
    protected IObjectPool _objectPool;
    private I_Data_Gun _gun_Data;

    public I_Data_Gun Data {get => _gun_Data;}
    public abstract Entity_Magazine Magazine{get;}
    public float ReloadTime{get => _reloadTime;}
    public float ShotInterval{get => _shotInterval;}
    public string Name {get => _gunName;}
    public uint MaxAmmoNum{get => _maxAmmoNum;}

    public virtual void OnSetUp(IObjectPool objectPool)
    {
        _objectPool = objectPool;
    }

    public virtual void Init(I_Data_Gun data)
    {
        _reloadTime = data.ReloadTime;
        _shotInterval = data.ShotInterval;
        _muzzleVelocity = data.ShotVelocity;

        _maxAmmoNum = data.MaxAmmoNum;

        _gun_Data = data;
    }
    //public abstract void OnUpdate();
    public abstract void Reload(Entity_Magazine magazine);
    public abstract void Shot();
    public abstract void Jam();

}
