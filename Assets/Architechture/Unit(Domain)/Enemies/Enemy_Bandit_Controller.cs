using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;

public class Enemy_Bandit_Controller : AEnemy, IEnemy, IBandit
{
    [SerializeField]
    private float _enemy_Bandit_MaxHP, _enemy_Bandit_InitHP;

    private Entity_HealthPoint _enemy_Bandit_HP;

    [SerializeField]
    private float _enemy_Bandit_RotateSpeed;


    [SerializeField]
    private LayerMask opponentLayer;

    [SerializeField]
    private LayerMask obstacleLayer;

    [SerializeField]
    private float _enemyViewAngle;
     [SerializeField]
    private float _enemyViewRadius;

    private bool isFighting;

    IGun _enemyGun;

    private FindOpponent _enemyFindView;

    private ReactiveProperty<Transform>_currentTarget;
    private ReactiveProperty<IBandit.BanditStatus> _currentStatus;
    private ReactiveProperty<IBandit.BanditAction> _currentAction;
    private ReactiveProperty<IBandit.BanditBattleAction> _currentBattleAction;

    private CompositeDisposable _disposablesByStatus;
    private CompositeDisposable _disposablesByAction;
    private CompositeDisposable _disposablesByLifeCycle;

    //いずれはEnemyも生成した側で初期化することだけ留意
    void Start()
    {
        OnSetUp(new Entity_HealthPoint(_enemy_Bandit_MaxHP, _enemy_Bandit_InitHP), new HandGun());

        _currentStatus = new ReactiveProperty<IBandit.BanditStatus>(IBandit.BanditStatus.Usual);
        _currentAction = new ReactiveProperty<IBandit.BanditAction>(IBandit.BanditAction.Standing);
        _currentBattleAction = new ReactiveProperty<IBandit.BanditBattleAction>(IBandit.BanditBattleAction.Idle);
        _currentTarget = new ReactiveProperty<Transform>();

        _disposablesByStatus = new CompositeDisposable();
        _disposablesByAction = new CompositeDisposable();
        _disposablesByLifeCycle = new CompositeDisposable();

        SetEvent();
    }

    public void OnSetUp(Entity_HealthPoint enemy_Bandit_HP, IGun gun)
    {
        _enemyFindView = new FindOpponent(opponentLayer, obstacleLayer);
        _enemy_Bandit_HP = enemy_Bandit_HP;

        EntitySetUp();
        EntityMeshDisable();

        if(_enemy_Bandit_HP == null)
        {
            this.gameObject.SetActive(false);
            Debug.LogWarning($"{this.gameObject.name}に体力を設定してください");
            return;
        }
    }

    public void SetEvent()
    {
         //状態が変化した時に一度だけ状態変化イベントを行う（未定）
        _currentStatus.DistinctUntilChanged().Subscribe(status =>  
            {
                TriggerEventOnStatusChanged(ref status);
            }).AddTo(_disposablesByLifeCycle, this);

         //状態が変化した時に一度だけ状態変化イベントを行う（未定）
        _currentAction.DistinctUntilChanged().Subscribe(action =>
            {
                TriggerEventOnActionChanged(ref action);
            }).AddTo(_disposablesByLifeCycle,this);

        _currentBattleAction.DistinctUntilChanged().Subscribe(battleAction =>
            {
                TriggerEventOnBattleActionChanged(ref battleAction);
            }).AddTo(_disposablesByLifeCycle,this);
         
         //状態が変化した時に一度だけ状態変化イベントを行う（未定）
        _currentTarget.DistinctUntilChanged().Subscribe(target =>
            {
                TriggerEventOnTargetTransformChanged(ref target);
            }).AddTo(_disposablesByLifeCycle, this);

        //targetを見つけている時は常にtargetの方を向く
        Observable.EveryUpdate().Subscribe(_ => 
            {
                Rotate();
            }).AddTo(_disposablesByLifeCycle, this);
    }

