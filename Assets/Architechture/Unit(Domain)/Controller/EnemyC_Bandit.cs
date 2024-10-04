using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : AEnemy
{
    [SerializeField]
    float _maxHP, _currentHP;

    void Start()
    {
        OnSetUp(new Entity_HealthPoint(_maxHP, _currentHP));
    }

    public override void OnSetUp(Entity_HealthPoint enemyHP)
    {
        EntitySetUp(enemyHP);

        OnEntityMeshDisable();

        if(_entityHP == null)
        {
            this.gameObject.SetActive(false);
            Debug.Log($"{this.gameObject.name}に体力を設定してください");
        }
    }

    public override void OnMove()
    {
        
    }

    public override void OnAttack()
    {
        
    }

    public override void OnDamage(float damage)
    {
        _entityHP.EntityDamage(damage);

        if(_entityHP.CurrentHp <= 0)
        {
            OnEntityDead();
        }
    }

    public override void OnReload()
    {
        
    }

    public override void OnEntityDead()
    {
        this.gameObject.SetActive(false);
        Debug.Log($"{this.gameObject.name}はやられた！");
    }

    public override void OnEntityMeshDisable()
    {
        _entityRenderer.enabled = false;

        foreach(MeshRenderer mesh in _entityChildrenMeshsArray)
        {
            mesh.enabled = false;
        }
    }

    public override void OnEntityMeshAble()
    {
        _entityRenderer.enabled = true;

        foreach(MeshRenderer mesh in _entityChildrenMeshsArray)
        {
            mesh.enabled = true;
        }
    }
}
