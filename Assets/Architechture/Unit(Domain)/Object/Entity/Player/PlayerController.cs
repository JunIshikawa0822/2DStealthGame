using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
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
    //private AGun[] _playerGunsArray;
    //private int _selectingGunsArrayIndex;
    //private CompositeDisposable _disposablesByLifeCycle;

    [HideInInspector]public Action<IStorage> storageFindEvent;
    [HideInInspector]public Action leaveStorageEvent;
    public override void OnSetUp(Entity_HealthPoint playerHP)
    {
        base.OnSetUp(playerHP);

        //_fieldOfView = GetComponent<FOV>();
        _playerAnimator = GetComponent<Animator>();
    }

    public void Move(Vector2 inputDirection)
    {
        //Debug.Log("移動");
        //移動

        _entityRigidbody.velocity = new Vector3(inputDirection.x, 0, inputDirection.y) * _playerMoveForce;
        //_entityRigidbody.AddForce(new Vector3(inputDirection.x, 0, inputDirection.y) * _playerMoveForce, ForceMode.Force); 
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
        // if(Input.GetKeyDown(KeyCode.J))
        // {
        //     List<string> isList = new List<string>();
        //     foreach(AGun gun in _playerGunsArray)
        //     {
        //         if(gun == null)
        //         {
        //             isList.Add("入っていない");
        //         }
        //         else
        //         {
        //             isList.Add("入っている");
        //         }
        //     }

        //     //Debug.Log("1" + _playerGun1.Name);
        //     //Debug.Log("2" + _playerGun2.Name);

        //     Debug.Log(string.Join(",", isList));
        // }

        Collider[] collides = Physics.OverlapSphere(this.transform.position, 1, 1 << 11);
        if(collides.Length > 0)
        {
            storageFindEvent?.Invoke(collides[0].GetComponent<NormalStorage>());
        }
        else
        {
            leaveStorageEvent?.Invoke();
        }
    }

    public void Attack(AGun gun)
    {
        if(gun == null)return;
        gun.Shot();

        _playerAnimator.SetTrigger("Shot");

        EntityActionInterval(null, _actionCancellationTokenSource.Token, gun.ShotInterval, "動けない").Forget();
    }

    public void Reload(AGun gun)
    {
        if(_isEntityActionInterval)return;

        if(gun == null)return;
        if(gun.Magazine.MagazineRemaining >= gun.Magazine.MagazineCapacity) return;
        
        uint max = gun.MaxAmmoNum;
        uint current = gun.MaxAmmoNum;

        Debug.Log(max + "," + current);
        Entity_Magazine magazine = new Entity_Magazine(max, current);

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
        _entityHP.EntityDamage(damage);

        Debug.Log(_entityHP.CurrentHp);

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
            _isEntityActionInterval = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Storage見つけた");
        if(collider.gameObject.tag == "Storage")
        {
            Debug.Log("Storage見つけた");
            storageFindEvent?.Invoke(collider.GetComponent<NormalStorage>());
        }
        else
        {
            //storageFindEvent?.Invoke(collider.GetComponent<AEntity>().Storage);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if(collider.gameObject.tag == "Storage")
        {
            Debug.Log("Storage離れた");
            //leaveStorageEvent?.Invoke(collider.GetComponent<NormalStorage>());
        }
        else
        {
            //leaveStorageEvent?.Invoke(collider.GetComponent<AEntity>().Storage);
        }
    }
}
