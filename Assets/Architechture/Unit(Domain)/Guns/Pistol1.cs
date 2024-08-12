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

    [Tooltip("Reference to Object Pool")]
    [SerializeField] ObjectPool objectPool;
    [SerializeField] BulletFactories bulletFactories;

    IBulletType.BulletType currentBulletState;
    IBulletFactory factory;

    public void Start()
    {
        SetUp();
    }

    public void SetUp()
    {
        currentBulletState = IBulletType.BulletType.Bullet_10mm;
        factory = bulletFactories.BulletFactory(currentBulletState);
        
        //ダメ
        objectPool.SetUp(factory);
    }

    public void OnUpdate()
    {

    }

    void IGun.Shot()
    {
        factory = bulletFactories.BulletFactory(currentBulletState);
        GameObject bulletObject = objectPool.GetObject(factory).gameObject;

        if (bulletObject == null)return;

        bulletObject.SetActive(true);

        // align to gun barrel/muzzle position
        bulletObject.transform.SetPositionAndRotation(muzzlePosition.position, muzzlePosition.rotation);

        // move projectile forward
        bulletObject.GetComponent<Rigidbody>().AddForce(bulletObject.transform.forward * muzzleVelocity, ForceMode.Acceleration);
    }

    void IGun.Reload()
    {

    }

    void IGun.Jam()
    {

    }
}
