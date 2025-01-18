using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UnityEngine.UI;
using TMPro;

public class Enemy_Bandit_Controller : AEnemy, IEnemy, IBandit
{
    [SerializeField]
    private Transform _gunTrans;
    //private Entity_HealthPoint _enemy_Bandit_HP;
    [SerializeField]
    private float _enemy_Bandit_RotateSpeed;

    [SerializeField]
    private TextMeshPro _statusText;

    private bool isFighting;

    //AGun _enemyGun;
    FOV _enemyFieldOfView;

    [SerializeField]private NormalStorage _enemyStorage;
    [SerializeField]private WeaponStorage _enemyWeaponStorage;
    public override IStorage Storage {get => _enemyStorage;}
    public override IStorage WeaponStorage {get => _enemyWeaponStorage;}

    //private FindOpponent _enemyFindView;

    private ReactiveProperty<Transform>_currentTarget;
    private ReactiveProperty<IBandit.BanditStatus> _currentStatus;
    private ReactiveProperty<IBandit.BanditAction> _currentAction;
    private ReactiveProperty<IBandit.BanditBattleAction> _currentBattleAction;

    private CompositeDisposable _disposablesByStatus;
    private CompositeDisposable _disposablesByAction;
    private CompositeDisposable _disposablesByBattleAction;
    private CompositeDisposable _disposablesByLifeCycle;

    //public Action<AGun> onEnemyDeadEvent;

    //いずれはEnemyも生成した側で初期化することだけ留意
    void Start()
    {
        OnSetUp(new Entity_HealthPoint(100, 100));
    }

