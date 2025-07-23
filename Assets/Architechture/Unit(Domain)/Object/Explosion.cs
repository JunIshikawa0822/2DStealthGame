using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Explosion : MonoBehaviour
{
    [Header("爆風に当たったときに吹っ飛ぶ力の強さ")]
    [SerializeField]
    private float _futtobiPower;

    [Header("爆風の判定が実際に発生するまでのディレイ")]
    [SerializeField]
    private float _startDelaySeconds = 0.1f;
    [Header("爆風の持続フレーム数")][SerializeField] private int _durationFrameCount = 1;

    [Header("エフェクト含めすべての再生が終了するまでの時間")]
    [SerializeField]
    private float _stopSeconds = 2f;
    [SerializeField] private ParticleSystem _effect;
    [SerializeField] private SphereCollider _collider;

    private void Awake()
    {
        _effect.Stop();
        _collider.enabled = false;
    }

    public void Explode()
    {
        var ct = this.GetCancellationTokenOnDestroy();
        ExplodeAsync(ct).Forget();
        StopAsync(ct).Forget();

        // エフェクト
        _effect.Play();
    }

    private async UniTaskVoid ExplodeAsync(CancellationToken ct)
    {
        // 指定秒数が経過するまでFixedUpdate上で待つ
        var delayCount = Mathf.Max(0, _startDelaySeconds);
        while (delayCount > 0)
        {
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, ct);
            delayCount -= Time.fixedDeltaTime;
        }

        // 時間経過したらコライダを有効化して爆発の当たり判定が出る
        _collider.enabled = true;

        // 一定フレーム数有効化
        await UniTask.DelayFrame(_durationFrameCount, PlayerLoopTiming.FixedUpdate, ct);

        // 当たり判定無効化
        _collider.enabled = false;
    }

    private async UniTaskVoid StopAsync(CancellationToken ct)
    {
        // 時間経過後に消す
        await UniTask.Delay((int)(_stopSeconds * 1000), cancellationToken: ct);
        _effect.Stop();
        // _sfx.Stop();
        _collider.enabled = false;

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var rigidBody = other.GetComponentInParent<Rigidbody>();
        var script = other.GetComponentInParent<AEntity>();

        if (script != null)
        {
            script.OnDamage(_futtobiPower);
        }

        if (rigidBody == null) return;

        var direction = (other.transform.position - transform.position).normalized;
        rigidBody.AddForce(direction * _futtobiPower, ForceMode.VelocityChange);
    }
}
