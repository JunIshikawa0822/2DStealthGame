using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading;
using System.Threading;


public abstract class  ABullet : APooledObject<ABullet>
{
    protected Rigidbody _bulletRigidbody;
    protected Transform _bulletTransform;
    private Vector3 _bulletPrePos;
    private RaycastHit _bulletHit;
    private float _bulletLifeDistance;
    private float _bulletLifeMaxDistance;

    public Rigidbody GetBulletRigidbody(){return _bulletRigidbody;}
    public Transform GetBulletTransform(){return _bulletTransform;}
    public RaycastHit GetBulletRaycastHit(){return _bulletHit;}

    public void OnSetUp(float bulletLifeMaxDistance)
    {
        _bulletRigidbody = GetComponent<Rigidbody>();
        _bulletTransform = GetComponent<Transform>();

        _bulletPrePos = _bulletRigidbody.position;
        _bulletLifeMaxDistance = bulletLifeMaxDistance;
        _bulletLifeDistance = 0;
    }

    protected bool IsBeyondLifeDistance()
    {
        Vector3 bulletNowPos = _bulletRigidbody.position;
        float bulletMoveDistance = (bulletNowPos - _bulletPrePos).magnitude;
        _bulletLifeDistance += bulletMoveDistance;

        bool isBeyondLifeDistance = false;

        if(_bulletLifeDistance > _bulletLifeMaxDistance)
        {
            _bulletLifeDistance = 0;
            isBeyondLifeDistance = true;
        }

        return isBeyondLifeDistance;
    }
    
    protected bool IsBulletCollide()
    {
        //今フレームでの位置
        Vector3 bulletNowPos = _bulletRigidbody.position; 
        Vector3 bulletMoveVec = bulletNowPos - _bulletPrePos;
        bool isBulletCollide = false;

        //前フレームの位置から今の位置の向きにRayを飛ばす
        Ray ray = new Ray(_bulletPrePos, bulletNowPos); 

        if (Physics.Raycast(ray, out RaycastHit hit, bulletMoveVec.magnitude))
        {
            isBulletCollide = true;
            _bulletHit = hit;
        }

        //今のフレームの位置を次のフレームにおける前のフレームの位置として保存
        _bulletPrePos = bulletNowPos; 
        return isBulletCollide;
    }

    protected async UniTask Timer(float seconds, CancellationToken cancellationToken)
    {
        try
        {
            await UniTask.Delay((int)seconds * 1000, cancellationToken: cancellationToken);
            Debug.Log($"{seconds}秒が経過しました");
        }
        catch (OperationCanceledException)
        {
            // タスクがキャンセルされた場合の処理
            Debug.Log("タイマーがキャンセルされました");
        }
    }

    protected abstract void BulletLifeTime();

    public abstract Type GetBulletType();
}
