using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Pistol1 : MonoBehaviour, IGun, IBulletType_10mm
{
    [SerializeField] 
    private float _muzzleVelocity = 700f;
    [SerializeField] 
    private Transform _muzzlePosition;
    private IObjectPool _objectPool;
    private IBulletFactories _bulletFactories;
    private IBulletFactory _factory;
    private IBulletType_10mm.BulletType_10mm _currentBulletState;
    
    public void OnSetUp(IBulletFactories bulletFactories, IObjectPool objectPool)
    {
        this._bulletFactories = bulletFactories;
        this._objectPool = objectPool;

        _currentBulletState = IBulletType_10mm.BulletType_10mm.Bullet_10mm_Normal;
        

        //ダメ
        _factory = bulletFactories.BulletFactory((int)_currentBulletState);
        objectPool.PoolSetUp(_factory);
    }

    public void OnUpdate()
    {

    }

    public void Shot()
    {
        _factory = _bulletFactories.BulletFactory((int)_currentBulletState);
        if(_factory == null)return;

        GameObject bulletObject = _objectPool.GetFromPool(_factory).gameObject;
        if (bulletObject == null)return;

        bulletObject.SetActive(true);

        // align to gun barrel/muzzle position
        bulletObject.transform.SetPositionAndRotation(_muzzlePosition.position, _muzzlePosition.rotation);

        // move projectile forward
        bulletObject.GetComponent<Rigidbody>().AddForce(bulletObject.transform.forward * _muzzleVelocity, ForceMode.Acceleration);
    }

    public void Reload()
    {

    }

    public void Jam()
    {

    }
}
