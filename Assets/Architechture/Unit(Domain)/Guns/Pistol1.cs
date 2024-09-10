using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Pistol1 : MonoBehaviour, IGun_10mm
{
    [SerializeField]
    private float _muzzleVelocity = 700f;
    [SerializeField] 
    private Transform _muzzlePosition;

    //もっと一般化していい奴ら↓
    private IObjectPool<ABullet> _objectPool;
    private IBulletFactories _bulletFactories;
    private IFactory<ABullet> _factory;
    private IBType_10mm.Caliber _bulletcaliber;
    
    public void OnSetUp(IBulletFactories bulletFactories, IObjectPool<ABullet> objectPool)
    {
        this._bulletFactories = bulletFactories;
        this._objectPool = objectPool;

        _bulletcaliber = IBType_10mm.Caliber.Bullet_10mm;
        //ダメ
        _factory = bulletFactories.BulletFactory(_bulletcaliber);
    }

    public void OnUpdate()
    {

    }

    public void Shot()
    {
        _factory = _bulletFactories.BulletFactory(_bulletcaliber);
        if(_factory == null)return;

        GameObject bulletObject = _objectPool.GetFromPool(_factory).gameObject;
        if (bulletObject == null)return;

        bulletObject.transform.SetPositionAndRotation(_muzzlePosition.position, _muzzlePosition.rotation);
        bulletObject.GetComponent<Rigidbody>().AddForce(bulletObject.transform.forward * _muzzleVelocity, ForceMode.Acceleration);
    }

    public void Reload()
    {

    }

    public void Jam()
    {

    }
}
