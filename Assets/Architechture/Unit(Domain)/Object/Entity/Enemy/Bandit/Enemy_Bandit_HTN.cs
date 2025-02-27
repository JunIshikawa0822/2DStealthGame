using JetBrains.Annotations;
using JunUtilities;
using UnityEngine;
using System.Collections.Generic;

public class Enemy_Bandit_HTN : AEnemy
{
    [SerializeField] private Transform _targetPos;
    [SerializeField]
    private Transform _gunTrans;
    [SerializeField]
    private float _enemy_Bandit_RotateSpeed;
    
    [SerializeField]private NormalStorage _enemyStorage;
    [SerializeField]private WeaponStorage _enemyWeaponStorage;
    public override IStorage Storage {get => _enemyStorage;}
    public override IStorage WeaponStorage {get => _enemyWeaponStorage;}
    
    private Transform _currentTarget;
    
    private HTNPlanner _enemyAI;
    private RRTStar _enemyMoveAlgorithm;

    public override void OnSetUp(Entity_HealthPoint enemy_Bandit_HP)
    {
        base.OnSetUp(enemy_Bandit_HP);

        if(EntityHP == null)
        {
            this.gameObject.SetActive(false);
            Debug.LogWarning($"{this.gameObject.name}に体力を設定してください、行動を開始できません");
            return;
        }
    }
    
    public override void SetUpEnemyAI(HTNPlanner enemyAI, RRTStar enemyMoveAlgorithm)
    {
        _enemyAI = enemyAI;
        _enemyMoveAlgorithm = enemyMoveAlgorithm;
    }

    public override void SetUpEnemyAI(RRTStar enemyMoveAlgorithm)
    {
        _enemyMoveAlgorithm = enemyMoveAlgorithm;
        Debug.Log(_targetPos);
        List<Vector3> points = _enemyMoveAlgorithm.FindPath(this.transform.position, _targetPos.position);
        //Debug.Log(string.Join(", ", _enemyMoveAlgorithm.FindPath(this.transform.position, _targetPos.position)));
    }
    
    public override void Rotate()
    {
        if(_currentTarget == null)return;
        Quaternion targetRotation = Quaternion.LookRotation(_currentTarget.position - _entityTransform.position);
        _entityTransform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(_entityTransform.eulerAngles.y, targetRotation.eulerAngles.y, _enemy_Bandit_RotateSpeed * Time.deltaTime);
    }

    public override void Attack()
    {
        if(EnemyGun == null)return;
        EnemyGun.TriggerOn();
    }

    public override void Reload()
    {
        if(EnemyGun == null)return;
        if(EnemyGun.Magazine == null)return;
        if(EnemyGun.Magazine.MagazineRemaining >= EnemyGun.Magazine.MagazineCapacity)return;

        EnemyGun.Reload(new Entity_Magazine(EnemyGun.Magazine.MagazineCapacity, EnemyGun.Magazine.MagazineCapacity));
    }

    public override void Equip(AGun gun)
    {
        EnemyGun = gun;
        gun.transform.SetParent(_gunTrans);
        gun.transform.SetPositionAndRotation(_gunTrans.position, this.transform.rotation);
    }
    public override void OnDamage(float damage)
    {
        EntityHP.EntityDamage(damage);
        if(IsEntityDead())
        {
            base.OnEntityDead();
            
        }
    } 
}
