using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : AEntity, IPlayer
{
    [SerializeField] private float _moveForce = 5;
    [SerializeField] private float _jumpForce = 5;

    private IGun _equipGun1;
    private IGun _equipGun2;
    private int _selectIndex;

    public override void OnSetUp()
    {
        base.OnSetUp();

        _selectIndex = 0;
    }

    public void SetEquipment(IGun item, int index)
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

    public void OnMove(Vector2 inputDirection)
    {
        _entityRigidbody.AddForce(new Vector3(inputDirection.x, 0, inputDirection.y) * _moveForce);
    }

    public void OnAttack()
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
}
