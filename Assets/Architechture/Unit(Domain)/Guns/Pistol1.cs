using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Pistol1 : MonoBehaviour, IGun_10mm
{
    [Tooltip("Projectile force")]
    [SerializeField] float muzzleVelocity = 700f;
    [Tooltip("End point of gun where shots appear")]
    [SerializeField] private Transform muzzlePosition;
    IObjectPool objectPool;
 
    //------------------------------------------------------------//

    IBulletFactories bulletFactories;
    IBulletFactory factory;
    [SerializeField]
    IBulletType_10mm.BulletType_10mm currentBulletState;

    //------------------------------------------------------------//

     IBulletCaliberFactories bulletCaliberFactories;

     //[SerializeField]
     IBulletCaliberType.BulletCaliberType currentBulletCaliberState;
    
    public void OnSetUp(IBulletFactories bulletFactories, IObjectPool objectPool)
    {
        this.bulletFactories = bulletFactories;
        this.objectPool = objectPool;

        currentBulletState = IBulletType_10mm.BulletType_10mm.Bullet_10mm_Normal;
 
        //ダメ
        factory = bulletFactories.BulletFactory((int)currentBulletState);
        objectPool.PoolSetUp(factory);
    }

    // public void OnSetUp(IBulletCaliberFactories bulletCaliberFactories, IObjectPool objectPool)
    // {
    //     this.bulletCaliberFactories = bulletCaliberFactories;
    //     this.objectPool = objectPool;

    //     currentBulletCaliberState = IBulletCaliberType.BulletCaliberType.Bullet_10mm;
 
    //     //ダメ
    //     factory = bulletCaliberFactories.BulletFactory(currentBulletCaliberState);
    //     objectPool.PoolSetUp(factory);
    // }

    public void OnUpdate()
    {

    }

    public void Shot()
    {
        factory = bulletFactories.BulletFactory((int)currentBulletState);
        //factory = bulletCaliberFactories.BulletFactory(currentBulletCaliberState);
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
