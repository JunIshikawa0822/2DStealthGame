using UnityEngine;
using UnityEngine.InputSystem;
public class InputSystem : ASystem, IOnPreUpdate
{
    private InputAction_Test _gameInputs;
    private LayerMask _mouseHitLayer;
    private Vector2 _cursorScreenPosition;
    private Vector3 _cursorWorldPosition;
    private RaycastHit _rayCastHit;
    
    public override void OnSetUp()
    {
        _gameInputs = new InputAction_Test();
        _mouseHitLayer = 1 << 6;

        // Actionイベント登録
        _gameInputs.PlayerActionTest.PlayerMoveTest.started += OnMoveInput;
        _gameInputs.PlayerActionTest.PlayerMoveTest.performed += OnMoveInput;
        _gameInputs.PlayerActionTest.PlayerMoveTest.canceled += OnMoveInput;

        _gameInputs.PlayerActionTest.PlayerAttackTest.started += OnAttackInput;
        _gameInputs.PlayerActionTest.PlayerReloadTest.started += OnReloadInput;

        _gameInputs.Enable();

        Cursor.visible = false;
    }

    public void OnPreUpdate()
    {
        _cursorScreenPosition = _gameInputs.PlayerActionTest.CursorPosition.ReadValue<Vector2>();

        if(IsCursorRayHit(_cursorScreenPosition, out _rayCastHit))
        {
            _cursorWorldPosition = _rayCastHit.point;
        }

        if(_cursorScreenPosition != null) gameStat.cursorScreenPosition = _cursorScreenPosition;
        if(_cursorWorldPosition != null) gameStat.cursorWorldPosition = _cursorWorldPosition;
    }
    
    private void OnDestroy()
    {
        _gameInputs?.Dispose();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        gameStat.moveDirection = context.ReadValue<Vector2>();
    }

    private void OnAttackInput(InputAction.CallbackContext context)
    {
        //Debug.Log("click");
        gameStat.onPlayerAttackEvent?.Invoke();
    }

    private void OnReloadInput(InputAction.CallbackContext context)
    {
        //Debug.Log("Reload");
        gameStat.onPlayerReloadEvent?.Invoke();
    }

    private Vector3 ConvertScreenToWorldPoint(Vector2 screenPos)
    {
        return Camera.main.ScreenToWorldPoint(screenPos);
    }

    private bool IsCursorRayHit(Vector2 screenPos, out RaycastHit raycastHit)
    {
        bool isRayhit = false;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _mouseHitLayer))
        {
            isRayhit = true;
        }
        else
        {
            isRayhit = false;
        }

        raycastHit = hitInfo;
        return isRayhit;
    }
}
