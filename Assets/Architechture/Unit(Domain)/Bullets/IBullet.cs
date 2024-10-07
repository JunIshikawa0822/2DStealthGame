using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBullet
{
    //セットアップ
    public void OnSetUp(float lifeDist);
    //弾の初期化
    public void Init(Vector3 position);
    //破壊の判定
    public bool IsBeyondLifeDistance();
    //衝突判定
    public bool IsBulletCollide();

}
