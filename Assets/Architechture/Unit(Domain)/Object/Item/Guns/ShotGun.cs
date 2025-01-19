using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;
public class Shotgun : AGun
{
    //発射の内部的な処理に必要
    //----------------------------------------
    private int _simulNum; 
    private float _spreadAngle;
    //----------------------------------------
    //private LineRenderer _muzzleFlashRenderer;
    //private IObjectPool _objectPool;

    //----------------------------------------
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

    public override void Init(I_Data_Gun data)
    {
        base.Init(data);

        if(!(data is I_Data_Shotgun shotgunData))return;
        _simulNum = shotgunData.SimulNum;
        _spreadAngle = shotgunData.SpreadAngle;
    }

    public void OnUpdate()
    {

    }

    public override void TriggerOn()
    {
        Shot();
    }

    public override void Shooting()
    {
        if(_gun_Data.IsAuto == false) return;
        Shot();
    }

    public override void TriggerOff()
    {
        Debug.Log("連射不可");
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
        //Poolからもってくる

        Quaternion centerAngle = _muzzlePosition.rotation;

        for(int i = 0; i < _simulNum; i++)
        {
            //Poolからもってくる
            ABullet bullet = _objectPool.GetFromPool() as ABullet;

            if(bullet == null)
            if (bullet.gameObject == null)return;

            float angleOffset = -_spreadAngle / 2 + _spreadAngle / (_simulNum - 1) * i;

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

        _isShotIntervalActive = true;
        IntervalWait(() => _isShotIntervalActive = false, _shotIntervalTokenSource.Token, _shotInterval, "射撃クールダウン").Forget();    
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
