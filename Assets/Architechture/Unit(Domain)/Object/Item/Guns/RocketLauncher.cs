using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UniRx;
public class RocketLauncher : AGun
{
    //発射の内部的な処理に必要
    //----------------------------------------
    //----------------------------------------
    //private LineRenderer _muzzleFlashRenderer;
    //private IObjectPool _objectPool;

    //----------------------------------------
    //Shotの方、いるんか？
    private CancellationTokenSource _shotIntervalTokenSource;
    private CancellationTokenSource _actionIntervalTokenSource;
    //----------------------------------------
    private CompositeDisposable _shotDisposable;
    //銃に必要な処理
    //----------------------------------------
    private Entity_Magazine _magazine;

    public override Entity_Magazine Magazine { get => _magazine; }
    //----------------------------------------

    public override void OnSetUp(IObjectPool objectPool)
    {
        //_bulletFactories = bulletFactories;
        base.OnSetUp(objectPool);
        // _muzzleFlashRenderer = GetComponent<LineRenderer>();
        // _muzzleFlashRenderer.enabled = false;

        IsShotIntervalActive = false;
        _isJamming = false;

        _shotDisposable = new CompositeDisposable();
    }

    public override AGun Init(I_Data_Gun data)
    {
        base.Init(data);
        return this;
    }

    private async UniTaskVoid ContinuousFireAsync()
    {
        while (_actionIntervalTokenSource.IsCancellationRequested == false)
        {
            Debug.Log("うごいている");
            //Debug.Log(IsShotIntervalActive);
            if (!IsShotIntervalActive)
            {
                Shot();
                IsShotIntervalActive = true;
                IntervalWait(() => IsShotIntervalActive = false, _shotIntervalTokenSource.Token, _shotInterval, "射撃クールダウン").Forget();
            }
            else
            {
                Debug.Log("まだIntervalだよ");
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // 残弾が0になったら発射を停止
            if (Magazine.MagazineRemaining < 1)
            {
                _actionIntervalTokenSource.Cancel();
                _shotIntervalTokenSource.Cancel();
                break;
            }
        }

        Debug.Log("ぬけた");
    }

    public override void TriggerOn()
    {
        RefreshActionToken();
        RefreshShotIntervalToken();

        if (_gun_Data.IsAuto)
        {
            ContinuousFireAsync().Forget();
        }
        else
        {
            // Debug.Log("こっちがうごく");
            if (IsShotIntervalActive) return;

            Shot();
            IsShotIntervalActive = true;
            IntervalWait(() => IsShotIntervalActive = false, _shotIntervalTokenSource.Token, _shotInterval, "射撃クールダウン").Forget();
        }
    }

    public override void TriggerOff()
    {
        _actionIntervalTokenSource.Cancel();
    }

    private void RefreshShotIntervalToken()
    {
        if (_shotIntervalTokenSource == null)
        {
            _shotIntervalTokenSource = new CancellationTokenSource();
        }
        else if (_shotIntervalTokenSource.IsCancellationRequested)
        {
            _shotIntervalTokenSource.Dispose();
            _shotIntervalTokenSource = new CancellationTokenSource();
        }
    }

    private void RefreshActionToken()
    {
        if (_actionIntervalTokenSource == null)
        {
            _actionIntervalTokenSource = new CancellationTokenSource();
        }
        else if (_actionIntervalTokenSource.IsCancellationRequested)
        {
            _actionIntervalTokenSource.Dispose();
            _actionIntervalTokenSource = new CancellationTokenSource();
        }
    }

    private void Shot()
    {
        //マガジンがないor弾がないとそもそも撃てない
        if (_magazine == null || _magazine.MagazineRemaining < 1)
        {
            Debug.Log("弾、ないよ");
            return;
        }

        //射撃と射撃の間隔を制御
        //if(_isShotIntervalActive)return;
        //_objectPoolの有無をチェック
        if (_objectPool == null) return;
        //Poolからもってくる
        ABullet bullet = _objectPool.GetFromPool() as ABullet;

        if (bullet == null)
        {
            Debug.Log("キャスト無理ぃ");
            return;
        }

        if (bullet.gameObject == null) return;
        //発射
        bullet.Init(_muzzlePosition.position);
        bullet.GetBulletTransform().SetPositionAndRotation(_muzzlePosition.position, _muzzlePosition.rotation);
        bullet.GetBulletRigidbody().AddForce(bullet.gameObject.transform.forward * _muzzleVelocity, ForceMode.Acceleration);

        //弾を消費する
        _magazine.ConsumeBullet();
        _referenceInventoryItem.StackingNum = _magazine.MagazineRemaining;
        //_shotIntervalTokenSource = new CancellationTokenSource();
    }

    public override void Reload(Entity_Magazine magazine)
    {
        _magazine = magazine;

        if (_referenceInventoryItem == null) return;
        _referenceInventoryItem.StackingNum = _magazine.MagazineRemaining;
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
