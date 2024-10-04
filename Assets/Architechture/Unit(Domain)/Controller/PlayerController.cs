using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class PlayerController : APlayer
{
    [SerializeField] private float _moveForce = 5;
    private Quaternion targetRotation;
    private float rotationSpeed = 500;

    private bool isActionInterval = false;
    private CancellationTokenSource actionCancellationTokenSource;

    public override void OnSetUp(int playerHp)
    {
        base.OnSetUp(playerHp);
    }

    public override void OnMove(Vector2 inputDirection, Vector3 mouseWorldPosition)
    {
        //移動
        _entityRigidbody.AddForce(new Vector3(inputDirection.x, 0, inputDirection.y) * _moveForce, ForceMode.Force);

        //回転
        targetRotation = Quaternion.LookRotation(mouseWorldPosition - _entityTransform.position);
        _entityTransform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(_entityTransform.eulerAngles.y, targetRotation.eulerAngles.y, rotationSpeed * Time.deltaTime);
    }

    public override void OnAttack(IGun gun)
    {
        gun.Shot();
    }

    public override void OnReload(IGun gun, Entity_Magazine magazine)
    {
        if(isActionInterval)return;
        //CancelAction(actionCancellationTokenSource);

        actionCancellationTokenSource = new CancellationTokenSource();
        ActionInterval(() => gun.Reload(magazine), isActionInterval, actionCancellationTokenSource.Token, 2f, "リロード").Forget();
    }

    public override void OnDamage(float damage)
    {
        
    }

    public override void OnEntityDead()
    {
        Debug.Log("テストだよん");
    }

    public async UniTask ActionInterval(Action waitAction, bool flag, CancellationToken token, float time, string ActionName)
    {
        isActionInterval = true;

        try
        {
            // 指定されたクールタイム期間を待つ (キャンセル可能)
            Debug.Log($"{ActionName} 開始");
            await UniTask.Delay((int)(time * 1000), cancellationToken: token);
            waitAction?.Invoke();
            Debug.Log($"{ActionName} 終了");
        }
        catch
        {
            Debug.Log($"{ActionName} がキャンセルされました");
        }
        finally
        {
            isActionInterval = false; // クールタイム終了（またはキャンセル)
        }
    }

    public void CancelAction(CancellationTokenSource tokenSource)
    {
        Debug.Log("キャンセルしようとしている");
        if (tokenSource != null && !tokenSource.IsCancellationRequested)
        {
            Debug.Log("きゃんせる");
            tokenSource.Cancel();
            tokenSource.Dispose();
            tokenSource = null;
            isActionInterval = false;
        }
    }
}
