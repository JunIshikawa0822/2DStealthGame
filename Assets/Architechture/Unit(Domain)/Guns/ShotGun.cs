using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;
public class Shotgun : AGun
{
    //発射の内部的な処理に必要
    //----------------------------------------
    private float _muzzleVelocity = 700f;
    private float _shotInterval = 0.5f;

    private int _simulNum; 
    private float _spreadAngle;
    //----------------------------------------

    [SerializeField]
    private Transform _muzzlePosition;
    //private LineRenderer _muzzleFlashRenderer;
    private IObjectPool _objectPool;

    //----------------------------------------
    private bool _isShotIntervalActive;
    private bool _isJamming;
    private CancellationTokenSource _shotIntervalTokenSource;
    //----------------------------------------

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
    }

    public void ShotgunInit(float velocity, float shotInterval, int simulNum, float spreadAngle)
    {
        _muzzleVelocity = velocity;
        _shotInterval = shotInterval;
        _simulNum = simulNum;
        _spreadAngle = spreadAngle;
    }

    public void OnUpdate()
    {

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
        if(_isShotIntervalActive)return;
        //_objectPoolの有無をチェック
        if(_objectPool == null)return;

        Quaternion centerAngle = _muzzlePosition.rotation;

        for(int i = 0; i < _simulNum; i++)
        {
            //Poolからもってくる
            ABullet bullet = _objectPool.GetFromPool() as ABullet;

            if(bullet == null)
            {
                Debug.Log("キャスト無理ぃ");
                return;
            }

            if (bullet.gameObject == null)return;

            float angleOffset = -_spreadAngle / 2 + (_spreadAngle / (_simulNum - 1)) * i;

            Debug.Log(angleOffset);
            Quaternion bulletRotation = centerAngle * Quaternion.Euler(0, angleOffset, 0);

            //発射
            Debug.Log("発射");
            bullet.Init(_muzzlePosition.position);
            bullet.GetBulletTransform().SetPositionAndRotation(_muzzlePosition.position, bulletRotation);
            bullet.GetBulletRigidbody().AddForce(bullet.gameObject.transform.forward * _muzzleVelocity, ForceMode.Acceleration);
        }

        //弾を消費する
        _magazine.ConsumeBullet();
        
        _shotIntervalTokenSource = new CancellationTokenSource();
        ShotInterval(_shotIntervalTokenSource.Token, _shotInterval, "射撃クールダウン").Forget();

        // if(_muzzleFlashRenderer == null) return;

        // _muzzleFlashRenderer.SetPosition(0, _muzzlePosition.position);
        // _muzzleFlashRenderer.SetPosition(1, _muzzlePosition.position + _muzzlePosition.forward * 2);
        // ActionInterval(() => LineRendererFlash(_muzzleFlashRenderer), _shotIntervalTokenSource.Token, 0.1f, "マズルフラッシュ").Forget();
        
        //ActionInterval(() => LineRendererFlash(_shotOrbitRenderer), shotIntervalTokenSource.Token, 0.1f, "軌道").Forget();
    }

    public override void Reload(Entity_Magazine magazine)
    {
        Debug.Log(this.gameObject.name + ":" + magazine);
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

    public async UniTask ShotInterval(CancellationToken token, float time, string ActionName)
    {
        _isShotIntervalActive = true;
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
            _isShotIntervalActive = false; // クールタイム終了（またはキャンセル)
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
}
