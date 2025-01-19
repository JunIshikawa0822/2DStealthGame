using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using System.Threading;
using System;

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
    protected I_Data_Gun _gun_Data;

    [SerializeField]
    protected Transform _muzzlePosition;

    protected bool _isShotIntervalActive;
    protected bool _isJamming;

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
    public abstract void TriggerOn();
    public abstract void Shooting();
    public abstract void TriggerOff();

    public abstract void Shot();
    public abstract void Jam();
    public abstract UniTask IntervalWait(Action action, CancellationToken token, float time, string ActionName);

}
