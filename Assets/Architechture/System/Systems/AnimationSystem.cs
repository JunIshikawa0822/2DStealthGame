using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AnimationSystem : ASystem, IOnUpdate
{
    Animator _playerAnimator;
    public override void OnSetUp()
    {
        _playerAnimator = gameStat.player.GetComponentInChildren<Animator>();
    }

    public void OnUpdate()
    {
        float horizontal = gameStat.moveDirection.x;
        float vertical = gameStat.moveDirection.y;

        _playerAnimator.SetFloat("Horizontal", horizontal);
        _playerAnimator.SetFloat("Vertical", vertical);
    }
}