    public override void OnSetUp(Entity_HealthPoint enemy_Bandit_HP)
    {
        //_enemy_Bandit_HP = enemy_Bandit_HP;
        //gun.position = _gunTrans.position;
        //gun.SetParent(_gunTrans);
        base.OnSetUp(enemy_Bandit_HP);

        if(_entityHP == null)
        {
            this.gameObject.SetActive(false);
            Debug.LogWarning($"{this.gameObject.name}に体力を設定してください、行動を開始できません");
            return;
        }

        _enemyFieldOfView = GetComponent<FOV>();
        //EntityMeshDisable();
        //Debug.Log(_entityRenderer);

        _currentStatus = new ReactiveProperty<IBandit.BanditStatus>(IBandit.BanditStatus.Usual);
        _currentAction = new ReactiveProperty<IBandit.BanditAction>(IBandit.BanditAction.Standing);
        _currentBattleAction = new ReactiveProperty<IBandit.BanditBattleAction>(IBandit.BanditBattleAction.Idle);
        _currentTarget = new ReactiveProperty<Transform>();

        _disposablesByStatus = new CompositeDisposable();
        _disposablesByAction = new CompositeDisposable();
        _disposablesByLifeCycle = new CompositeDisposable();
        _disposablesByBattleAction = new CompositeDisposable();

        SetEvent();
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

                if(_currentTarget.Value == null)return;    
                _statusText.transform.LookAt(_currentTarget.Value.position);
            }).AddTo(_disposablesByLifeCycle, this);
    }

    private void TriggerEventOnStatusChanged(ref IBandit.BanditStatus status)
    {
        switch(status)
        {
            case IBandit.BanditStatus.Usual : 
                _statusText.text = $"<color=#{0xFFFFFFFF:X}>{_currentStatus.Value.ToString()}</color>";
            
                StartSearchAround(0.5f); 
                _currentBattleAction.Value = IBandit.BanditBattleAction.Idle; 
                _currentAction.Value = IBandit.BanditAction.Standing; 
                break;
            case IBandit.BanditStatus.Warn : 
                _statusText.text = $"<color=#{0xFF0000FF:X}>{_currentStatus.Value.ToString()}</color>";

                StartSearchAround(0.2f);
                _currentBattleAction.Value = IBandit.BanditBattleAction.Attacking;
                break;
            case IBandit.BanditStatus.Caution : 
                _statusText.text = $"<color=#{0xFFFF00FF:X}>{_currentStatus.Value.ToString()}</color>";

                StartSearchAround(0.35f);
                _currentBattleAction.Value = IBandit.BanditBattleAction.Idle; break;
            default : break;
        }
    }

    private void TriggerEventOnActionChanged(ref IBandit.BanditAction action)
    {
        switch(action)
        {
            case IBandit.BanditAction.Standing: break;
            case IBandit.BanditAction.Running : break;
            case IBandit.BanditAction.Walking : break;
            default : break;
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
            case IBandit.BanditBattleAction.Attacking : 
                StartAttacking(0.8f); break;
            default : break;
        }
    }

    private void TriggerEventOnTargetTransformChanged(ref Transform targetTransform)
    {
        if(targetTransform != null) 
        {
            _currentStatus.Value = IBandit.BanditStatus.Warn;
            _enemyFieldOfView.ViewRadius = 40f;
            _enemyFieldOfView.ViewAngle = 85;
        }
        else if(_currentStatus.Value == IBandit.BanditStatus.Warn)
        {
            EntityActionInterval(() => _currentStatus.Value = IBandit.BanditStatus.Caution, _actionCancellationTokenSource.Token, 10f, "警戒中").Forget();
            _enemyFieldOfView.ViewRadius = 30f;
            _enemyFieldOfView.ViewAngle = 70;
        }
        else if(_currentStatus.Value == IBandit.BanditStatus.Caution)
        {
            EntityActionInterval(() => _currentStatus.Value = IBandit.BanditStatus.Usual, _actionCancellationTokenSource.Token, 10f, "警戒解除").Forget();
            _enemyFieldOfView.ViewRadius = 25f;
            _enemyFieldOfView.ViewAngle = 60;
        }
        else
        {
            _currentStatus.Value = IBandit.BanditStatus.Usual;
        }
    }

    //Statusが変化するたびに呼ばれる 
    private void StartSearchAround(float interval)
    {
        _disposablesByStatus.Clear();

        //float interval = GetSearchInterval(ref status);

        //statusに応じたインターバルで周囲を探索する
        Observable.Interval(System.TimeSpan.FromSeconds(interval)).Subscribe(_ =>
            {
                SearchAround();
            })
            .AddTo(_disposablesByStatus, this);
    }

    private void SearchAround()
    {
        List<Transform> objectList = _enemyFieldOfView.FindTargets();
        _currentTarget.Value = FindNearestObject(objectList, this.transform);

        if(_currentTarget.Value == null)return;
        //Debug.Log($"{this.gameObject.name} : {_currentTarget.Value.position}に敵がいるぞ！");
    }

    // private float GetSearchInterval(ref IBandit.BanditStatus status)
    // {
    //     switch(status)
    //     {
    //         case IBandit.BanditStatus.Usual : return 0.5f;
    //         case IBandit.BanditStatus.Warn : return 0.2f;
    //         case IBandit.BanditStatus.Caution : return 0.35f;
    //         default : return 1f;
    //     }
    // }

    public override void Move()
    {
        //ランダムな移動
    }

    public override void Rotate()
    {
        if(_currentTarget.Value == null)return;
        Quaternion targetRotation = Quaternion.LookRotation(_currentTarget.Value.position - _entityTransform.position);
        _entityTransform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(_entityTransform.eulerAngles.y, targetRotation.eulerAngles.y, _enemy_Bandit_RotateSpeed * Time.deltaTime);
    }

    private void StartAttacking(float interval)
    {
        _disposablesByBattleAction.Clear();

        if(_enemyWeaponStorage == null) return;
        if(_enemyGun == null)return;
        //statusに応じたインターバルで周囲を探索する
        Observable.Interval(System.TimeSpan.FromSeconds(interval))
            .TakeWhile(_ => _currentBattleAction.Value == IBandit.BanditBattleAction.Attacking)
            .Subscribe(_ =>
            {
                if(_enemyGun.Magazine.MagazineRemaining > 0)
                {
                    Attack();
                }
                else
                {
                    Reload(_enemyGun, new Entity_Magazine(10, 10));
                }
            })
            .AddTo(_disposablesByBattleAction, this);
    }

    public override void Attack()
    {
        _enemyGun.Shot();
    }

    public override void Hide()
    {
       //攻撃された状態のとき一定間隔で
       //HPが一定以下になったら
       //周りに仲間がいない時

       //隠れる行動
    }

    public override void Reload(AGun gun, Entity_Magazine magazine)
    {
        //リロード
        if(_isEntityActionInterval)return;
        //CancelAction(actionCancellationTokenSource);

        _actionCancellationTokenSource = new CancellationTokenSource();
        EntityActionInterval(() => gun.Reload(magazine), _actionCancellationTokenSource.Token, 2f, "リロード").Forget();
    }

    public override void Equip(AGun gun)
    {
        //Debug.Log("こんにちは！！！");
        gun.transform.SetParent(_gunTrans);
        gun.transform.SetPositionAndRotation(_gunTrans.position, this.transform.rotation);
        _enemyGun = gun;
    }

    public override void OnDamage(float damage)
    {
        _entityHP.EntityDamage(damage);

        _currentStatus.Value = IBandit.BanditStatus.Warn;

        if(IsEntityDead())
        {
            OnEntityDead();
        }
    }

    public override bool IsEntityDead()
    {
        if(_entityHP.CurrentHp <= 0)
        {
            return true;
        }

        return false;
    }

    public override void OnEntityDead()
    {
        _disposablesByLifeCycle.Clear();
        _disposablesByStatus.Clear();

        this.gameObject.SetActive(false);
        Debug.Log($"{this.gameObject.name}はやられた！");

        //onEnemyDeadEvent?.Invoke(_enemyGun);
        base.OnEntityDead();
    }   
}
