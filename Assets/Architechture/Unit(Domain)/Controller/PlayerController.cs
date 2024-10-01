using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : APlayer
{
    [SerializeField] private float _moveForce = 5;
    //[SerializeField] private float _jumpForce = 5;

    private IGun[] _playerGunsArray = new IGun[2];
    private int _selectGunIndex;

    private Quaternion targetRotation;
    private float rotationSpeed = 500;

    public override void OnSetUp(int playerHp)
    {
        base.OnSetUp(playerHp);
        _selectGunIndex = 0;
    }

    public override void SetEquipment(IGun item, int index)
    {
        switch(index)
        {
            default:
            case 0: _playerGunsArray[0] = item;
                    break;
            case 1: _playerGunsArray[1] = item;
                    break;
        }
    }

    public override void OnMove(Vector2 inputDirection, Vector3 mouseWorldPosition)
    {
        //移動
        _entityRigidbody.AddForce(new Vector3(inputDirection.x, 0, inputDirection.y) * _moveForce, ForceMode.Force);

        //回転
        targetRotation = Quaternion.LookRotation(mouseWorldPosition - _entityTransform.position);
        _entityTransform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(_entityTransform.eulerAngles.y, targetRotation.eulerAngles.y, rotationSpeed * Time.deltaTime);
    }

    public override void OnAttack()
    {
        _playerGunsArray[_selectGunIndex].Shot();
    }

    public override void OnReload()
    {
        Entity_Magazine newMagazine = new Entity_Magazine(10, 10);
        _playerGunsArray[_selectGunIndex].Reload(newMagazine);
    }

    public override void OnDamage(float damage)
    {
        
    }

    public override void OnEntityDead()
    {
        Debug.Log("テストだよん");
    }
}
