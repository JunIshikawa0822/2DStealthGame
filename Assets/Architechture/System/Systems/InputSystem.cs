using UnityEngine;
using UnityEngine.InputSystem;
public class InputSystem : ASystem, IOnPreUpdate
{
    private InputAction_Test _gameInputs;
    //private Vector2 _moveInputValue;
    
    public override void OnSetUp()
    {
        _gameInputs = new InputAction_Test();

        // Actionイベント登録
        _gameInputs.PlayerActionTest.PlayerMoveTest.started += OnMoveInput;
        _gameInputs.PlayerActionTest.PlayerMoveTest.performed += OnMoveInput;
        _gameInputs.PlayerActionTest.PlayerMoveTest.canceled += OnMoveInput;

        //_gameInputs.PlayerActionTest.PlayerMoveTest.performed += OnJump;

        // Input Actionを機能させるためには、
        // 有効化する必要がある
        _gameInputs.Enable();
    }

    public void OnPreUpdate()
    {
        
    }
    
    private void OnDestroy()
    {
        // 自身でインスタンス化したActionクラスはIDisposableを実装しているので、
        // 必ずDisposeする必要がある
        _gameInputs?.Dispose();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        // Moveアクションの入力取得
       gameStat.moveDirection = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        // ジャンプする力を与える
        //_rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }
}
