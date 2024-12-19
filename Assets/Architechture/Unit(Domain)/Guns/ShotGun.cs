using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;
public class Shotgun<T> : IItem where T : ABullet, IPooledObject<T>
{
   //発射の内部的な処理に必要
    //----------------------------------------
    private float _muzzleVelocity = 700f;
    private float _shotInterval = 0.5f;
    //----------------------------------------

    [SerializeField] 
    private Transform _muzzlePosition;
    private LineRenderer _muzzleFlashRenderer;
    private IObjectPool _objectPool;

    //----------------------------------------
    private bool _isShotIntervalActive;
    private bool _isJamming;
    private CancellationTokenSource _shotIntervalTokenSource;
    //----------------------------------------

    //銃に必要な処理
    //----------------------------------------
    private Entity_Magazine _magazine;
    public string Name{get;set;}

    public void OnSetUp(IObjectPool objectPool, string name)
    {
        //_bulletFactories = bulletFactories;
        _objectPool = objectPool;

        //_muzzleFlashRenderer = GetComponent<LineRenderer>();
        _muzzleFlashRenderer.enabled = false;

        _isShotIntervalActive = false;
        _isJamming = false;
        Name = name;
    }

    public void ShotgunInit( int simulNum, float velocity, float shotInterval)
    {

    }

    public void OnUpdate()
    {

    }

    public void Shot()
    {
        
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

    public Entity_Magazine GetMagazine()
    {
        return _magazine;
    }


}
