using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class HandGun : MonoBehaviour, IGun_10mm
{
    //発射の内部的な処理に必要
    //----------------------------------------
    [SerializeField]
    private float _muzzleVelocity = 700f;
    [SerializeField] 
    private Transform _muzzlePosition;
    private LineRenderer _muzzleFlashRenderer;

    private IObjectPool<ABullet> _objectPool;
    private IFactories<ABullet> _bulletFactories;
    private IFactory<ABullet> _bulletcaliberFactory;
    //----------------------------------------
    private bool _isShotIntervalActive;
    private bool _isJamming;
    private CancellationTokenSource shotIntervalTokenSource;

    //----------------------------------------

    //銃に必要な処理
    //----------------------------------------
    private Entity_Magazine _magazine;
    //----------------------------------------

    public void OnSetUp(IFactories<ABullet> bulletFactories, IObjectPool<ABullet> objectPool)
    {
        _bulletFactories = bulletFactories;
        _objectPool = objectPool;

        _muzzleFlashRenderer = GetComponent<LineRenderer>();
        _muzzleFlashRenderer.enabled = false;

        _isShotIntervalActive = false;
        _isJamming = false;
        
        _bulletcaliberFactory = GetFactory_10mm<IBType_10mm>();
    }

    public void OnUpdate()
    {

    }

    public void Shot()
    {
        //マガジンがないor弾がないとそもそも撃てない
        if(_magazine == null || _magazine.MagazineRemaining < 1)
        {
            Debug.Log("弾、ないよ");
            return;
        }

        //射撃と射撃の間隔を制御
        if(_isShotIntervalActive)return;
        shotIntervalTokenSource = new CancellationTokenSource();
        Interval(_isShotIntervalActive,shotIntervalTokenSource.Token, 0.5f, "射撃クールダウン").Forget();

        //BulletのFactoryをチェック
        _bulletcaliberFactory = GetFactory_10mm<IBType_10mm>();
        if(_bulletcaliberFactory == null)return;
        
        //Poolからもってくる
        ABullet bullet = _objectPool.GetFromPool(_bulletcaliberFactory) as ABullet;

        if(bullet == null)
        {
            Debug.Log("キャスト無理ぃ");
            return;
        }

        if (bullet.gameObject == null)return;
        //発射
        Debug.Log("発射");
        bullet.Init(_muzzlePosition.position);
        bullet.GetBulletTransform().SetPositionAndRotation(_muzzlePosition.position, _muzzlePosition.rotation);
        bullet.GetBulletRigidbody().AddForce(bullet.gameObject.transform.forward * _muzzleVelocity, ForceMode.Acceleration);

        //弾を消費する
        _magazine.ConsumeBullet();

        if(_muzzleFlashRenderer == null) return;
        shotIntervalTokenSource = new CancellationTokenSource();

        _muzzleFlashRenderer.SetPosition(0, _muzzlePosition.position);
        _muzzleFlashRenderer.SetPosition(1, _muzzlePosition.position + _muzzlePosition.forward * 2);
        ActionInterval(() => LineRendererFlash(_muzzleFlashRenderer), shotIntervalTokenSource.Token, 0.1f, "マズルフラッシュ").Forget();
        
        //ActionInterval(() => LineRendererFlash(_shotOrbitRenderer), shotIntervalTokenSource.Token, 0.1f, "軌道").Forget();
    }

    public void Reload(Entity_Magazine magazine)
    {
        _magazine = magazine;
    }

    public void Jam()
    {
        _isJamming = true;
    }

    public void LineRendererFlash(LineRenderer lineRenderer)
    {
        lineRenderer.enabled = !lineRenderer.enabled;
    }

    public async UniTask Interval(bool flag, CancellationToken token, float time, string ActionName)
    {
        flag = true;

        try
        {
            // 指定されたクールタイム期間を待つ (キャンセル可能)
            //Debug.Log($"{ActionName} 開始");
            await UniTask.Delay((int)(time * 1000), cancellationToken: token);
            //Debug.Log($"{ActionName} 終了");
        }
        catch
        {
            Debug.Log($"{ActionName} がキャンセルされました");
        }
        finally
        {
            flag = false; // クールタイム終了（またはキャンセル)
        }
    }

    public async UniTask ActionInterval(Action action, CancellationToken token, float time, string ActionName)
    {
        try
        {
            // 指定されたクールタイム期間を待つ (キャンセル可能)
            //Debug.Log($"{ActionName} 開始");
            action.Invoke();
            await UniTask.Delay((int)(time * 1000), cancellationToken: token);
            //Debug.Log($"{ActionName} 終了");
        }
        catch
        {
            Debug.Log($"{ActionName} がキャンセルされました");
        }
        finally
        {
            action.Invoke();
        }
    }

    // public void CancelInterval(CancellationTokenSource tokenSource)
    // {
    //     if (tokenSource != null && !tokenSource.IsCancellationRequested)
    //     {
    //         tokenSource.Cancel();
    //         tokenSource.Dispose();
    //         tokenSource = null;
    //     }
    // }

    public Entity_Magazine GetMagazine()
    {
        return _magazine;
    }

    public IFactory<ABullet> GetFactory_10mm<T>() where T : IBType_10mm
    {
        // ここで、T 型が IBType_10mm を継承していることが保証されています
        return _bulletFactories.BulletFactory(typeof(T));
    }
}