    private void TriggerEventOnStatusChanged(ref IBandit.BanditStatus status)
    {
        StartSearchAround(ref status); //状態に応じた探索の時間間隔でSearchAroundを実行

        switch(status)
        {
            case IBandit.BanditStatus.Usual : break;
            case IBandit.BanditStatus.Warn : 
                _currentBattleAction.Value = IBandit.BanditBattleAction.Attacking;
                break;
            case IBandit.BanditStatus.Caution : break;
        }
    }

    private void TriggerEventOnActionChanged(ref IBandit.BanditAction action)
    {
        switch(action)
        {
            case IBandit.BanditAction.Standing: break;
            case IBandit.BanditAction.Running : break;
            case IBandit.BanditAction.Walking : break;
        }
    }

    private void TriggerEventOnBattleActionChanged(ref IBandit.BanditBattleAction action)
    {
        switch(action)
        {
            case IBandit.BanditBattleAction.Idle: break;
            case IBandit.BanditBattleAction.Healing: break;
            case IBandit.BanditBattleAction.Hiding : break;
            case IBandit.BanditBattleAction.Reloading : break;
            case IBandit.BanditBattleAction.Attacking : break;
        }
    }

    private void TriggerEventOnTargetTransformChanged(ref Transform targetTransform)
    {
        switch(targetTransform)
        {
            case null : _currentStatus.Value = IBandit.BanditStatus.Usual; break;
            default :  _currentStatus.Value = IBandit.BanditStatus.Warn; break;
        }
    }

    //Statusが変化するたびに呼ばれる 
    private void StartSearchAround(ref IBandit.BanditStatus status)
    {
        _disposablesByStatus.Clear();

        float interval = GetSearchInterval(ref status);

        //statusに応じたインターバルで周囲を探索する
        Observable.Interval(System.TimeSpan.FromSeconds(interval)).Subscribe(_ =>
            {
                SearchAround();
            })
            .AddTo(_disposablesByStatus, this);
    }

    private void SearchAround()
    {
        List<Transform> objectList = _enemyFindView.FindVisibleTargets(_enemyViewAngle, _enemyViewRadius, this.transform);
        _currentTarget.Value = FindNearestObject(objectList, this.transform);

        if(_currentTarget.Value == null)return;
        Debug.Log($"{this.gameObject.name} : {_currentTarget.Value.position}に敵がいるぞ！");
    }

    private float GetSearchInterval(ref IBandit.BanditStatus status)
    {
        switch(status)
        {
            case IBandit.BanditStatus.Usual : return 0.5f;
            case IBandit.BanditStatus.Warn : return 0.2f;
            case IBandit.BanditStatus.Caution : return 0.35f;
            default : return 1f;
        }
    }

    public void Move()
    {
        //ランダムな移動
    }

    public void Rotate()
    {
        if(_currentTarget.Value == null)return;
        Quaternion targetRotation = Quaternion.LookRotation(_currentTarget.Value.position - _entityTransform.position);
        _entityTransform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(_entityTransform.eulerAngles.y, targetRotation.eulerAngles.y, _enemy_Bandit_RotateSpeed * Time.deltaTime);
    }

    public void Attack()
    {
        _enemyGun.Shot();
    }

    public void Hide()
    {
       //攻撃された状態のとき一定間隔で
       //HPが一定以下になったら
       //周りに仲間がいない時

       //隠れる行動
    }

    public void Reload(IGun gun, Entity_Magazine magazine)
    {
        //リロード

        if(_isEntityActionInterval)return;
        //CancelAction(actionCancellationTokenSource);

        _actionCancellationTokenSource = new CancellationTokenSource();
        ActionInterval(() => gun.Reload(magazine), _actionCancellationTokenSource.Token, 2f, "リロード").Forget();
    }

    public override void OnDamage(float damage)
    {
        _enemy_Bandit_HP.EntityDamage(damage);

        if(IsEntityDead())
        {
            OnEntityDead();
        }
    }

    public override bool IsEntityDead()
    {
        if(_enemy_Bandit_HP.CurrentHp <= 0)
        {
            return true;
        }

        return false;
    }

    public override void OnEntityDead()
    {
        _disposablesByLifeCycle.Clear();

        this.gameObject.SetActive(false);
        Debug.Log($"{this.gameObject.name}はやられた！");
    }

    
}
