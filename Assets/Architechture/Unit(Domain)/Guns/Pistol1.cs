using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Pistol1 : MonoBehaviour, IGun, IBulletType
{
    [Tooltip("Projectile force")]
    [SerializeField] float muzzleVelocity = 700f;
    [Tooltip("End point of gun where shots appear")]
    [SerializeField] private Transform muzzlePosition;
    IObjectPool objectPool;
    IBulletFactories bulletFactories;
    IBulletFactory factory;
    IBulletType.BulletType currentBulletState;
    
    public void OnSetUp(IBulletFactories bulletFactories, IObjectPool objectPool)
    {
        this.bulletFactories = bulletFactories;
        this.objectPool = objectPool;

        currentBulletState = IBulletType.BulletType.Bullet_10mm;
        factory = bulletFactories.BulletFactory(currentBulletState);

        //ダメ
        objectPool.PoolSetUp(factory);
    }

    public void OnUpdate()
    {

    }

    public void Shot()
    {
        factory = bulletFactories.BulletFactory(currentBulletState);
        if(factory == null)return;

        GameObject bulletObject = objectPool.GetFromPool(factory).gameObject;
        if (bulletObject == null)return;

        bulletObject.SetActive(true);

        // align to gun barrel/muzzle position
        bulletObject.transform.SetPositionAndRotation(muzzlePosition.position, muzzlePosition.rotation);

        // move projectile forward
        bulletObject.GetComponent<Rigidbody>().AddForce(bulletObject.transform.forward * muzzleVelocity, ForceMode.Acceleration);
    }

    public void Reload()
    {

    }

    public void Jam()
    {

    }
}
