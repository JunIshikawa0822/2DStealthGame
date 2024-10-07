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

    //視界の情報
    //------------------------------------------
    private DrawFieldOfView _drawFieldOfView;
    private FindOpponent _find;
    private DrawOpponent _draw;
    private float _viewAngle;
    private float _viewRadius;

    public override void OnSetUp(Entity_HealthPoint playerHP, DrawFieldOfView drawFieldOfView, FindOpponent find, DrawOpponent draw, float viewRadius, float viewAngle)
    {
        EntitySetUp(playerHP);

        _drawFieldOfView = drawFieldOfView;
        _find = find;
        _draw = draw;
        
        _viewRadius = viewRadius;
        _viewAngle = viewAngle;

        FindAndDrawEnemies(0.2f).Forget();
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
        _entityHP.EntityDamage(damage);

        if(_entityHP.CurrentHp <= 0)
        {
            OnEntityDead();
        }
    }

    public async UniTask FindAndDrawEnemies(float delayTime)
    {
        while(true)
        {
            if(this == null)return;
            
            List<Transform> foundOpponents = _find.FindVisibleTargets(_viewAngle, _viewRadius, this.transform);
            _draw.DrawTargets(foundOpponents);

            await UniTask.Delay((int)delayTime * 1000);
        }
    }

    public override void DrawView()
    {
        _drawFieldOfView.DrawFOV(_viewAngle, _viewRadius, this.transform);
    }

    public override void OnEntityDead()
    {
        Debug.Log($"プレイヤー({this.gameObject.name})はやられた！");
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
