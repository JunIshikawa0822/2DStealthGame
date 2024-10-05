using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : AEnemy
{
    [SerializeField]
    float _maxHP, _initHP;

    [SerializeField]
    private LayerMask opponentLayer;

    [SerializeField]
    private LayerMask obstacleLayer;

    private bool isFighting;

    //いずれはEnemyも生成した側で初期化することだけ留意
    void Start()
    {
        Entity_HealthPoint enemyHP = new Entity_HealthPoint(_maxHP, _initHP);
        FindOpponent findOpponent = new FindOpponent(opponentLayer, obstacleLayer);

        OnSetUp(enemyHP, findOpponent);
    }

    public override void OnSetUp(Entity_HealthPoint enemyHP, FindOpponent find)
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
        //ランダムな移動
    }

    public override void OnAttack()
    {
        //交戦中のみ

        //相手の位置を確認
        //相手に向かって射撃
    }

    public override void OnHide()
    {
       //攻撃された状態のとき一定間隔で
       //HPが一定以下になったら
       //周りに仲間がいない時

       //隠れる行動
    }

    // public override void SearchAround(Transform transform, float radius)
    // {
    //     //交戦中

    //     //敵を探す動き
    //     opponentsArray = Physics.OverlapSphere(transform.position, radius, opponentLayer);
    // }

    public override void OnDamage(float damage)
    {
        _entityHP = new Entity_HealthPoint(_maxHP, _entityHP.CurrentHp - damage);

        if(_entityHP.CurrentHp <= 0)
        {
            OnEntityDead();
        }
    }

    public override void OnReload()
    {
        //リロード
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
