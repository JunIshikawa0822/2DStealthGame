using System;
using UnityEngine;
using System.Threading;
using Unity.VisualScripting;
public class Bullet_Rocket : ABullet, IObject
{
    [SerializeField]
    float _lifeDistance;
    [SerializeField]
    float _bulletDamage;

    [SerializeField]
    GameObject _explosionPrefab;
    public string Name { get; set; }

    void Awake()
    {
        OnSetUp(_lifeDistance);
    }

    //弾の当たり判定はFixedUpdate内で計算。

    void FixedUpdate()
    {
        //Debug.Log($"Distance{_bulletLifeDistance}");
        if (IsBeyondLifeDistance())
        {
            // Debug.Log("距離によって破壊");
            //Debug.Log($"距離で削除された時のPrePos : {_bulletPrePos}");
            Release();
        }
        else if (IsBulletCollide())
        {
            Debug.Log($"{GetBulletRaycastHit().collider.name}にぶつかって破壊");

            AEntity entity = GetBulletRaycastHit().collider.GetComponent<AEntity>();
            GameObject Explosion_obj = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Explosion explosion = Explosion_obj.GetComponent<Explosion>();
            explosion.Explode();
            Release();

            if (entity == null) return;
            entity.OnDamage(_bulletDamage);
        }
    }

    public override Type GetBulletType()
    {
        return typeof(Bullet_Rocket);
    }
}
