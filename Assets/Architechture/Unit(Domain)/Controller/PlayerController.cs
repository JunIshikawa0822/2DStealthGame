using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : APlayer
{
    [SerializeField] private float _moveForce = 5;
    [SerializeField] private float _jumpForce = 5;

    private IGun _equipGun1;
    private IGun _equipGun2;
    private int _selectIndex;

    public override void OnSetUp(int playerHp)
    {
        base.OnSetUp(playerHp);

        _selectIndex = 0;
    }

    public override void SetEquipment(IGun item, int index)
    {
        switch(index)
        {
            default:
            case 0: _equipGun1 = item;
                    break;
            case 1: _equipGun2 = item;
                    break;
        }
    }

    public override void OnMove(Vector2 inputDirection)
    {
        Debug.Log(inputDirection);
        _entityRigidbody.AddForce(new Vector3(inputDirection.x, 0, inputDirection.y) * _moveForce, ForceMode.Force);
    }

    public override void OnAttack()
    {
        switch(_selectIndex)
        {
            default:
            case 0: _equipGun1.Shot();
                    break;
            case 1: _equipGun2.Shot();
                    break;
        }
    }

    public override void OnEntityDead()
    {
        Debug.Log("テストだよん");
    }
}
