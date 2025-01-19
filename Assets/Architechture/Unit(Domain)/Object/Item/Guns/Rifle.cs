using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UniRx;

public class Rifle : AGun
{
    //発射の内部的な処理に必要
    //----------------------------------------
    //----------------------------------------
    //private LineRenderer _muzzleFlashRenderer;
    //private IObjectPool _objectPool;

    //----------------------------------------
    private CancellationTokenSource _shotIntervalTokenSource;
    private CancellationTokenSource _actionIntervalTokenSource;
    //----------------------------------------
    private CompositeDisposable _shotDisposable;

    //銃に必要な処理
    //----------------------------------------
    private Entity_Magazine _magazine;

    public override Entity_Magazine Magazine{get => _magazine;}
    //----------------------------------------

    public override void OnSetUp(IObjectPool objectPool)
    {
        //_bulletFactories = bulletFactories;
        base.OnSetUp(objectPool);
        // _muzzleFlashRenderer = GetComponent<LineRenderer>();
        // _muzzleFlashRenderer.enabled = false;

        _isShotIntervalActive = false;
        _isJamming = false;

        _shotDisposable = new CompositeDisposable();
    }

    public override void Init(I_Data_Gun data)
    {
        base.Init(data);

        if(!(data is I_Data_Rifle rifleData)) return;
        
    }

    public void OnUpdate()
    {

    }

    public override void TriggerOn()
    {
        //射撃と射撃の間隔を制御
        if(_isShotIntervalActive)return;

        Shot();

        _isShotIntervalActive = true;
        _shotIntervalTokenSource = new CancellationTokenSource();
        
        IntervalWait(() => _isShotIntervalActive = false, _shotIntervalTokenSource.Token, _shotInterval, "射撃クールダウン").Forget();    

    }

    public override void Shooting()
    {
        if(_gun_Data.IsAuto == false) return;

        Observable.Interval(System.TimeSpan.FromSeconds(_gun_Data.ShotInterval))
            .Subscribe(_ => Shot())
            .AddTo(_shotDisposable); // 射撃用のDisposableに追加
    }

    public override void TriggerOff()
    {
        _shotDisposable.Clear();
    }

    public override void Shot()
    {
        //マガジンがないor弾がないとそもそも撃てない
        if(_magazine == null || _magazine.MagazineRemaining < 1)
        {
            Debug.Log("弾、ないよ");
            return;
        }

        //射撃と射撃の間隔を制御
        //if(_isShotIntervalActive)return;
        //_objectPoolの有無をチェック
        if(_objectPool == null)return;
        //Poolからもってくる
        ABullet bullet = _objectPool.GetFromPool() as ABullet;

        if(bullet == null)
        {
            Debug.Log("キャスト無理ぃ");
            return;
        }

        if (bullet.gameObject == null)return;
        //発射
        bullet.Init(_muzzlePosition.position);
        bullet.GetBulletTransform().SetPositionAndRotation(_muzzlePosition.position, _muzzlePosition.rotation);
        bullet.GetBulletRigidbody().AddForce(bullet.gameObject.transform.forward * _muzzleVelocity, ForceMode.Acceleration);

        //弾を消費する
        _magazine.ConsumeBullet();
        //_shotIntervalTokenSource = new CancellationTokenSource();

        _isShotIntervalActive = true;
        //IntervalWait(() => _isShotIntervalActive = false, _shotIntervalTokenSource.Token, _shotInterval, "射撃クールダウン").Forget();
    }

    public override void Reload(Entity_Magazine magazine)
    {
        _magazine = magazine;
    }

    public override void Jam()
    {
        _isJamming = true;
    }

    public void LineRendererFlash(LineRenderer lineRenderer)
    {
        lineRenderer.enabled = !lineRenderer.enabled;
    }

    public override async UniTask IntervalWait(Action action, CancellationToken token, float time, string ActionName)
    {
        try
        {
            await UniTask.Delay((int)(time * 1000), cancellationToken: token);
        }
        catch
        {
            Debug.Log($"{ActionName} がキャンセルされました");
        }
        finally
        {
            action?.Invoke();
        }
    }
}
