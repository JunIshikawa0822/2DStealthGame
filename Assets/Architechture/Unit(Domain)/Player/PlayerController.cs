using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.Rendering;

public class PlayerController : AEntity
{
    [SerializeField]
    private float _player_RotateSpeed = 500;

    //private Entity_HealthPoint _playerHP;

    public Transform equipPos;

    [SerializeField] private float _playerMoveForce = 50;

    private FOV _fieldOfView;
    private Animator _playerAnimator;

    public Action<Storage> storageFindEvent;
    public Action<Storage> leaveStorageEvent;
    public void OnSetUp(Entity_HealthPoint playerHP)
    {
        #region 直で代入でよくね
        EntitySetUp();
        #endregion

        _entityHP = playerHP;
        _fieldOfView = GetComponent<FOV>();
        _playerAnimator = GetComponent<Animator>();
        
        //FindAndDrawEnemies(0.2f).Forget();
    }

    public void Move(Vector2 inputDirection)
    {
        //Debug.Log("移動");
        //移動
        _entityRigidbody.AddForce(new Vector3(inputDirection.x, 0, inputDirection.y) * _playerMoveForce, ForceMode.Force); 
    }

    public void Rotate(Vector3 mouseWorldPosition)
    {
        
        //回転
        Quaternion targetRotation = Quaternion.LookRotation(mouseWorldPosition - _entityTransform.position);
        _entityTransform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(_entityTransform.eulerAngles.y, targetRotation.eulerAngles.y, _player_RotateSpeed * Time.deltaTime);
    }

    public void UpdateAnimation(Vector2 inputDirection)
    {
        //ここから移動アニメーション用計算
        Vector2 forwardDirection = new Vector2(this.transform.forward.x, this.transform.forward.z);
        Vector2 relativeVec = CalculateRelativePosition(forwardDirection, inputDirection);

        _playerAnimator.SetFloat("Horizontal", relativeVec.x);
        _playerAnimator.SetFloat("Vertical", relativeVec.y);
        _playerAnimator.SetFloat("Speed", relativeVec.magnitude);

        Vector2 CalculateRelativePosition(Vector2 A, Vector2 B)
        {
            Vector2 normalizedBaseVec = A.normalized;
            float relativeX = normalizedBaseVec.x * B.x + normalizedBaseVec.y * B.y;     // Aを基準にしたBのx成分
            float relativeY = -normalizedBaseVec.y * B.x + normalizedBaseVec.x * B.y;    // Aを基準にしたBのy成分

            Debug.Log(new Vector2(-relativeY, relativeX));

            return new Vector2(-relativeY, relativeX);
        }
    }

    public void Attack(AGun gun)
    {
        gun.Shot();
    }

    public void Reload(AGun gun, Entity_Magazine magazine)
    {
        if(_isEntityActionInterval)return;
        //CancelAction(actionCancellationTokenSource);

       // _actionCancellationTokenSource = new CancellationTokenSource();
        EntityActionInterval(() => gun.Reload(magazine), _actionCancellationTokenSource.Token, 2f, "リロード").Forget();
    }

    public void Equip(AGun gun)
    {
        gun.transform.SetParent(equipPos);
        gun.transform.SetPositionAndRotation(equipPos.position, this.transform.rotation);
    }

    public override void OnDamage(float damage)
    {
        _entityHP.EntityDamage(damage);

        if(_entityHP.CurrentHp <= 0)
        {
            OnEntityDead();
        }
    }

    // public async UniTask FindAndDrawEnemies(float delayTime)
    // {
    //     while(true)
    //     {
    //         if(this == null)return;

    //         List<Transform> foundOpponents = _find.FindVisibleTargets(_viewAngle, _viewRadius, this.transform);
    //         _draw.DrawTargets(foundOpponents);

    //         await UniTask.Delay((int)delayTime * 1000);
    //     }
    // }

    // public void DrawView()
    // {
    //     _drawFieldOfView.DrawFOV(_viewAngle, _viewRadius, this.transform);
    // }

    public override void OnEntityDead()
    {
        Debug.Log($"プレイヤー({this.gameObject.name})はやられた！");
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
            _isEntityActionInterval = false;
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Storage")Debug.Log("Storage見つけた");
        storageFindEvent?.Invoke(collider.gameObject.GetComponent<Storage>());
    }

    public void OnTriggerExit(Collider collider)
    {
        if(collider.gameObject.tag == "Storage")Debug.Log("Storageから離れた");
        leaveStorageEvent?.Invoke(collider.gameObject.GetComponent<Storage>());
    }
}
