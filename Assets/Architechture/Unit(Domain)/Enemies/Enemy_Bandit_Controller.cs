using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bandit_Controller : AEnemy, IEnemy
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
        OnSetUp();
    }

    public void OnSetUp()
    {
        Entity_HealthPoint enemyHP = new Entity_HealthPoint(_maxHP, _initHP);
        _enemyFindView = new FindOpponent(opponentLayer, obstacleLayer);
        
        EntitySetUp(enemyHP);

        OnEntityMeshDisable();

        if(_entityHP == null)
        {
            this.gameObject.SetActive(false);
            Debug.Log($"{this.gameObject.name}に体力を設定してください");
        }
    }

    public void OnMove()
    {
        //ランダムな移動
    }

    public void OnAttack()
    {
        //交戦中のみ

        //相手の位置を確認
        //相手に向かって射撃
    }

    public void OnHide()
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
        _entityHP.EntityDamage(damage);

        if(IsEntityDead())
        {
            OnEntityDead();
        }
    }

    public void OnReload()
    {
        //リロード
    }

    public override void OnEntityDead()
    {
        this.gameObject.SetActive(false);
        Debug.Log($"{this.gameObject.name}はやられた！");
    }
}
