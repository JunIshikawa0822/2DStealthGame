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
        _mouseHitLayer = gameStat.mouseLayHitlayer;
        // Actionイベント登録
        _gameInputs.PlayerActionTest.PlayerMoveTest.started += OnMoveInput;
        _gameInputs.PlayerActionTest.PlayerMoveTest.performed += OnMoveInput;
        _gameInputs.PlayerActionTest.PlayerMoveTest.canceled += OnMoveInput;

        _gameInputs.PlayerActionTest.PlayerAttackTest.started += OnAttackStartInput;
        _gameInputs.PlayerActionTest.PlayerAttackTest.performed += OnAttackingInput;
        _gameInputs.PlayerActionTest.PlayerAttackTest.canceled += OnAttackEndInput;

        _gameInputs.PlayerActionTest.PlayerReloadTest.started += OnReloadInput;
        _gameInputs.PlayerActionTest.PlayerInventoryTest.started += OnInventoryInput;
        
        _gameInputs.PlayerActionTest.DashTest.started += OnDashInput;

        _gameInputs.PlayerActionTest.SelectingIndexChangeTest.started += OnPlayerSelectingIndexChangeFromKey;

        _gameInputs.Enable();
    }

    public void OnPreUpdate()
    {
        Debug.Log(_gameInputs);
        _cursorScreenPosition = _gameInputs.PlayerActionTest.CursorPosition.ReadValue<Vector2>();

        if(IsCursorRayHit(_cursorScreenPosition, out _rayCastHit))
        {
            _cursorWorldPosition = _rayCastHit.point;
        }

        if(_cursorScreenPosition != null) gameStat.cursorScreenPosition = _cursorScreenPosition;
        if(_cursorWorldPosition != null) gameStat.cursorWorldPosition = _cursorWorldPosition;
        
        gameStat.cursorAlignShotPosition = RectTransformUtility.WorldToScreenPoint 
        (
            gameStat.camera, 
            new Vector3(_cursorWorldPosition.x, gameStat.player.equipPos.transform.position.y, _cursorWorldPosition.z)
        );
    }
    
    private void OnDestroy()
    {
        _gameInputs?.Dispose();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        gameStat.moveDirection = context.ReadValue<Vector2>() * 0.8f;
    }

    private void OnDashInput(InputAction.CallbackContext context)
    {
        gameStat.moveDirection = (gameStat.moveDirection * 2).normalized;
    }

    private void OnAttackStartInput(InputAction.CallbackContext context)
    {
        gameStat.onPlayerAttackStartEvent?.Invoke();
    }

    private void OnAttackingInput(InputAction.CallbackContext context)
    {
        gameStat.onPlayerAttackingEvent?.Invoke();
    }

    private void OnAttackEndInput(InputAction.CallbackContext context)
    {
        gameStat.onPlayerAttackEndEvent?.Invoke();
    }

    private void OnReloadInput(InputAction.CallbackContext context)
    {
        //Debug.Log("Reload");
        gameStat.onPlayerReloadEvent?.Invoke();
    }

    private void OnInteractiveInput(InputAction.CallbackContext context)
    {

    }

    private void OnPlayerSelectingIndexChangeFromKey(InputAction.CallbackContext context)
    {
        Debug.Log(context.control.name);
        Debug.Log("選択前" + gameStat.selectingGunsArrayIndex);

        string pressedKey = context.control.name;
        if(int.TryParse(pressedKey, out int index))
        {
            gameStat.selectingGunsArrayIndex = index - 1;
        }
        else
        {
            Debug.Log($"{pressedKey}は無効なキー入力です");
        }

        Debug.Log("選択後" + gameStat.selectingGunsArrayIndex);

        gameStat.onSelectGunChange?.Invoke();
    }

    private void OnInventoryInput(InputAction.CallbackContext context)
    {
        gameStat.onInventoryActiveEvent?.Invoke();
        Cursor.visible = gameStat.isInventoryPanelActive;
    }

    private Vector3 ConvertScreenToWorldPoint(Vector2 screenPos)
    {
        return gameStat.camera.ScreenToWorldPoint(screenPos);
    }

    private bool IsCursorRayHit(Vector2 screenPos, out RaycastHit raycastHit)
    {
        bool isRayhit = false;

        Ray ray = gameStat.camera.ScreenPointToRay(screenPos);
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
