using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class HandGun : MonoBehaviour, IGun_10mm
{
    //発射の内部的な処理に必要
    //----------------------------------------
    [SerializeField]
    private float _muzzleVelocity = 700f;
    [SerializeField] 
    private Transform _muzzlePosition;

    //もっと一般化していい奴ら↓
    private IObjectPool<ABullet> _objectPool;
    private IBulletFactories _bulletFactories;
    private IFactory<ABullet> _bulletcaliberFactory;
    //----------------------------------------
    private bool isCooldownActive;
    private CancellationTokenSource cancellationTokenSource;
    //----------------------------------------

    //銃に必要な処理
    //----------------------------------------
    private Entity_Magazine _magazine;
    //----------------------------------------

    public void OnSetUp(IBulletFactories bulletFactories, IObjectPool<ABullet> objectPool)
    {
        _bulletFactories = bulletFactories;
        _objectPool = objectPool;

        isCooldownActive = false;
        
        _bulletcaliberFactory = GetFactory_10mm<IBType_10mm>();
    }

    public void OnUpdate()
    {

    }

    public void Shot()
    {
        //マガジンがないor弾がないとそもそも撃てない
        if(_magazine == null || _magazine.MagazineRemaining < 1)
        {
            Debug.Log("弾、ないよ");
            return;
        }

        //射撃と射撃の間隔を制御
        if(isCooldownActive)return;
        cancellationTokenSource = new CancellationTokenSource();
        ShotInterval(cancellationTokenSource.Token, 0.5f).Forget();

        //BulletのFactoryをチェック
        _bulletcaliberFactory = GetFactory_10mm<IBType_10mm>();
        if(_bulletcaliberFactory == null)return;
        
        //Poolからもってくる
        ABullet bullet = _objectPool.GetFromPool(_bulletcaliberFactory) as ABullet;

        if(bullet == null)
        {
            Debug.Log("キャスト無理ぃ");
            return;
        }

        if (bullet.gameObject == null)return;
        //発射
        Debug.Log("発射");
        bullet.Init(_muzzlePosition.position);
        bullet.GetBulletTransform().SetPositionAndRotation(_muzzlePosition.position, _muzzlePosition.rotation);
        bullet.GetBulletRigidbody().AddForce(bullet.gameObject.transform.forward * _muzzleVelocity, ForceMode.Acceleration);

        //弾を消費する
        _magazine.ConsumeBullet();
    }

    public void Reload(Entity_Magazine magazine)
    {
        _magazine = magazine;
    }

    public void Jam()
    {

    }

    public async UniTask ShotInterval(CancellationToken token, float time)
    {
        isCooldownActive = true;

        try
        {
            // 指定されたクールタイム期間を待つ (キャンセル可能)
            Debug.Log("クールダウン開始");
            await UniTask.Delay((int)(time * 1000), cancellationToken: token);
            Debug.Log("クールダウン終了");
        }
        catch
        {
            Debug.Log("クールダウンがキャンセルされました");
        }
        finally
        {
            isCooldownActive = false; // クールタイム終了（またはキャンセル)
        }
    }

    private void CancelInterval()
    {
        if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }
    }

    public IFactory<ABullet> GetFactory_10mm<T>() where T : IBType_10mm
    {
        // ここで、T 型が IBType_10mm を継承していることが保証されています
        return _bulletFactories.BulletFactory(typeof(T));
    }
}
