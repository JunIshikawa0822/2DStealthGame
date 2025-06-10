using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using JunUtilities;
using UniRx;
using UnityEngine.Rendering;
using Unity.Entities.UniversalDelegates;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerController : AEntity
{
    [SerializeField]
    private float _player_RotateSpeed = 500;

    //private Entity_HealthPoint _playerHP;

    private AGun[] _playerGunArray;

    public Transform equipPos;
    public Transform subPos;

    [SerializeField] private float _playerMoveForce = 50;

    //private FOV _fieldOfView;
    private Animator _playerAnimator;

    public override IStorage Storage{get => _playerStorage;}
    public IStorage PlayerWeaponStorage1{get => _weaponStorage1;}
    public IStorage PlayerWeaponStorage2{get => _weaponStorage2;}
    [SerializeField] WeaponStorage _weaponStorage1;
    [SerializeField] WeaponStorage _weaponStorage2;
    [SerializeField] NormalStorage _playerStorage;

    public event Action<IStorage> storageFindAction;
    public event Action<IStorage> storageLeaveAction;
    
    public override void OnSetUp(Entity_HealthPoint playerHP, AABB3DTree<(Transform, AlignedOBB)> stageObjectTree)
    {
        base.OnSetUp(playerHP, stageObjectTree);

        //_fieldOfView = GetComponent<FOV>();
        _playerAnimator = GetComponent<Animator>();
        _playerGunArray = new AGun[2];
    }

    public void Move(Vector2 inputDirection)
    {
        Vector3 velocity = new Vector3(inputDirection.x, 0, inputDirection.y); // 上下のキー入力からZ軸方向の移動量を取得
        transform.localPosition += velocity * _playerMoveForce * Time.fixedDeltaTime;
    }

    public void Rotate(Vector3 mouseWorldPosition)
    {
        //回転
        Quaternion targetRotation = Quaternion.LookRotation(mouseWorldPosition - _entityTransform.position);
        _entityTransform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(_entityTransform.eulerAngles.y, targetRotation.eulerAngles.y, _player_RotateSpeed * Time.deltaTime);
    }

    public void MoveAnimation(Vector2 inputDirection)
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

        //Debug.Log(new Vector2(-relativeY, relativeX));

            return new Vector2(-relativeY, relativeX);
        }
    }

    void Update()
    {
        equipPos.SetPositionAndRotation(equipPos.position, this.transform.rotation);

        // Collider[] collides = Physics.OverlapSphere(this.transform.position, 1, 1 << 11);
        // if(collides.Length > 0)
        // {
        //     storageFindEvent?.Invoke(collides[0].GetComponent<NormalStorage>());
        // }
        // else
        // {
        //     leaveStorageEvent?.Invoke();
        // }
    }

    public void AttackStart(AGun gun)
    {
        if(gun == null)return;
        if(IsEntityActionInterval)return;

        IsEntityActionInterval = true;
        gun.TriggerOn();

        _playerAnimator.SetTrigger("Shot");

        //EntityActionInterval(null, _actionCancellationTokenSource.Token, gun.ShotInterval, "動けない").Forget();
    }

    public void AttackEnd(AGun gun)
    {
        if(gun == null)return;
        
        IsEntityActionInterval = false;
        gun.TriggerOff();
    }

    public void Reload(AGun gun)
    {
        //Debug.Log("リロード");
        if(IsEntityActionInterval)return;

        //Debug.Log("デバッグその1");
        if(gun == null)return;
        uint bulletRemain = gun.Magazine.MagazineRemaining;
        uint bulletCapacity = gun.Magazine.MagazineCapacity;
        
        //Debug.Log("デバッグその2");
        if(bulletRemain >= bulletCapacity) return;

        //Debug.Log(gun.Data.CaliberType);
        IInventoryItem ammoItem = _playerStorage.FindItem<I_Data_Ammo>((I_Data_Ammo ammo) => ammo.CaliberType == gun.Data.CaliberType);
        //Debug.Log(ammoItem);
        //Debug.Log("デバッグその3");
        if(ammoItem == null) return;

        //Debug.Log("リロードしている");
        uint reloadNum = ammoItem.StackingNum >= bulletCapacity ? bulletCapacity - bulletRemain : ammoItem.StackingNum;

        ammoItem.StackingNum -= reloadNum;
        Entity_Magazine magazine = new Entity_Magazine(bulletCapacity, reloadNum + bulletRemain);
        EntityActionInterval(() => gun.Reload(magazine), _actionCancellationTokenSource.Token, gun.ReloadTime, "リロード").Forget();
    }

    public void EquipMotion(AGun gun)
    {
        Debug.Log("Euipment実行");
        if(gun == null)
        {
            _playerAnimator.SetInteger("Equip", 0);
        }
        else
        {
            Debug.Log("nullじゃない");
            if(gun.Data is I_Data_Rifle)_playerAnimator.SetInteger("Equip", 2);
            else if(gun.Data is I_Data_HandGun)_playerAnimator.SetInteger("Equip", 1);
            else _playerAnimator.SetInteger("Equip", 2);

            Debug.Log(equipPos);
            gun.gameObject.SetActive(true);
            gun.transform.SetParent(equipPos);
            gun.transform.SetPositionAndRotation(equipPos.position, this.transform.rotation);
        }
    }

    public void UnEquipMotion(AGun gun)
    {
        Debug.Log("UnEquip");
        if(gun == null)
        {
            _playerAnimator.SetInteger("Equip", 0);
            return;
        }
        else
        {
            gun.gameObject.SetActive(false);
            gun.transform.SetParent(subPos);
            gun.transform.SetPositionAndRotation(subPos.position, this.transform.rotation);
        }
    }

    public override void OnDamage(float damage)
    {
        EntityHP.EntityDamage(damage);

        Debug.Log(EntityHP.CurrentHp);

        if(IsEntityDead())
        {
            OnEntityDead();
        }
    }

    public override void OnEntityDead()
    {
        Debug.Log($"プレイヤー({this.gameObject.name})はやられた！");

        base.OnEntityDead();
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
            IsEntityActionInterval = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        Debug.Log(other.TryGetComponent<IStorage>(out IStorage some));
        if (other.CompareTag("Storage") && other.TryGetComponent<IStorage>(out IStorage storage))
        {
            Debug.Log("Storageみつけた");
            storageFindAction?.Invoke(storage);
        }
    }
    
    public void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        if (other.CompareTag("Storage") && other.TryGetComponent(out IStorage storage))
        {
            Debug.Log("Storage離れた");
            storageLeaveAction?.Invoke(storage);
        }
    }
}
